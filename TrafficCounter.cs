using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Configuration;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using System.Linq;
using System.Threading;

using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using TreeLib;
using VTC.ServerReporting.ReportItems;

namespace VTC
{
   public partial class TrafficCounter : Form
   {
      private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, 1.0, 1.0);
      private static Capture _cameraCapture;

      //************* Main image variables ***************
      Image<Bgr, Byte> Frame;               //current Frame from camera
      Image<Bgr, float> Color_Background;   //Average Background being formed
      Image<Bgr, float> Color_Difference;   //Difference between the two frames
      Image<Gray, byte> Movement_Mask;      //Thresholded, b&w movement mask
      Image<Bgr, float> ROI_image;          //Area occupied by traffic

      //************* Background subtraction parameters ***************
      double alpha = 0.001;                 //stores alpha for thread access
      int color_threshold = 40;             //Threshold below which frame-movement is ignored

      //************* Object detection parameters ***************  
      int car_radius = 12;                  //Radius of car image in pixels
      double noise_mass = 000000.0;         //Background movement noise
      double per_car = 40000.0;             //White pixels per car in image
      int max_object_count = 20;            //Maximum number of blobs to detect
      private RegionConfig _regionConfig;

      HypothesisTree hypothesis_tree;
      //Multiple hypothesis tracking parameters
      int miss_threshold = 30;               //Number of misses to consider an object gone
      int max_targets = 10;                 //Maximum number of concurrently tracked targets
      int tree_depth = 3;                   //Maximum allowed hypothesis tree depth
      int k_hypotheses = 2;                 //Branching factor for hypothesis tree
      int validation_region_deviation = 7;  //Mahalanobis distance multiplier used in measurement gating
      double Pd = 0.80;                      //Probability of object detection
      double Px = 0.0001;                    //Probability of track termination
      double lambda_x = 20;                 //Termination likelihood
      double lambda_f = 0.4e-6;            //Density of Poisson-distributed false positives
      double lambda_n = 0.6e-6;             //Density of Poission-distributed new vehicles
      double pruning_ratio = 0.001;         //Probability ratio at which hypotheses are pruned
      double q = 10;                       //Process noise matrix multiplier
      double r = 10;                        //Measurement noise matrix multiplier

      //************* Rendering parameters ***************  
      double velocity_render_multiplier = 1.0; //Velocity is multiplied by this quantity to give a line length for rendering
      double render_radius_threshold = 30.0;   // Uncertainty radius is not rendered if above this value
      bool render_clean = true;

      bool VIDEO_FILE = false;

      //************* Server connection parameters ***************  
      string intersection_id = "";
      private int FRAME_UPLOAD_INTERVAL_MINUTES = 1;
       
      public TrafficCounter()
      {
         StateHypothesis initial_hypothesis = new StateHypothesis(miss_threshold);
         hypothesis_tree = new HypothesisTree(initial_hypothesis);
         intersection_id = ConfigurationSettings.AppSettings["IntersectionId"];
         InitializeComponent();
         Run();
      }

      public TrafficCounter(string argument)
      {
          StateHypothesis initial_hypothesis = new StateHypothesis(miss_threshold);
          if (argument == "VIDEO_FILE")
              VIDEO_FILE = true;
      
          hypothesis_tree = new HypothesisTree(initial_hypothesis);
          InitializeComponent();
          Run();
      }

		private RegionConfig RegionConfig
		{
			get
			{
				return _regionConfig;
			}
			set
			{
				_regionConfig = value;
				
				ROI_image = RegionConfig.RoiMask.GetMask(_cameraCapture.Width, _cameraCapture.Height, new Bgr(Color.White));
			}
		}

