# AOT - CS354R Final Project
## Overview
As stated in the design document, my final project is a plugin for the Unity Game Engine. 

- I was originally creating this in a separate C# solution outside of Unity and referencing the UnityEngine dll's,
but build instructions were much simpler once I moved all the code into the project itself, as the engine takes
care of compilation into one final dll.

This repo contains the contents of said project.

## Links

- [Design Doc](https://docs.google.com/document/d/1ak7sRlAg89GKwfk467ZYdelZO70lKg4bU35JVlfL7rw/edit)

- [Alpha/Milestone Video](https://youtu.be/-Xd1_spMVP0)

- [Report](https://docs.google.com/document/d/1nHYBb5cJcZX_f5hZiSG6fKHq9GPrcdM8xyLA5CkjKFU/edit?usp=sharing)

- [Video Presentation](https://youtu.be/0k1A6oWrVls)

- [Game Trailer](https://youtu.be/Beuzh6sr8bY)

## Build instructions

- Clone the repository.
- Download UnityHub [here](https://unity.com/download)
- Download Unity 2019.4.16f1 [here](https://unity.com/releases/editor/archive). It's under the Unity 2019.X tab. Click the UnityHub button to install through UnityHub.
- Once done, navigate back to the Projects tab in UnityHub
- Press the Open button, and navigate to the cloned repository. Open the cloned repository at its root.
- The Unity Editor will launch, opening the project. It may take some time to import the assets. 
- Once the editor is open, go to File->Build And Run. The editor will ask you where you want your build to be located. Any place on your machine works.
- Once the build completes, the game will open automatically. If not, you can run the .exe that will be created at the build location you selected.

## Instruction Manual

There's an in-game tutorial which I highly, highly recommend doing, but here's a list of controls anyways.

- WASD to move when grounded, E to jump
- Right click to tether
- Space to reel in
- Left click to attack
- Double press and hold WASD to burst forward, left, backwards, and right respectively (WHEN AIRBORNE)
- Hold Q or E to burst down or up respectively (WHEN AIRBORNE)

## Credits

- In-game music is Aquatic Base Level 2 by Mariko Nanba from Sonic the Hedgehog (2006)
- Some environment assets by [polyperfect](https://assetstore.unity.com/packages/3d/props/low-poly-ultimate-pack-54733)
- Skybox by PULSAR BYTES from [ColorSkies](https://assetstore.unity.com/packages/2d/textures-materials/sky/colorskies-91541)
- SFX by Sidearm Studios from [Ultimate SFX & Musix Bundle](https://assetstore.unity.com/packages/audio/sound-fx/ultimate-sfx-music-bundle-everything-bundle-200453)