using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using VTC.Common;

namespace VTC.Common
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public sealed class AppSettings : ISettings
    {
        #region Constants

        private const string IntersectionIdKey = "IntersectionID";
        private const string FrameWidthKey = "FrameWidth";
        private const string FrameHeightKey = "FrameHeight";
        private const string CarRadiusKey = "CarRadius";

        #endregion

        private readonly Configuration _config;

        public string FtpUserName { get; private set; }
        public string FtpPassword { get; private set; }
        public short FrameUploadIntervalMinutes { get; private set; }
        public int StateUploadIntervalMs { get; private set; }

        public double FrameWidth { get; set; }
        public double FrameHeight { get; set; }

        public string IntersectionID { get; set; }
        public string ServerUrl { get; set; }

        public double Alpha { get; private set; }
        public int ColorThreshold { get; private set; }
        public int MoGUpdateDownsampling { get; private set; }

        public int MaxObjectCount { get; private set; }
        public int CarRadius { get; set; }
        public int MinObjectSize { get; set; }
        public double Timestep { get; set; }

        public int MissThreshold { get; private set; }
        public int MaxHypothesisTreeDepth { get; private set; }
        public int MaxTargets { get; private set; }
        public int KHypotheses { get; private set; }
        public int ValidationRegionDeviation { get; private set; }
        public double LambdaF { get; private set; }
        public double LambdaN { get; private set; }
        public double Pd { get; private set; }
        public double Px { get; private set; }

        public double Q_position { get; private set; }  
        public double Q_color { get; private set; }   
        public double R_position { get; private set; }   
        public double R_color { get; private set; }   

        public double VehicleInitialCovX { get; private set; } 
        public double VehicleInitialCovY { get; private set; }
        public double VehicleInitialCovVX { get; private set; }
        public double VehicleInitialCovVY { get; private set; }
        public double VehicleInitialCovR { get; private set; }
        public double VehicleInitialCovG { get; private set; }
        public double VehicleInitialCovB { get; private set; }

        public double CompensationGain { get; private set; }   

        public int ClassifierSubframeWidth { get; private set; }
        public int ClassifierSubframeHeight { get; private set; }
        public int VelocityFieldResolution { get; private set; }
        public String[] Classes { get; private set; }

        public string RegionConfigPath { get; private set; }

        /// <summary>
        /// Assembly with unit tests to visualize.
        /// </summary>
        public string UnitTestsDll
        {
            get { return ConfigurationManager.AppSettings["UnitTestDll"]; }
        }

        /// <summary>
        /// Filepath of the configuration file.
        /// </summary>
        public string FilePath
        {
            get { return _config.FilePath; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AppSettings()
        {
            FtpUserName = ConfigurationManager.AppSettings["FTPusername"];
            FtpPassword = ConfigurationManager.AppSettings["FTPpassword"];
            FrameUploadIntervalMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["FRAME_UPLOAD_INTERVAL_MINUTES"]);
            StateUploadIntervalMs = Convert.ToInt32(ConfigurationManager.AppSettings["state_upload_interval_ms"]);

            FrameWidth = Convert.ToDouble(ConfigurationManager.AppSettings["FrameWidth"], CultureInfo.InvariantCulture);
            FrameHeight = Convert.ToDouble(ConfigurationManager.AppSettings["FrameHeight"], CultureInfo.InvariantCulture);

            Alpha = Convert.ToDouble(ConfigurationManager.AppSettings["Alpha"], CultureInfo.InvariantCulture);
            ColorThreshold = Convert.ToInt32(ConfigurationManager.AppSettings["ColorThreshold"], CultureInfo.InvariantCulture);
            MoGUpdateDownsampling = Convert.ToInt16(ConfigurationManager.AppSettings["MoGUpdateDownsampling"]);

            CarRadius = Convert.ToInt32(ConfigurationManager.AppSettings[CarRadiusKey]);
            MinObjectSize = Convert.ToInt32(ConfigurationManager.AppSettings["MinObjectSize"]);
            Timestep = Convert.ToDouble(ConfigurationManager.AppSettings["Timestep"], CultureInfo.InvariantCulture);

            MissThreshold = Convert.ToInt32(ConfigurationManager.AppSettings["MissThreshold"]);
            MaxHypothesisTreeDepth = Convert.ToInt32(ConfigurationManager.AppSettings["MaxHypTreeDepth"]);
            MaxTargets = Convert.ToInt32(ConfigurationManager.AppSettings["MaxTargets"]);
            KHypotheses = Convert.ToInt32(ConfigurationManager.AppSettings["KHypotheses"]);
            ValidationRegionDeviation = Convert.ToInt32(ConfigurationManager.AppSettings["ValRegDeviation"]);

            LambdaF = Convert.ToDouble(ConfigurationManager.AppSettings["LambdaF"], CultureInfo.InvariantCulture);
            LambdaN = Convert.ToDouble(ConfigurationManager.AppSettings["LambdaN"], CultureInfo.InvariantCulture);
            Pd = Convert.ToDouble(ConfigurationManager.AppSettings["Pd"], CultureInfo.InvariantCulture);
            Px = Convert.ToDouble(ConfigurationManager.AppSettings["Px"], CultureInfo.InvariantCulture);

            ServerUrl = ConfigurationManager.AppSettings["server_url"];
            IntersectionID = ConfigurationManager.AppSettings[IntersectionIdKey];

            String[] classKeyStrings = ConfigurationManager.AppSettings.AllKeys;
            Classes = classKeyStrings.Where(thisKey => thisKey.Contains("class")).Select(element => element.Substring(6,element.Length-6)).ToArray();
            ClassifierSubframeWidth = Convert.ToInt16(ConfigurationManager.AppSettings["ClassifierSubframeWidth"]);
            ClassifierSubframeHeight = Convert.ToInt16(ConfigurationManager.AppSettings["ClassifierSubframeHeight"]);

            VelocityFieldResolution = Convert.ToInt16(ConfigurationManager.AppSettings["VelocityFieldResolution"]);

            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            Q_position = Convert.ToDouble(ConfigurationManager.AppSettings["Q_position"], CultureInfo.InvariantCulture);
            Q_color = Convert.ToDouble(ConfigurationManager.AppSettings["Q_color"], CultureInfo.InvariantCulture);
            R_position = Convert.ToDouble(ConfigurationManager.AppSettings["R_position"], CultureInfo.InvariantCulture);
            R_color = Convert.ToDouble(ConfigurationManager.AppSettings["R_color"], CultureInfo.InvariantCulture);

            VehicleInitialCovX = Convert.ToDouble(ConfigurationManager.AppSettings["VehicleInitialCovX"]);
            VehicleInitialCovY = Convert.ToDouble(ConfigurationManager.AppSettings["VehicleInitialCovY"]);
            VehicleInitialCovVX = Convert.ToDouble(ConfigurationManager.AppSettings["VehicleInitialCovVX"]);
            VehicleInitialCovVY = Convert.ToDouble(ConfigurationManager.AppSettings["VehicleInitialCovVY"]);
            VehicleInitialCovR = Convert.ToDouble(ConfigurationManager.AppSettings["VehicleInitialCovR"]);
            VehicleInitialCovG = Convert.ToDouble(ConfigurationManager.AppSettings["VehicleInitialCovG"]);
            VehicleInitialCovB = Convert.ToDouble(ConfigurationManager.AppSettings["VehicleInitialCovB"]);

            CompensationGain = Convert.ToDouble(ConfigurationManager.AppSettings["CompensationGain"], CultureInfo.InvariantCulture);        
        }

        /// <summary>
        /// Save changed parameters.
        /// </summary>
        public void Save() // only several parameters can be saved
        {
            _config.AppSettings.Settings[CarRadiusKey].Value = CarRadius.ToString(CultureInfo.InvariantCulture);
            _config.AppSettings.Settings[IntersectionIdKey].Value = IntersectionID;
            _config.AppSettings.Settings[FrameWidthKey].Value = FrameWidth.ToString(CultureInfo.InvariantCulture);
            _config.AppSettings.Settings[FrameHeightKey].Value = FrameHeight.ToString(CultureInfo.InvariantCulture);

            _config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