      void Run()
      {
         try
         {

             if(VIDEO_FILE==false)
            _cameraCapture = new Capture();
             else 
             {
                 _cameraCapture = new Capture(ConfigurationSettings.AppSettings["VideoFilePath"]);
             }

             RegionConfig = RegionConfig.Load(ConfigurationSettings.AppSettings["RegionConfig"]);
         }
         catch (Exception e)
         {
            //MessageBox.Show(e.Message);
             Console.WriteLine(e.Message);
            return;
         }
         
         // TODO: Should be detaching this on TrafficCounter.Dispose() as it is a static event
         Application.Idle += ProcessFrame;
         //Application.Idle += PushStateProcess;

          //TODO: Where d othe credentials come from?
         String IntersectionImagePath = "ftp://traffic-camera.com/assets/intersection_" + ConfigurationSettings.AppSettings["IntersectionId"] + ".png";
         ServerReporter.INSTANCE.AddReportItem(
             new FtpSendFileReportItem(
                 FRAME_UPLOAD_INTERVAL_MINUTES,
                 new Uri(@IntersectionImagePath),
                  new NetworkCredential("imloader@traffic-camera.com", "traffic"),
                  GetCameraFrameBytes
                 ));
         ServerReporter.INSTANCE.Start();
      }

      private Byte[] GetCameraFrameBytes()
      {
          // TODO:  Want PNG?

          var bmp = Color_Background.ToBitmap();

          byte[] byteArray;
          using (MemoryStream stream = new MemoryStream())
          {
              bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
              stream.Close();

              byteArray = stream.ToArray();
          }

          return byteArray;
      }

      void ProcessFrame(object sender, EventArgs e)
      {
         Image<Bgr, Byte> frame = _cameraCapture.QueryFrame();

         if (frame != null)
         {
             if (Frame == null) //we need at least one frame to initialize the background
             {
                 Frame = frame;
                 Color_Background = frame.Convert<Bgr, float>();
             }
             else
             {
                 frame.Clone();
                 Frame = frame;
                 

                 int count = CountObjects();
                 if (count > max_object_count)
                     count = max_object_count;

                 UpdateBackground(frame);

                 Coordinates[] coordinates = FindBlobCenters(frame, count);

                 MHT_Update(coordinates);

             }

             renderUI(frame);
         }
      }

      void PushStateProcess(object sender, EventArgs e)
      {
        if (pushStateCheckbox.Checked)
        {
            Thread pushThread = new Thread(pushState);
            pushThread.Start();
        }
      }


