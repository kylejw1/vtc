namespace VTC.Common
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

        #region Video stream parameters
        double FrameWidth { get; }
        double FrameHeight { get; }

        /// <summary>
        /// Vertical and horizontal resolution of velocity field
        /// </summary>
        int VelocityFieldResolution { get; }

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

        /// <summary>
        /// Downsampling ratio for MoG background sampling
        /// </summary>
        int MoGUpdateDownsampling { get; }
        #endregion


        #region Object detection parameters
        /// <summary>
        /// Maximum number of blobs to detect.
        /// </summary>
        int MaxObjectCount { get; }

        /// <summary>
        /// Radius of car image in pixels.
        /// </summary>
        int CarRadius { get; }        
        

        #endregion
    }
}