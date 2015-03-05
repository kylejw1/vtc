vtc
===
Video Traffic Counter

---

Live: [www.traffic-camera.com](http://www.traffic-camera.com) Development: [dev.traffic-camera.com](http://dev.traffic-camera.com)

Realtime video-based object tracking software. Written in C# using Emgu CV. Requires a webcam.

#Development environment setup

Get sources to some dir (say - `c:\projects\vtc`). Further referenced as `REPODIR`.

##EmguCV
*to be improved*

It's assumed that `emgucv-windows-universal-cuda 2.9.0.1922` version of EmguCV is installed with default options (IOW - the library installed to `C:\Emgu\emgucv-windows-universal-cuda 2.9.0.1922\` dir).
* Solution projects are using the path, so no need to fix it.
* Run **setup.bat** (as Administrator) to create links to huge x86/x64 dirs of EmguCV library.
** (not sure) maybe it's necessary to build solution before the step above, so `bin` folder got created.

##DirectShow
??? can we simply put the dll to repository? Is it legal???

* Download and extract DirectShowLib v2.1 (http://sourceforge.net/projects/directshownet/files/DirectShowNET/)
* Copy `DirectShowLib-2005.dll` to `REPODIR\Thirdparty\DirectShowLibNET\`

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
