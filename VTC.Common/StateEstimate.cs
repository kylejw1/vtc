using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace VTC.Common
{
    /// <summary>
    /// Holds 2D position and velocity estimates for Kalman filtering
    /// </summary>
    public class StateEstimate
    {
        //public Measurement measurements;
        public double X;
        public double Y;
        public double CovX;          //Location covariances
        public double CovY;

        public double Vx;             //Velocity estimates
        public double Vy;
        public double CovVx;         //Velocity covariances
        public double CovVy;

        public double Red;
        public double Green;
        public double Blue;

        public double CovRed;
        public double CovGreen;
        public double CovBlue;

        public double PathLength;    //Total path length travelled so far
        public Turn Turn;              //enum Turn to indicate turn decision (left, right or straight)

        public bool IsPedestrian;    //Binary flag set if object is likely to be a pedestrian

        public int MissedDetections; //Total number of times this object has not been detected during its lifetime

        public StateEstimate PropagateStateNoMeasurement(double timestep, DenseMatrix H, DenseMatrix R, DenseMatrix F, DenseMatrix Q, double compensationGain)
        {
            var updatedState = new StateEstimate
            {
                Turn = Turn,
                IsPedestrian = IsPedestrian,
                MissedDetections = MissedDetections + 1,
                PathLength =
                    PathLength + Math.Sqrt(Math.Pow((timestep * Vx), 2) + Math.Pow((timestep * Vy), 2))
            };

            var zEst = new DenseMatrix(7, 1); //4-Row state vector: x, vx, y, vy
            zEst[0, 0] = X;
            zEst[1, 0] = Vx;
            zEst[2, 0] = Y;
            zEst[3, 0] = Vy;
            zEst[4, 0] = Red;
            zEst[5, 0] = Green;
            zEst[6, 0] = Blue;

            var pBar = new DenseMatrix(7, 7);
            pBar[0, 0] = CovX;
            pBar[1, 1] = CovVx;
            pBar[2, 2] = CovY;
            pBar[3, 3] = CovVy;
            pBar[4, 4] = CovRed;
            pBar[5, 5] = CovGreen;
            pBar[6, 6] = CovBlue;

            //DenseMatrix B = H * P_bar * H;
            var zNext = F * zEst;
            var fTranspose = (DenseMatrix)F.Transpose();
            var pNext = (F * pBar * fTranspose) + compensationGain * Q;

            //Move values from matrix form into object properties
            updatedState.X = zNext[0, 0];
            updatedState.Y = zNext[2, 0];
            updatedState.Vx = zNext[1, 0];
            updatedState.Vy = zNext[3, 0];
            updatedState.Red = zNext[4, 0];
            updatedState.Green = zNext[5, 0];
            updatedState.Blue = zNext[6, 0];

            updatedState.CovX = pNext[0, 0];
            updatedState.CovVx = pNext[1, 1];
            updatedState.CovY = pNext[2, 2];
            updatedState.CovVy = pNext[3, 3];
            updatedState.CovRed = pNext[4, 4];
            updatedState.CovGreen = pNext[5, 5];
            updatedState.CovBlue = pNext[6, 6];

            return updatedState;
        }

        public StateEstimate PropagateState(double timestep, DenseMatrix H, DenseMatrix R, DenseMatrix F, DenseMatrix Q, Measurement measurements)
        {
            var updatedState = new StateEstimate
            {
                Turn = Turn,
                PathLength =
                    PathLength + Math.Sqrt(Math.Pow((timestep * Vx), 2) + Math.Pow((timestep * Vy), 2))
            };

            var zEst = new DenseMatrix(7, 1); //7-Row state vector: x, vx, y, vy, r, g, b
            zEst[0, 0] = X;
            zEst[1, 0] = Vx;
            zEst[2, 0] = Y;
            zEst[3, 0] = Vy;
            zEst[4, 0] = Red;
            zEst[5, 0] = Green;
            zEst[6, 0] = Blue;

            var zMeas = new DenseMatrix(5, 1); //5-Row measurement vector: x,y,r,g,b
            zMeas[0, 0] = measurements.X;
            zMeas[1, 0] = measurements.Y;
            zMeas[2, 0] = measurements.Red;
            zMeas[3, 0] = measurements.Green;
            zMeas[4, 0] = measurements.Blue;

            var pBar = new DenseMatrix(7, 7);
            pBar[0, 0] = CovX;
            pBar[1, 1] = CovVx;
            pBar[2, 2] = CovY;
            pBar[3, 3] = CovVy;
            pBar[4, 4] = CovRed;
            pBar[5, 5] = CovGreen;
            pBar[6, 6] = CovBlue;

            //DenseMatrix B = H * P_bar * H;
            var zNext = F * zEst;
            var fTranspose = (DenseMatrix)F.Transpose();
            var pNext = (F * pBar * fTranspose) + Q;
            var yResidual = zMeas - H * zNext;
            var hTranspose = (DenseMatrix)H.Transpose();
            var s = H * pNext * hTranspose + R;
            var sInv = (DenseMatrix)s.Inverse();
            var k = pNext * hTranspose * sInv;
            var zPost = zNext + k * yResidual;
            var pPost = (DenseMatrix.Identity(7) - k * H) * pNext;

            //Move values from matrix form into object properties
            updatedState.CovX = pPost[0, 0];
            updatedState.CovVx = pPost[1, 1];
            updatedState.CovY = pPost[2, 2];
            updatedState.CovVy = pPost[3, 3];
            updatedState.CovRed = pPost[4, 4];
            updatedState.CovGreen = pPost[5, 5];
            updatedState.CovBlue = pPost[6, 6];

            updatedState.X = zPost[0, 0];
            updatedState.Vx = zPost[1, 0];
            updatedState.Y = zPost[2, 0];
            updatedState.Vy = zPost[3, 0];
            updatedState.Red = zPost[4, 0];
            updatedState.Green = zPost[5, 0];
            updatedState.Blue = zPost[6, 0];

            return updatedState;
        }

        public static DenseMatrix Residual(DenseMatrix zEst, DenseMatrix zMeas)
        {
            var residual = zMeas - zEst;
            return residual;
        }

    }




}
