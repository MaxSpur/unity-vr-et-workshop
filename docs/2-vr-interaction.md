# Interaction in VR

Let's now dig into actually using VR with Unity. For that we need to add some additional packages to Unity which will be specific to the hardware we're using for this tutorial — the HTC Vive Pro Eye.

## A new project

Let's first of all create a new Unity 3D project with the hub, like before. You can close the one from the previous tutorial, we won't be needing it anymore. Set up the new project's window like we did for the first tutorial, or to your own liking.

### Installing and importing Packages

In the Unity editor's menu bar, go to __Window → Package Manager__, which will open a new floating window. Selecting *My Assets* from the __Packages__ drop-down menu at the top, type in `steamvr` in the search box and you should have the *SteamVR Plugin* appear — import it with the __Import__ button below. A new window will present itself to let you select parts of the package, but we will just keep everything checked and hit __Import__ again.

??? info "The Asset Store"
    If SteamVR can not be found anywhere in the Package Manager, it may not be added yet to this Unity account. In that case go to the Unity Asset store, find it there, and add it to "your assets."

![Importing SteamVR](2-vr-interaction/3-import-combined.png "Importing it all")

During this, some dialog boxes may appear — just confirm what they want. SteamVR may ask to accept changes to the project's settings — __Accept All__ and it will be very happy. You can close the package manager windows now.

Having the basics for VR installed with the SteamVR package, we now need an additional one for the eye tracker. You could [download it from the developer's website](https://developer.tobii.com/xr/develop/unity/getting-started/vive-pro-eye/#step-3-download-and-import-the-vive-sranipal-sdk), but we have already put it in a folder on your desktop.

To import a package (or an *asset*) that is not listed in the package manager, you need to go to __Assets → Import Package → Custom Package…__ in the menu bar. Navigate to the *packages* folder in the workshop folder and locate `Vive-SRanipal-Unity-Plugin.unitypackage` under `packages → SDK-v1.3.3.0 → SDK → 02_Unity`. Confirm with __Open__ and you'll be presented with the same interface as before to select only parts of it — again, keep all checked and __Import__.


## Adding VR basics

Now that we have the necessary packages all imported, we can add the bare necessities to our scene for VR to function.