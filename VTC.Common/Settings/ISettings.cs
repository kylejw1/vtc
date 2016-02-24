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

        /// <summary>
        /// Radius of car image in pixels.
        /// </summary>
        int MinObjectSize { get; }

        /// <summary>
        /// Background movement noise.
        /// </summary>
        double Timestep { get; }

        /// <summary>
        /// Process noise matrix multipler - position
        /// </summary>
        double Q_position { get; }

        /// <summary>
        /// Process noise matrix multipler - color
        /// </summary>
        double Q_color { get; }

        /// <summary>
        /// Measurement noise matrix multipler - position
        /// </summary>
        double R_position { get; }

        /// <summary>
        /// Measurement noise matrix multipler - color
        /// </summary>
        double R_color { get; }

        /// <summary>
        /// Initial X Covariance for vehicles
        /// </summary>
        double VehicleInitialCovX { get; }

        /// <summary>
        /// Initial X Covariance for vehicles
        /// </summary>
        double VehicleInitialCovY { get; }

        /// <summary>
        /// Initial X Covariance for vehicles
        /// </summary>
        double VehicleInitialCovVX { get; }

        /// <summary>
        /// Initial X Covariance for vehicles
        /// </summary>
        double VehicleInitialCovVY { get; }

        /// <summary>
        /// Initial X Covariance for vehicles
        /// </summary>
        double VehicleInitialCovR { get; }

        /// <summary>
        /// Initial X Covariance for vehicles
        /// </summary>
        double VehicleInitialCovG { get; }

        /// <summary>
        /// Initial X Covariance for vehicles
        /// </summary>
        double VehicleInitialCovB { get; }

        

        #endregion
    }
}