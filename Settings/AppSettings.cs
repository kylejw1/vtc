using System;
using System.Configuration;
using System.Globalization;

namespace VTC.Settings
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public sealed class AppSettings : ISettings
    {
        #region Constants

        private const string PerCarKey = "PerCar";
        private const string PerCarMinKey = "PerCarMin";
        private const string IntersectionIdKey = "IntersectionID";

        #endregion

        private readonly Configuration _config;

        public string FtpUserName { get; private set; }
        public string FtpPassword { get; private set; }
        public short FrameUploadIntervalMinutes { get; private set; }
        public int StateUploadIntervalMs { get; private set; }

        public string IntersectionID { get; set; }
        public string ServerUrl { get; private set; }

        public double Alpha { get; private set; }

        public int ColorThreshold { get; private set; }

        public double NoiseMass { get; private set; }
        public double PerCar { get; set; }
        public int MaxObjectCount { get; private set; }
        public double PerCarMinimum { get; set; }
        public int CarRadius { get; private set; }

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

            Alpha = Convert.ToDouble(ConfigurationManager.AppSettings["Alpha"]);
            ColorThreshold = Convert.ToInt32(ConfigurationManager.AppSettings["ColorThreshold"]);

            CarRadius = Convert.ToInt32(ConfigurationManager.AppSettings["CarRadius"]);
            PerCarMinimum = Convert.ToDouble(ConfigurationManager.AppSettings[PerCarMinKey]);
            MaxObjectCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxObjCount"]);
            PerCar = Convert.ToDouble(ConfigurationManager.AppSettings[PerCarKey]);
            NoiseMass = Convert.ToDouble(ConfigurationManager.AppSettings["NoiseMass"]);

            MissThreshold = Convert.ToInt32(ConfigurationManager.AppSettings["MissThreshold"]);
            MaxHypothesisTreeDepth = Convert.ToInt32(ConfigurationManager.AppSettings["MaxHypTreeDepth"]);
            MaxTargets = Convert.ToInt32(ConfigurationManager.AppSettings["MaxTargets"]);
            KHypotheses = Convert.ToInt32(ConfigurationManager.AppSettings["KHypotheses"]);
            ValidationRegionDeviation = Convert.ToInt32(ConfigurationManager.AppSettings["ValRegDeviation"]);

            LambdaX = Convert.ToDouble(ConfigurationManager.AppSettings["LambdaX"]);
            LambdaF = Convert.ToDouble(ConfigurationManager.AppSettings["LambdaF"]);
            LambdaN = Convert.ToDouble(ConfigurationManager.AppSettings["LambdaN"]);
            Pd = Convert.ToDouble(ConfigurationManager.AppSettings["Pd"]);
            Px = Convert.ToDouble(ConfigurationManager.AppSettings["Px"]);

            ServerUrl = ConfigurationManager.AppSettings["server_url"];
            IntersectionID = ConfigurationManager.AppSettings[IntersectionIdKey];

            RegionConfigPath = ConfigurationManager.AppSettings["RegionConfig"];

            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // some unused settings from TrafficCounter. Keeping it just in case.
            double pruning_ratio = Convert.ToDouble(ConfigurationManager.AppSettings["PruningRatio"]);    //Probability ratio at which hypotheses are pruned
            double q = Convert.ToDouble(ConfigurationManager.AppSettings["Q"]);                           //Process noise matrix multiplier
            double r = Convert.ToDouble(ConfigurationManager.AppSettings["R"]);                           //Measurement noise matrix multiplier
        }

        /// <summary>
        /// Save changed parameters.
        /// </summary>
        public void Save() // only several parameters can be saved
        {
            _config.AppSettings.Settings[PerCarKey].Value = PerCar.ToString(CultureInfo.InvariantCulture);
            _config.AppSettings.Settings[PerCarMinKey].Value = PerCarMinimum.ToString(CultureInfo.InvariantCulture);
            _config.AppSettings.Settings[IntersectionIdKey].Value = IntersectionID;

            _config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
