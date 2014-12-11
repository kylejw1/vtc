namespace VTC.Settings
{
    public interface ISettings : IMultipleHypothesisSettings
    {
        #region FTP-related stuff

        string FtpUserName { get; }
        string FtpPassword { get; }
        short FrameUploadIntervalMinutes { get; }
        int StateUploadIntervalMs { get; }
        string IntersectionID { get; }
        string ServerUrl { get; }

        #endregion

        /// <summary>
        /// Pathname of region config file.
        /// </summary>
        string RegionConfigPath { get; }

        #region Video stream parameters
        double FrameWidth { get; }
        double FrameHeight { get; }

        #endregion

        #region Background subtraction parameters

        /// <summary>
        /// Stores alpha for thread access.
        /// </summary>
        double Alpha { get; }

        /// <summary>
        /// Threshold below which frame-movement is ignored.
        /// </summary>
        int ColorThreshold { get; }

        #endregion


        #region Object detection parameters

        /// <summary>
        /// Background movement noise.
        /// </summary>
        double NoiseMass { get; }

        /// <summary>
        /// White pixels per car in image.
        /// </summary>
        double PerCar { get; }

        /// <summary>
        /// Maximum number of blobs to detect.
        /// </summary>
        int MaxObjectCount { get; }

        /// <summary>
        /// Minimum number of white pixels per car - handles case when 0 is entered in avg-per-car textbox.
        /// </summary>
        double PerCarMinimum { get; }

        /// <summary>
        /// Radius of car image in pixels.
        /// </summary>
        int CarRadius { get; }

        #endregion

    }
}