# First Unity Project — getting to know the Editor

<img src="0-hub-app.png" alt="Iconic" width="200"/>
<!-- ![Unity Hub Desktop Icon](assets/page-1/0-hub-app.png "Iconic") -->

## Create a first project using the Unity Hub

If not already running on your computer, start the Unity Hub (see the icon above). Depending on previous use there may be already projects listed in its window — if so, disregard them for now and just click the __New project__ button.

![The Unity Hub](page-1/0-hub-new.png "The Unity Hub")

You will be presented with a dialog to configure the project. Usually it's already all set up correctly, but to make sure just verify the following points:

+ __Editor Version__ should be the latest one from the list (2021.3.11f1 in our case),
+ The __3D__ *template* should be selected,
+ Don't forget to give your first project a name and choose a location (the default works here).

![Creating a new project](page-1/1-hub-create.png "My first Unity Project")

Pushing the __Create project__ button will open the *Unity Editor* and tell it create a new project with the above parameters. This may take some time…

![Now Printing](page-1/2-wait.png "なうぷりんてぃんぐ")

## Setting up the Editor

Once this process is complete, you will see a the Unity Editor in its default configuration, showing a blank *scene.*

![Raw Unity](page-1/3-raw-unity.png "Blank and raw")

You can leave this as is, or drag the tab handles and section dividers around to create a more comfortable working environment. One particularly important change would be to permanently reveal the *Console* window by dragging the *Project* window it's being obscured by to a new location. Below is our suggested arrangement with new placements of the *Hierarchy,* *Project,* and *Console* windows.

![Comfortable Unity](page-1/4-better-unity.png "That's better.")

The biggest part should show you the *__Scene__ View,* where you can freely navigate using common 3D software controls, like zooming in and out with the mouse wheel, rotating by holding the right mouse button, and translating your position by holding the middle mouse button. See the [official Unity manual](https://docs.unity3d.com/Manual/SceneViewNavigation.html) on more information on this.

## Scene, Hierarchy, and Camera Views + Inspector

The Scene View shows you the contents of your currently open *Scene* — a kind of super *Object* that holds all other objects. A full overview of all objects and their relationships is shown in the *__Hierarchy__ window* — as you can see, it currently only contains our only Scene (__SampleScene__), which holds a simple *Directional Light* for illuminating the world and the *Main Camera* that lets a player see it. Let's select that camera by clicking on it in the *Hierarchy* or on its *Gizmo* (camera icon) in the *Scene* view:

![Selected Camera](page-1/5-select%20camera.png "Inspecting the camera")

It is now highlighted in both *Scene* and *Hierarchy* views, as shown by a highlight and the appearance of arrows pointing in the cardinal directions around the object. In addition, a view from this camera now shows up in the Scene view as a floating window: this is what a player of this game will see if we run it. The same view is visible in a full window by selecting the *Game* tab in the Scene window.

Another thing that happens when we select a *Game Object* like this camera in the editor is the appearance of this object's *__Inspector__* in its previously empty window to the top right. This window allows us to view and change every single aspect of any Game Object, which may differ by their type. One *Component* all possible objects, be they cameras, lights, or dinosaur models share is their *Transform:* a grid that contains the coordinates of their position, rotation vector, and its scale.

??? info "Transforms and Hierarchies"
    In the case of the camera and light and any other objects directly under the __SampleScene__ Scene object, these coordinates are equivalent to their "world" coordinates. For any object that sits below another in the Hierarchy, these coordinates are basically offset by those of their *Parent.* See Unity's documentation on the [Hierarchy](https://docs.unity3d.com/Manual/Hierarchy.html) and [Transforms](https://docs.unity3d.com/Manual/class-Transform.html) for more details.

Let's set the camera's Position to the *origin* and rotate it to point in the blue arrow's (the Z-axis) direction by editing its Transform Component accordingly: Position `X=0, Y=0, Z=0` and Rotation `X=0, Y=90, Z=0`.

## Creating a first object