vtc
===
Video Traffic Counter

---

Live: [www.traffic-camera.com](http://www.traffic-camera.com) Development: [dev.traffic-camera.com](http://dev.traffic-camera.com)

Realtime video-based object tracking software. Written in C# using Emgu CV. Requires a webcam.

#Development environment setup

Make sure you have NuGet extension (applicable for VS2010).

##EmguCV
* Copy EmguCV x64 and x86 directories (emgucv-windows-universal-cuda 2.9.0.1922\bin\x64, x86) to the Visual Studio solution directory for VTC.
 * *or do symlink to the folders mentioned above*
* Update references to Emgu.CV, Emgu.CV.UI and Emgu.Util

##DirectShow
* Download and extract DirectShowLib v2.1 (http://sourceforge.net/projects/directshownet/files/DirectShowNET/)
* Update reference to DirectShowLib-2005

##Manifest
* It's necessary to remove manifest to run project on dev machine.

#Running app

##Using existing video for debugging
Pass pathname of local video as command line argument.

##First run
> You need to go through the "Configure Regions" dialog the first time you use the application, or whenever it's recompiled, or whenever the intersection is switched.
>
> The ROI mask is the total region of interest. Any movement outside of this region is ignored/masked out.
>
> Each approach is a polygon enclosing a single road entering the intersection. Each exit is a polygon enclosing a single road exiting the intersection.  
>
> The regions can overlap - just try to approximately cover one entry or exit area with each approach/exit polygon. For turn detection to work properly, each approach/exit pair has to be on the same road, in the same direction. The approach exit/pairs are selected in counterclockwise order.
>
> Think of the intersection as being a north, south, east and west-travelling road. Then think of starting northbound. The northbound approach would be Approach 1 and the northbound exit would be Exit 1. Approach/Exit 2 would correspond to the westbound approach and exit, and so on.


Alexander Farley 2014