      private void MHT_Update(Coordinates[] coordinates)
      {
          int num_detections = coordinates.Length;
          //Maintain hypothesis tree
          if (hypothesis_tree.children.Count > 0)
          {
                hypothesis_tree.Prune(1);
                hypothesis_tree = hypothesis_tree.GetChild(0);

              //To do: save deleted
              //hypothesis_tree.SaveDeleted(file path, length threshold);
          }

          List<Node<StateHypothesis>> childNodeList = hypothesis_tree.GetLeafNodes();
          foreach (Node<StateHypothesis> childNode in childNodeList) //For each lowest-level hypothesis node
          {
              // Update child node
              int numExistingTargets = childNode.nodeData.vehicles.Count;

              StateEstimate[] target_state_estimates = childNode.nodeData.GetStateEstimates();
              
              //Allocate matrix one column for each existing vehicle plus one column for new vehicles and one for false positives, one row for each object detection event
              if (num_detections > 0)
              {
                  //Got detections
                  DenseMatrix ambiguity_matrix;
                  DenseMatrix false_assignment_matrix = new DenseMatrix(num_detections, num_detections, Double.MinValue);
                  double[] false_assignment_diagonal = Enumerable.Repeat(Math.Log10(lambda_f), num_detections).ToArray();
                  false_assignment_matrix.SetDiagonal(false_assignment_diagonal); //Represents a false positive

                  DenseMatrix new_target_matrix = new DenseMatrix(num_detections, num_detections, Double.MinValue);
                  double[] new_target_diagonal = Enumerable.Repeat(Math.Log10(lambda_n), num_detections).ToArray();
                  new_target_matrix.SetDiagonal(new_target_diagonal); //Represents a new object to track


                  //Generate a matrix where each row signifies a detection and each column signifies an existing target
                  //The value in each cell is the probability of the row's measurement occuring for the column's object
                  ambiguity_matrix = GenerateAmbiguityMatrix(coordinates, numExistingTargets, target_state_estimates);

                  //Generating expanded hypothesis
                  //Hypothesis matrix needs to have a unique column for each detection being treated as a false positive or new object
                  DenseMatrix hypothesis_expanded;
                  if (numExistingTargets > 0)
                  {
                      //Expanded hypothesis: targets exist
                      DenseMatrix target_assignment_matrix = (DenseMatrix)ambiguity_matrix.SubMatrix(0, num_detections, 1, numExistingTargets);

                      hypothesis_expanded = new DenseMatrix(num_detections, 2 * num_detections + numExistingTargets);

                      hypothesis_expanded.SetSubMatrix(0, num_detections, 0, num_detections, false_assignment_matrix);
                      hypothesis_expanded.SetSubMatrix(0, num_detections, num_detections, numExistingTargets, target_assignment_matrix);
                      hypothesis_expanded.SetSubMatrix(0, num_detections, num_detections + numExistingTargets, num_detections, new_target_matrix);
                      
                  }
                  else
                  {
                      //Expanded hypothesis: no targets
                      hypothesis_expanded = new DenseMatrix(num_detections, 2 * num_detections);
                      hypothesis_expanded.SetSubMatrix(0, num_detections, 0, num_detections, false_assignment_matrix);
                      hypothesis_expanded.SetSubMatrix(0, num_detections, num_detections, num_detections, new_target_matrix);
                  }
                  
                  //Calculate K-best assignment using Murty's algorithm
                  double[,] costs = hypothesis_expanded.ToArray();
                  //Console.WriteLine("Finding k-best assignment");
                  for (int i = 0; i < costs.GetLength(0); i++)
                      for (int j = 0; j < costs.GetLength(1); j++)
                          costs[i, j] = -costs[i, j];

                  List<int[]> k_best = OptAssign.FindKBestAssignments(costs, k_hypotheses);

                  //Generate child hypotheses from k-best assignments
                  for (int i = 0; i < k_best.Count; i++)
                  {
                      //Console.WriteLine("Generating hypothesis {0}", i);
                      int[] assignment = k_best[i];
                      StateHypothesis child_hypothesis = new StateHypothesis(miss_threshold);
                      childNode.AddChild(child_hypothesis);
                      HypothesisTree child_hypothesis_tree = new HypothesisTree(childNode.children[i].nodeData);
                      child_hypothesis_tree.parent = childNode;

                      child_hypothesis.probability = OptAssign.assignmentCost(costs, assignment);
                      //Update states for vehicles without measurements
                      for (int j = 0; j < numExistingTargets; j++)
                      {
                          //If this target is not detected
                          if (!(assignment.Contains(j + num_detections)))
                          {
                              //Updating state for missed measurement
                              StateEstimate last_state = childNode.nodeData.vehicles[j].state_history.Last();
                              StateEstimate no_measurement_update = last_state.PropagateStateNoMeasurement(0.033, hypothesis_tree.H, hypothesis_tree.R, hypothesis_tree.F, hypothesis_tree.Q, hypothesis_tree.compensation_gain);
                              child_hypothesis_tree.UpdateVehicleFromPrevious(j, no_measurement_update, false);
                          }
                      }

                      for (int j = 0; j < num_detections; j++)
                      {

                          //Account for new vehicles
                          if (assignment[j] >= numExistingTargets + num_detections && numExistingTargets < max_targets) //Add new vehicle
                          {
                              //Creating new vehicle
                              child_hypothesis.AddVehicle(Convert.ToInt16(coordinates[j].x), Convert.ToInt16(coordinates[j].y), 0, 0);
                          }
                          else if (assignment[j] >= num_detections && assignment[j] < num_detections + numExistingTargets) //Update states for vehicles with measurements
                          {
                              //Updating vehicle with measurement
                              StateEstimate last_state = childNode.nodeData.vehicles[assignment[j] - num_detections].state_history.Last();
                              StateEstimate measurement_update = last_state.PropagateState(0.033, hypothesis_tree.H, hypothesis_tree.R, hypothesis_tree.F, hypothesis_tree.Q, coordinates[j]);
                              child_hypothesis_tree.UpdateVehicleFromPrevious(assignment[j] - num_detections, measurement_update, true);
                          }

                      }
                  }

              }
              else
              {
                StateHypothesis child_hypothesis = new StateHypothesis(miss_threshold);
                childNode.AddChild(child_hypothesis);
                HypothesisTree child_hypothesis_tree = new HypothesisTree(childNode.children[0].nodeData);
                child_hypothesis_tree.parent = childNode;

                child_hypothesis.probability = Math.Pow((1 - Pd), numExistingTargets);
                //Update states for vehicles without measurements
                for (int j = 0; j < numExistingTargets; j++)
                {
                //Updating state for missed measurement
                StateEstimate last_state = childNode.nodeData.vehicles[j].state_history.Last();
                StateEstimate no_measurement_update = last_state.PropagateStateNoMeasurement(0.033, hypothesis_tree.H, hypothesis_tree.R, hypothesis_tree.F, hypothesis_tree.Q, hypothesis_tree.compensation_gain);
                child_hypothesis_tree.UpdateVehicleFromPrevious(j, no_measurement_update, false);   
                }
              }

          }

      }

