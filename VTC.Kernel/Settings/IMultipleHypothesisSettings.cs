namespace VTC.Kernel.Settings
{
    public interface IMultipleHypothesisSettings
    {
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
    }
}