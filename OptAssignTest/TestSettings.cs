using System;
using VTC.Settings;

namespace OptAssignTest
{
    /// <summary>
    /// Special set of settings for unit tests.
    /// </summary>
    class TestSettings : ISettings
    {
        public int MissThreshold
        {
            get { return 30; }
        }

        public int MaxHypothesisTreeDepth
        {
            get { return 2; }
        }

        public int MaxTargets
        {
            get { return 10; }
        }

        public int KHypotheses
        {
            get { return 2; }
        }

        public int ValidationRegionDeviation
        {
            get { return 7; }
        }

        public double LambdaX
        {
            get { throw new NotImplementedException(); }
        }

        public double LambdaF
        {
            get { return 0.0000004; }
        }

        public double LambdaN
        {
            get { return 0.0000006; }
        }

        public double Pd
        {
            get { return 0.8; }
        }

        public double Px
        {
            get { throw new NotImplementedException(); }
        }

        public string FtpUserName
        {
            get { throw new NotImplementedException(); }
        }

        public string FtpPassword
        {
            get { throw new NotImplementedException(); }
        }

        public short FrameUploadIntervalMinutes
        {
            get { throw new NotImplementedException(); }
        }

        public int StateUploadIntervalMs
        {
            get { throw new NotImplementedException(); }
        }

        public string IntersectionID
        {
            get { throw new NotImplementedException(); }
        }

        public string ServerUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string RegionConfigPath
        {
            get { return "config\regionConfig.xml"; }
        }

        public double Alpha
        {
            get { return 0.001; }
        }

        public int ColorThreshold
        {
            get { return 40; }
        }

        public double NoiseMass
        {
            get { return 0.0; }
        }

        public int MaxObjectCount
        {
            get { return 20; }
        }

        public double PerCarMinimum
        {
            get { return 1000; }
        }

        public int CarRadius
        {
            get { return _carRadius; }
            set { _carRadius = value; }
        }
        private int _carRadius = 12;

        public double FrameWidth
        {
            get { return 200; }
        }

        public double FrameHeight
        {
            get { return 200; }
        }
    }
}