      private static void CoordinatesContains(Coordinates[] coordinates, double problem_x, double problem_y)
      {
          for (int i = 0; i < coordinates.Length; i++)
          {
              if (coordinates[i].x == problem_x && coordinates[i].y == problem_y)
                  Console.WriteLine("Multiple detections here");
          }
      }

      private DenseMatrix GenerateAmbiguityMatrix(Coordinates[] coordinates, int numExistingTargets, StateEstimate[] target_state_estimates)
      {
          DenseMatrix ambiguity_matrix;
          int num_detections = coordinates.Length;
          ambiguity_matrix = new DenseMatrix(num_detections, numExistingTargets + 2);
          Normal norm = new MathNet.Numerics.Distributions.Normal();

          for (int i = 0; i < numExistingTargets; i++)
          {
              //Get this car's estimated next position using Kalman predictor
              StateEstimate no_measurement_estimate = target_state_estimates[i].PropagateStateNoMeasurement(0.033, hypothesis_tree.H, hypothesis_tree.R, hypothesis_tree.F, hypothesis_tree.Q, hypothesis_tree.compensation_gain);

              DenseMatrix P_bar = new DenseMatrix(4, 4);
              P_bar[0, 0] = no_measurement_estimate.cov_x;
              P_bar[1, 1] = no_measurement_estimate.cov_vx;
              P_bar[2, 2] = no_measurement_estimate.cov_y;
              P_bar[3, 3] = no_measurement_estimate.cov_vy;

              DenseMatrix H_trans = (DenseMatrix) hypothesis_tree.H.Transpose();
              DenseMatrix B = hypothesis_tree.H * P_bar*H_trans + hypothesis_tree.R;
              DenseMatrix B_inverse = (DenseMatrix)B.Inverse();

              for (int j = 0; j < num_detections; j++)
              {
                  DenseMatrix z_meas = new DenseMatrix(2, 1);
                  z_meas[0, 0] = coordinates[j].x;
                  z_meas[1, 0] = coordinates[j].y;

                  DenseMatrix z_est = new DenseMatrix(2, 1);
                  z_est[0, 0] = no_measurement_estimate.coordinates.x;
                  z_est[1, 0] = no_measurement_estimate.coordinates.y;

                  DenseMatrix residual = StateEstimate.residual(z_est, z_meas);
                  DenseMatrix residual_transpose = (DenseMatrix)residual.Transpose();
                  DenseMatrix mahalanobis = residual_transpose * B_inverse * residual;
                  double mahalanobis_distance = Math.Sqrt(mahalanobis[0, 0]);

                  if (mahalanobis_distance > validation_region_deviation)
                      ambiguity_matrix[j, i + 1] = Double.MinValue;
                  else
                  {
                      ambiguity_matrix[j, i + 1] = Math.Log10(Pd * norm.Density(mahalanobis_distance) / (1 - Pd));
                  }
              }
          }
          return ambiguity_matrix;
      }

