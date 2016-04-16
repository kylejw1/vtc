using System;
using VTC.Common;

namespace OptAssignTest
{
    /// <summary>
    /// Special set of settings for unit tests.
    /// </summary>
    class TestSettings : ISettings
    {
        public int MissThreshold
        {
            get { return 60; }
        }

        public int MaxHypothesisTreeDepth
        {
            get { return 4; }
        }

        public int MaxTargets
        {
            get { return 10; }
        }

        public int MinObjectSize
        {
            get { return 5; }
        }

        public int MoGUpdateDownsampling
        {
            get { return 50; }
        }

        public int VelocityFieldResolution
        {
            get { return 50; }
        }

        public int KHypotheses
        {
            get { return 4; }
        }

        public int ValidationRegionDeviation
        {
            get { return 3; }
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

        public int MaxObjectCount
        {
            get { return 20; }
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

        public double CompensationGain
        {
            get { return 30; }
        }

        public double Q_color
        {
            get { return 50000; }
        }

        public double Q_position
        {
            get { return 5; }
        }

        public double R_color
        {
            get { return 50000; }
        }

        public double R_position
        {
            get { return 5; }
        }

        public double Timestep
        {
            get { return 0.033; }
        }


        public double VehicleInitialCovX
        {
            get { return 2; }
        }

        public double VehicleInitialCovVX
        {
            get { return 300; }
        }

        public double VehicleInitialCovY
        {
            get { return 2; }
        }

        public double VehicleInitialCovVY
        {
            get { return 300; }
        }

        public double VehicleInitialCovR
        {
            get { return 50; }
        }

        public double VehicleInitialCovG
        {
            get { return 50; }
        }

        public double VehicleInitialCovB
        {
            get { return 50; }
        }
    }
}