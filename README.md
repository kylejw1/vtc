vtc
===
Video Traffic Counter

---

Live: [www.traffic-camera.com](http://www.traffic-camera.com)<br> Development: [dev.traffic-camera.com](http://dev.traffic-camera.com)

Realtime video-based object tracking software. Written in C# using Emgu CV. Requires a webcam.

#Development environment setup

Get sources to some dir (say - `c:\projects\vtc`). Further referenced as `REPODIR`.

##EmguCV
It's assumed that `emgucv-windows-universal-cuda 2.9.0.1922` version of EmguCV is installed with default options (IOW - the library installed to `C:\Emgu\emgucv-windows-universal-cuda 2.9.0.1922\` dir).
* Solution projects are using the path, so no need to fix it.
* Run **setup.bat** (as Administrator) to create links to huge x86/x64 dirs of EmguCV library.
** (not sure) maybe it's necessary to build solution before the step above, so `bin` folder got created.

##Manifest
* It's necessary to remove manifest to run project on dev machine.

##Image debugger visualizer
During debugging it's useful to view image objects as a raster image, not as a array or bytes. To install image visualizer follow instructions at `\Thirdparty\ImageVisualizer\readme.txt`.

#Running app

##Using existing video for debugging
Pass pathname of local video as command line argument.

##Visualizing unit tests
Pass full pathname to assembly with unit tests. <br>
**IMPORTANT:** unit tests using **OWN** settings. Check unit test source code to see values!

#Debug output
Debug output is written to C:\TrafficCounter\VTClog.txt (see app.config)


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
