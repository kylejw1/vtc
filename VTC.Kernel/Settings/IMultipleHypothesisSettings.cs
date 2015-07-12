namespace VTC.Kernel.Settings
{
    public interface IMultipleHypothesisSettings
    {

        /// <summary>
        /// Minimum object size for blob detection. 
        /// </summary>
        int MinObjectSize { get; }

        /// <summary>
        /// Number of misses to consider an object gone.
        /// </summary>
        int MissThreshold { get; }

        /// <summary>
        /// Maximum allowed hypothesis tree depth.
        /// </summary>
        int MaxHypothesisTreeDepth { get; }

        /// <summary>
        /// Maximum number of concurrently tracked targets.
        /// </summary>
        int MaxTargets { get; }

        /// <summary>
        /// Branching factor for hypothesis tree.
        /// </summary>
        int KHypotheses { get; }

        /// <summary>
        /// Mahalanobis distance multiplier used in measurement gating.
        /// </summary>
        int ValidationRegionDeviation { get; }

        /// <summary>
        /// Termination likelihood.
        /// </summary>
        double LambdaX { get; }

        /// <summary>
        /// Density of Poisson-distributed false positives.
        /// </summary>
        double LambdaF { get; }

        /// <summary>
        /// Density of Poission-distributed new vehicles.
        /// </summary>
        double LambdaN { get; }

        /// <summary>
        /// Probability of object detection.
        /// </summary>
        double Pd { get; }

        /// <summary>
        /// Probability of track termination.
        /// </summary>
        double Px { get; }

        /// <summary>
        /// Timestep between frames in seconds.
        /// </summary>
        double Timestep { get; }

        /// <summary>
        /// Process noise matrix multiplier - position
        /// </summary>
        double Q_position { get; }

        /// <summary>
        /// Process noise matrix multiplier - color
        /// </summary>
        double Q_color { get; }

        /// <summary>
        /// Measurement noise matrix multiplier for position
        /// </summary>
        double R_position { get; }

        /// <summary>
        /// Measurement noise matrix multiplier for color
        /// </summary>
        double R_color { get; }

        /// <summary>
        /// Covariance gain when a measurement is missed
        /// </summary>
        double CompensationGain { get; }


        /// <summary>
        /// Initial X Covariance for vehicles
        /// </summary>
        double VehicleInitialCovX { get; }

        /// <summary>
        /// Initial Y Covariance for vehicles
        /// </summary>
        double VehicleInitialCovY { get; }

        /// <summary>
        /// Initial VX Covariance for vehicles
        /// </summary>
        double VehicleInitialCovVX { get; }

        /// <summary>
        /// Initial VY Covariance for vehicles
        /// </summary>
        double VehicleInitialCovVY { get; }

        /// <summary>
        /// Initial R Covariance for vehicles
        /// </summary>
        double VehicleInitialCovR { get; }

        /// <summary>
        /// Initial G Covariance for vehicles
        /// </summary>
        double VehicleInitialCovG { get; }

        /// <summary>
        /// Initial B Covariance for vehicles
        /// </summary>
        double VehicleInitialCovB { get; }

      
    }
}