      private Coordinates[] FindBlobCenters(Image<Bgr, Byte> frame, int count)
      {
          Image<Gray, Byte> tempMovement_Mask = Movement_Mask.Clone();
          Coordinates[] coordinates = new Coordinates[count];
          for (int detection_count = 0; detection_count < count; detection_count++)
          {
              double[] minValues;
              double[] maxValues;
              System.Drawing.Point[] minLocations;
              System.Drawing.Point[] maxLocations;
              tempMovement_Mask.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
              int[] maxLocation = new int[] { maxLocations[0].X, maxLocations[0].Y };
              tempMovement_Mask.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), car_radius), new Gray(0), 0);
              frame.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), 1), new Bgr(255.0, 255.0, 255.0), 1);
              coordinates[detection_count].x = maxLocation[0];
              coordinates[detection_count].y = maxLocation[1];
          }
          return coordinates;
      }

      private void UpdateBackground(Image<Bgr, Byte> frame)
      {
          Image<Bgr, float> BackgroundUpdate = frame.Convert<Bgr, float>();
          Color_Background.RunningAvg(BackgroundUpdate, alpha);
      }

      private int CountObjects()
      {
          Color_Difference = Color_Background.AbsDiff(Frame.Convert<Bgr, float>());
          Image<Bgr, float> Masked_Difference = Color_Difference.And(ROI_image);
          Masked_Difference._ThresholdBinary(new Bgr(color_threshold, color_threshold, color_threshold), new Bgr(Color.White));
          Movement_Mask = Masked_Difference.Convert<Gray, Byte>();
          Movement_Mask._SmoothGaussian(5, 5, 1, 1);
          Image<Bgr, double> int_img = Masked_Difference.Integral();
          int lim_x = Masked_Difference.Width - 1;
          int lim_y = Masked_Difference.Height - 1;
          double raw_mass = (int_img[lim_y, lim_x].Blue + int_img[lim_y, lim_x].Red + int_img[lim_y, lim_x].Green);
          noise_mass = Convert.ToInt32(avgNoiseTextbox.Text);
          per_car = Convert.ToInt32(avgAreaTextbox.Text);
          if (per_car <= 0)
              per_car = 1000;

          int count = Convert.ToInt32((raw_mass - noise_mass) / per_car);
          if (count < 0)
              count = 0;
          if (count > max_object_count)
              count = max_object_count;

          movementMassBox.Text = raw_mass.ToString();
          detectionCountBox.Text = count.ToString();
          return count;
      }

      private void pushState()
      {
          try
          {
             
              StateEstimate[] state_estimates = hypothesis_tree.nodeData.vehicles.Select(v => v.state_history.Last()).ToArray();
              Dictionary<string, string> post_values = new Dictionary<string, string>();
              for (int vehicle_count = 0; vehicle_count < state_estimates.Length; vehicle_count++)
              {
                  String x = state_estimates[vehicle_count].coordinates.x.ToString();
                  String y = state_estimates[vehicle_count].coordinates.y.ToString();
                  String vx = state_estimates[vehicle_count].vx.ToString();
                  String vy = state_estimates[vehicle_count].vy.ToString();
                  String zero = "0";
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][x]", x);
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][vx]", vx);
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][y]", y);
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][vy]", vy);
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][_destroy]", zero);
              }
              post_values.Add("intersection_id", intersection_id);


              String post_string = "";
              foreach (KeyValuePair<string, string> post_value in post_values)
              {
                  post_string += post_value.Key + "=" + HttpUtility.UrlEncode(post_value.Value) + "&";
              }
              post_string = post_string.TrimEnd('&');

              //Upload state to server
              String post_url = "http://www.traffic-camera.com/state_samples";

              HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(post_url);
              objRequest.KeepAlive = true;
              objRequest.Pipelined = true;
              objRequest.Timeout = 2000;
              objRequest.Method = "POST";
              objRequest.ContentLength = post_string.Length;
              objRequest.ContentType = "application/x-www-form-urlencoded";

              //// post data is sent as a stream
              StreamWriter myWriter = null;
              myWriter = new StreamWriter(objRequest.GetRequestStream());
              myWriter.Write(post_string);
              myWriter.Close();
              objRequest.GetResponse();
              
          }
          catch (Exception ex)
          {
              Console.WriteLine(ex.Message);
          }
      }

      private void renderUI(Image<Bgr, Byte> frame)
      {
          Image<Bgr, float> Overlay = new Image<Bgr, float>(ROI_image.Width, ROI_image.Height, new Bgr(Color.Green));
          Overlay = Overlay.And(ROI_image);
          Overlay.Acc(Color_Background);

          
          hypothesis_tree.nodeData.vehicles.ForEach(delegate(Vehicle vehicle)
          {
              float x = (float) vehicle.state_history.Last().coordinates.x;
              float y = (float) vehicle.state_history.Last().coordinates.y;
              float radius = validation_region_deviation*((float)Math.Sqrt(Math.Pow(vehicle.state_history.Last().cov_x,2) + (float) Math.Pow(vehicle.state_history.Last().cov_y,2)));
              if (radius < 2.0)
                  radius = (float) 2.0;

              float vx_render = (float) (velocity_render_multiplier * vehicle.state_history.Last().vx);
              float vy_render = (float) (velocity_render_multiplier * vehicle.state_history.Last().vy);

              if (render_clean)
              {
                  frame.Draw(new CircleF(new PointF(x, y), 10), new Bgr(0.0, 255.0, 0.0), 2);
                  //frame.Draw(new LineSegment2D(new Point((int)x, (int)y), new Point((int)(x + vx_render), (int)(y + vy_render))), new Bgr(0.0, 0.0, 255.0), 1); //Render velocity vector
              }
              else 
              {
                //if (radius < render_radius_threshold) //For hiding vehicles whose position is uncertain
                //{
                frame.Draw(new CircleF(new PointF(x, y), radius), new Bgr(0.0, 255.0, 0.0), 1);
                frame.Draw(new LineSegment2D(new Point((int)x, (int)y), new Point((int)(x + vx_render), (int)(y + vy_render))), new Bgr(0.0, 0.0, 255.0), 1);
                //}
              }
              
          }
         );

          trackCountBox.Text = hypothesis_tree.nodeData.vehicles.Count().ToString();

          imageBox1.Image = frame;
          if (showPolygonsCheckbox.Checked)
              imageBox2.Image = Overlay;
          else
            imageBox2.Image = Color_Background;
          
          imageBox3.Image = Movement_Mask;
          
      }


      private void imageBox1_MouseDown(object sender, MouseEventArgs e)
      {
          int offsetX = (int)(e.Location.X / imageBox1.ZoomScale);
          int offsetY = (int)(e.Location.Y / imageBox1.ZoomScale);
          int horizontalScrollBarValue = imageBox1.HorizontalScrollBar.Visible ? (int)imageBox1.HorizontalScrollBar.Value : 0;
          int verticalScrollBarValue = imageBox1.VerticalScrollBar.Visible ? (int)imageBox1.VerticalScrollBar.Value : 0;
      }

      private void btnConfigureRegions_Click(object sender, EventArgs e)
      {
          RegionEditor r = new RegionEditor(Color_Background, RegionConfig);
          if (r.ShowDialog() == System.Windows.Forms.DialogResult.OK)
          {
              RegionConfig = r.GetRegionConfig();
              RegionConfig.Save(ConfigurationSettings.AppSettings["RegionConfig"]);
          }
      }

      private void TrafficCounter_FormClosed(object sender, FormClosedEventArgs e)
      {
          Application.Idle -= ProcessFrame;
      }
   }
}