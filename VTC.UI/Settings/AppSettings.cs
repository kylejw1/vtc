using System;
using System.Configuration;
using System.Globalization;
using VTC.Kernel.Settings;
using System.Linq;

namespace VTC.Settings
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public sealed class AppSettings : ISettings
    {
        #region Constants

        private const string PerCarMinKey = "PerCarMin";
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

        public double NoiseMass { get; private set; }
        public int MaxObjectCount { get; private set; }
        public double PerCarMinimum { get; set; }
        public int CarRadius { get; set; }


        public int MissThreshold { get; private set; }
        public int MaxHypothesisTreeDepth { get; private set; }
        public int MaxTargets { get; private set; }
        public int KHypotheses { get; private set; }
        public int ValidationRegionDeviation { get; private set; }
        public double LambdaX { get; private set; }
        public double LambdaF { get; private set; }
        public double LambdaN { get; private set; }
        public double Pd { get; private set; }
        public double Px { get; private set; }

        public int ClassifierSubframeWidth { get; private set; }
        public int ClassifierSubframeHeight { get; private set; }

        public String[] Classes { get; private set; }

        public string RegionConfigPath { get; private set; }

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

            CarRadius = Convert.ToInt32(ConfigurationManager.AppSettings[CarRadiusKey]);
            PerCarMinimum = Convert.ToDouble(ConfigurationManager.AppSettings[PerCarMinKey], CultureInfo.InvariantCulture);
            MaxObjectCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxObjCount"]);
            NoiseMass = Convert.ToDouble(ConfigurationManager.AppSettings["NoiseMass"], CultureInfo.InvariantCulture);

            MissThreshold = Convert.ToInt32(ConfigurationManager.AppSettings["MissThreshold"]);
            MaxHypothesisTreeDepth = Convert.ToInt32(ConfigurationManager.AppSettings["MaxHypTreeDepth"]);
            MaxTargets = Convert.ToInt32(ConfigurationManager.AppSettings["MaxTargets"]);
            KHypotheses = Convert.ToInt32(ConfigurationManager.AppSettings["KHypotheses"]);
            ValidationRegionDeviation = Convert.ToInt32(ConfigurationManager.AppSettings["ValRegDeviation"]);

            LambdaX = Convert.ToDouble(ConfigurationManager.AppSettings["LambdaX"], CultureInfo.InvariantCulture);
            LambdaF = Convert.ToDouble(ConfigurationManager.AppSettings["LambdaF"], CultureInfo.InvariantCulture);
            LambdaN = Convert.ToDouble(ConfigurationManager.AppSettings["LambdaN"], CultureInfo.InvariantCulture);
            Pd = Convert.ToDouble(ConfigurationManager.AppSettings["Pd"], CultureInfo.InvariantCulture);
            Px = Convert.ToDouble(ConfigurationManager.AppSettings["Px"], CultureInfo.InvariantCulture);

            ServerUrl = ConfigurationManager.AppSettings["server_url"];
            IntersectionID = ConfigurationManager.AppSettings[IntersectionIdKey];

            String[] classKeyStrings = ConfigurationManager.AppSettings.AllKeys;
            Classes = classKeyStrings.Where(this_key => this_key.Contains("class")).Select(element => element.Substring(6,element.Length-6)).ToArray();
            ClassifierSubframeWidth = Convert.ToInt16(ConfigurationManager.AppSettings["ClassifierSubframeWidth"]);
            ClassifierSubframeHeight = Convert.ToInt16(ConfigurationManager.AppSettings["ClassifierSubframeHeight"]);
            

            RegionConfigPath = ConfigurationManager.AppSettings["RegionConfig"];

            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // some unused settings from TrafficCounter. Keeping it just in case.
            double pruning_ratio = Convert.ToDouble(ConfigurationManager.AppSettings["PruningRatio"]);    //Probability ratio at which hypotheses are pruned
            double q = Convert.ToDouble(ConfigurationManager.AppSettings["Q"], CultureInfo.InvariantCulture);                           //Process noise matrix multiplier
            double r = Convert.ToDouble(ConfigurationManager.AppSettings["R"], CultureInfo.InvariantCulture);                           //Measurement noise matrix multiplier
        }

        /// <summary>
        /// Save changed parameters.
        /// </summary>
        public void Save() // only several parameters can be saved
        {
            _config.AppSettings.Settings[CarRadiusKey].Value = CarRadius.ToString(CultureInfo.InvariantCulture);
            _config.AppSettings.Settings[PerCarMinKey].Value = PerCarMinimum.ToString(CultureInfo.InvariantCulture);
            _config.AppSettings.Settings[IntersectionIdKey].Value = IntersectionID;
            _config.AppSettings.Settings[FrameWidthKey].Value = FrameWidth.ToString(CultureInfo.InvariantCulture);
            _config.AppSettings.Settings[FrameHeightKey].Value = FrameHeight.ToString(CultureInfo.InvariantCulture);
            

            _config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
