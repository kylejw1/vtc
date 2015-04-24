using VTC.Kernel.Settings;

namespace VTC.Kernel.Video
{
    /// <summary>
    /// Holder of video source and corresponding settings.
    /// </summary>
    public class CaptureContext
    {
        public ICaptureSource Capture { get; private set; }
        public ISettings Settings { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="capture"></param>
        /// <param name="settings"></param>
        public CaptureContext(ICaptureSource capture, ISettings settings)
        {
            Capture = capture;
            Settings = settings;
        }
    }
}
