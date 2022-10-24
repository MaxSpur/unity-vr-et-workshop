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