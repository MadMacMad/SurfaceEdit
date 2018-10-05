### SurfaceEdit is an open-source software for processing photo scanned PBR surfaces.  
#### This processing includes:  
1. Selecting the size of the surface in meters.  
1. Setting the scale of the raw surface, immediately after baking, according to the size in the real world and the selected surface size.  
1. Removing artifacts and filling empty spaces with the stamp tool, just like in photoshop.  
1. Manual or automatic seams removal.  
1. Color correction and tweaking of textures of each channel. For example, the intensity of a normal map, the blurriness or contrast of a heightmap, and so on.  

#### Other features of SurfaceEdit:  
1. The ability to handle incredibly huge textures. Up to 8k in real time and 16k when exporting! (Not yet implemented, but this is a matter of time)  
1. All texture processing is done on gpu with shaders so SurfaceEdit is runs very well even on computers with cheap videocards. On gtx 960 I am able to draw in real time on a mask and blend 2 PBR 8K surfaces (each with 4 textures)!  
1. All the necessary functions for processing photo-scanned surfaces are present in one program.  


#### Development started on September 6, 2018. At the moment, the program is in the early alpha version, and most of the functionality has not yet been implemented.  

### Demos: 

#### Demo - Alpha 3  
[Youtube - Blending between 2 8K! PBR surfaces(each with 3 textures) using paintable mask ](https://www.youtube.com/watch?v=cMtayP6I3_0)  
[Download](https://github.com/grenqa/SurfaceEdit/releases/tag/Alpha-3)
#### Demo - Alpha 2  
[Youtube - Blending between 2 2K PBR surfaces(each with 3 textures) using paintable mask](https://www.youtube.com/watch?v=2CBqoBdKA2o)   
[Download](https://github.com/grenqa/SurfaceEdit/releases/tag/Alpha-2)
#### Demo - Alpha 1  
[Youtube - Texture panting with UNDO/REDO](https://www.youtube.com/watch?v=AlqMkDNpghc)  
[Download](https://github.com/grenqa/SurfaceEdit/releases/tag/Alpha-1)
