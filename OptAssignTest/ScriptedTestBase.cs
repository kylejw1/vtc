using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using VTC;
using VTC.Settings;

namespace OptAssignTest
{
    /// <summary>
    /// Base class for scripted tests.
    /// </summary>
    public class ScriptedTestBase : TestBase
    {
        /// <summary>
        /// Frames to skip before validation.
        /// </summary>
        protected const int DetectionThreshold = 10;

        /// <summary>
        /// Execute script against the test action.
        /// </summary>
        protected static void RunScript(ISettings settings, Script script, Action<Vista, uint> testAction)
        {
            var vista = CreateVista(settings);

            // initialize background
            var background = new Image<Bgr, byte>((int) settings.FrameWidth, (int) settings.FrameHeight, new Bgr(Color.Black));
            vista.Update(background);

            // run the script
            for (uint frame = 0; ! script.IsDone(frame); frame++)
            {
                var image = background.Clone();

                script.Draw(frame, image);
                vista.Update(image);

                // run validation for the current frame
                testAction(vista, frame);
            }
        }
    }
}