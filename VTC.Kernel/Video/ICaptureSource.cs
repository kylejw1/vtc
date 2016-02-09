﻿using Emgu.CV;
using Emgu.CV.Structure;
using VTC.Common;

namespace VTC.Kernel.Video
{
    /// <summary>
    /// Interface for video stream.
    /// </summary>
    public interface ICaptureSource
    {
        /// <summary>
        /// Camera name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Camera width.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Camera height.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Get next frame from camera.
        /// </summary>
        /// <returns></returns>
        Image<Bgr, byte> QueryFrame();

        /// <summary>
        /// Initialize camera.
        /// </summary>
        /// <param name="settings"></param>
        void Init(ISettings settings);

        /// <summary>
        /// Destroy underlying camera.
        /// </summary>
        void Destroy();
    }
}