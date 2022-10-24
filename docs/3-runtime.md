# Runtime: Automated Processes

For now, our scenes have only strictly reacted to our inputs. Let's create some automated processes.

## Reuse the previous project

For this project, let's continue building on our previous one, without having to again import the libraries and creating our cubes or spheres (from the previous [challenge](2-vr-interaction.md/#switching-colors-of-a-material)) that trigger collisions.

Just remove (or deactivate) the cylinder GameObject from the hierarchy and delete the *Controller Input (Script)* component from our \[CameraRig\] GameObject.

## Shape of a cube factory

What's a better example for automation than a factory? Let's make a *cube factory.*

Create a new, empty GameObject in the hierarchy ("Create Empty") and name it `CubeFactory`. Place it at `(-1, 0, -0.25)`.

Now right-click on it to create *child* objects within it: six new cubes. Their names, positions, and scales should be as below.

!!! info "Editing Time Savers"
    You can quickly enter these numbers by hitting the Tab-key after entering each one, which will advance the focus to the next field. You can also save time by omitting the zeros before periods: the editor will fill them in automatically.

1. "CubeRight"
	+ Position `(0, 0.9, 0)`
    + Scale `(0.5, 1.8, 0.25)`
2. "CubeLeft"
	+ Position `(0, 0.9, 0.5)`
    + Scale `(0.5, 1.8, 0.25)`
3. "CubeBack"
	+ Position `(-0.2, 0.9, 0.25)`
    + Scale `(0.1, 1.8, 0.25)`
4. "CubeSlope"
	+ Position `(0, 0.17, 0.25)`
	+ Rotation `(0, 0, 45)`
    + Scale `(0.1, 0.6, 0.25)`
5. "CubeFront"
	+ Position `(0.2, 1.175, 0.25)`
    + Scale `(0.1, 1.25, 0.25)`
6. "CubeInteract"
	+ Position `(0.21, 1, 0.25)`
    + Scale `(0.1, 0.2, 0.2)`

You should end up with a hollow cuboid that has a sloped opening at its lower end — kind of like a chimney. "CubeInteract" should stick out just a little:

![Cube factory](3-runtime/1-cube-factory.png "A cube factory, ready to produce")

Another very useful "3D Object" we can add to the factory is *Text - TextMeshPro.* Create one within the CubeFactory GameObject, and give it the following parameters (it may ask you to import some resources the first time you do that — let it):

+ Within the *Rect Transform* component:
    + Position = `(0.251, 1.25, 0.25)`
    + Width = 70, Height = 20
    + Rotation = `(0, -90, 0)`
    + Scale = `(0.01, 0.01, 1)`
+ Center and middle alignment icons in its *TextMeshPro - Text* component
+ The following text in place of "Sample text":

    > &lt;b>Cube Factory<\b>
    >
    > Touch the red part with the controller and press the trigger to create a new cube
+ Set its Vertex Color to anything you like

You should now have a helpful label for the cube factory:

