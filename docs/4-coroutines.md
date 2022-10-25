# Coroutines: Parallel Execution and Experiment Protocols

We have mastered automated, simple events with the Cube Factory. Now let's create more complex flows and dive deeper into [coroutines](https://docs.unity3d.com/2020.1/Documentation/ScriptReference/Coroutine.html)!

## Reuse previous Unity project

We can once again save some time by continuing to use the previous project. Just make sure to delete or deactivate the CubeFactory object and you should be good to go.

??? info "Archiving and copying Unity projects"
    Another way to continue work without sacrificing previously finished projects is to making a copy of a whole Unity project in your file explorer, e.g., by going to the Unity hub, right-clicking your project to "Show in Finder/Explorer" and copying its whole folder. You can rename the copied folder to anything you want, then go back to the Hub and clicking __Open__ instead of new __New Project__, then point it to your newly copied folder.

## Setting the scene

We create again a few new objects.

Create as children of the floor plane object two new __Plane__ 3D objects and configure them like this:

+ "StandingMark"
    + Position: `(-2.333, .001, -2.333)`
    + Scale: `(.1, 1, .1)`
+ "OriginMark"
    + Position `(0, .001, 0)`
    + Scale `(.1, 1, .1)`

Assign StandingMark our [old *BlueBox* material](2-vr-interaction.md#materials-and-colors), and the *RedBox* material to OriginMark.

Outside the floor object, at empty spaces in the hierarchy of our scene, create:

+ A __Sphere__ 3D object and name it "Zeppelin":
    + Position `(-1, 1.5, -1)`
    + Rotation `(0, 90, 0)`
    + Scale `(.5, .2, .2)`
+ A new __Cube__:
    + Position `(0, 1.5, 1)`
    + Scale `(.2, .1, .2)`

Attach to the new Sphere (Zeppelin) and the new cube our existing `IsCollidingChecker` script as a component: Click __Add Component__ and start typing in its name.

Give the *Is Colliding Checker* components you've just attached to the Cube and Zeppelin two new colors of your choosing. They can be different to spice it up a bit!

## Setting objects in motion

Now should have the two last objects simply hovering in the air. Let's write new scripts to get them moving.

Create a script called `Moving` and attach it to the Zeppelin object:


```csharp title="Moving.cs"
using UnityEngine;

public class Moving : MonoBehaviour
{
    [Tooltip("Units per second")] public float speed;
    [Tooltip("Start position in 3D space")] public Vector3 startPoint;
    [Tooltip("End position in 3D space")] public Vector3 endPoint;

    public float interpolator = 1f;
    public bool isDone => interpolator > .999f;

    void Update()
    {
        print(isDone);
        if (isDone) return;
        
        interpolator += speed * Time.deltaTime;
        transform.position = Vector3.Lerp(startPoint, endPoint, interpolator);
    }
}
```

It takes `speed` and two Vector3 variables for its moving speed ([via interpolation](1-first-project.md#interpolation)) and for start and end points of its journey.

!!! info "isDone?"
    The expression `#!csharp isDone => interpolator > .999f` is a handy shortcut: the `=>` operator assigns to `isDone` the result of evaluating `interpolator > .999f`, similar to how an `if` statement would do. You can read more about this at the [C# documentation](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members).

    What this effectively does is to constantly check if the `interpolator` is greater than `.999f`, and setting `isDone` to `true` if so, and `false` if not.

    `interpolator` being already `1` at the beginning will set `isDone` to `true`, thus aborting the execution of `Update()`. It will need to be set to a smaller value to get it going.

This function also outputs the state of `isDone` to the *Console* at each update using the `print()` command: you can see it when the program is running, as it will be filling up the console with printouts quickly and keep scrolling down.

!!! info "Printing to console"
    It can be a good practice to output the state of variables to the console to have a clear understanding of what is going on. Doing it as above (printing at each `Update()`) is one way, but usually it is used only at specific events, like when a variable is changed. Using `print()` — or the Unity equivalent [`Debug.Log()`](https://docs.unity3d.com/2020.1/Documentation/ScriptReference/Debug.Log.html) — well is a powerful helper to understanding and *debugging* your code, so feel free to try it on other variables and other positions in your code by yourself!

Set `speed` in the inspector to `0.25`, and give `(-1, 1.5, -1)` as the start point and `(-1, 1.5, 1)` as the end point for its trajectory.

Now create a script called `Rotating` for the new cube object:

```csharp title="Rotating.cs" hl_lines="8 12"
using UnityEngine;

public class Rotating : MonoBehaviour
{
    [Tooltip("Units per second")] public float speed;
    [Tooltip("Axis to rotate around")] public Vector3 axis;

    public bool isRotating;

    void Update()
    {
        if (!isRotating) return;
        
        transform.Rotate(axis, speed * Time.deltaTime);
    }
}
```

Very similar to our [first rotation script](1-first-project.md#editing-the-script--constant-rotation-around-a-fixed-axis), it differs only by having a public boolean called `isRotating` that is checked before performing the rotation — it acts basically as an on/off switch.

Set the speed to `180` and the rotation axis to `(1, 0, 0)` in the inspector.

If you run the game now, only the the cube object should be rotating, and then only if you set its `isRotating` parameter to true in its Rotating component in the inspector. Try it out, and play around with the parameters in the inspector to see their effects:

<div style='border-color: #018281; border-style: solid;'>
<div style='overflow: hidden; margin-top:-7.5%; margin-bottom:-34%; position:relative; padding-bottom:calc(70.80% + 33px); clip-path: inset(10% 0 45% 0)'>
<iframe src='https://gfycat.com/ifr/paltrymildavocet?controls=0&hd=1' frameborder='0' scrolling='no'' width='100%' height='100%' style='position:absolute;top:0;left:0;'></iframe>
</div></div>

## Scripting a protocol

With the two moving objects and two areas to step on have set the stage to introduce more complex flows, such as might be needed for actual experiments (or even games).

### Variables and assignments

Create a new script for the \[CameraRig\] object and name it `Protocol`:

```csharp title="Protocol.cs"
using System.Collections;
using UnityEngine;
using Valve.VR;

public class Protocol : MonoBehaviour
{
    public SteamVR_Action_Boolean Trigger; // Set to \actions\default\in\InteractUI in editor
    
    public Moving movingComp;
    public Rotating rotatingComp;

    public IsCollidingChecker zeppelinColliderChecker;
    public IsCollidingChecker cubeColliderChecker;

    public Transform headCamera;
    public Transform standingMark;

    public float positionTolerance = 0.15f;

    private bool isRunning = true;
}
```

At this point, nothing in this first part of the declaration should be unfamiliar: `Protocol.cs` holds a number of public variables to store a SteamVR action, four components from other objects, two transforms, a floating point number, and a boolean.

__Save__ the script as it is so far, and make sure it's attached to our \[CameraRig\]. Go back to the Unity editor to assign the unfilled variables in the inspector.

!!! tip "Challenge: assign it yourself!"
    Can you figure out yourself how to fill the fields in the inspector for the \[CameraRig\]'s Protocol component correctly?

    The names and expected types should make this easy. As for the Trigger assignment, try to remember what you did the two previous times.

    __Continue only after you're confident you set it up correctly.__ Don't hesistate to ask if you're having trouble here.



### Interaction functions

Let's give us the ability to interact with `Protocol.cs` within the VR world. Add these functions:

```csharp title="Protocol.cs"
private void Update()
{
    if (Trigger.state && isRunning)
    {
        isRunning = false;
    }
}

private bool IsStandingOnTarget(Vector2 targetPos)
{
    Vector3 pos3D = headCamera.position;
    Vector2 standingPos = new Vector2(pos3D.x, pos3D.z);

    return Vector2.Distance(standingPos, targetPos) < positionTolerance;
}
```

The `Update()` loop should be clear: if the trigger is pressed AND `isRunning` is already true, THEN turn the variable `isRunning` to false.

`IsStandingOnTarget(Vector2 targetPos)` takes a given target position (a 2D vector) and measures its distance to a 2D projection of the `headCamera` — if it's below the `positionTolerance`, it return true, otherwise false.

### Scripted flow

Let's now put these variables and functions to work.

We will use the `Start()` call for that, but first change it from a basic `private void` type function, we will turn it into an `IEnumerator` — this way, it acts as a coroutine and we can stop and continue its execution using [`WaitWhile()`](https://docs.unity3d.com/ScriptReference/WaitWhile.html), [`WaitUntil()`](https://docs.unity3d.com/ScriptReference/WaitUntil.html), and [`WaitForSecondsRealtime()`](https://docs.unity3d.com/ScriptReference/WaitForSecondsRealtime.html) commands.

Replace the current `Start()` function with this:

```csharp title="Protocol.cs"
 private IEnumerator Start()
{
    while (isRunning)
    {
        // Wait until user has moved onto square on the floor
        Vector3 standingMarkPos = standingMark.position;
        yield return new WaitWhile(() => !IsStandingOnTarget(new Vector2(standingMarkPos.x, standingMarkPos.z)));
        print("Stepped on the first square");
        standingMark.gameObject.SetActive(false); // Hide
        
        // Wait until user has touched the zeppelin
        yield return new WaitUntil(() => zeppelinColliderChckr.isColliding);
        print("Touched the Zeppelin");
        
        movingComp.interp = 0; // This triggers the start of Zeppelin's animation
        // Wait for moving animation to end
        yield return new WaitUntil(() => movingComp.isDone);
        print("Zeppelin's animation is done");
        
        // Move to center of the room
        yield return new WaitWhile(() => !IsStandingOnTarget(Vector2.zero));
        print("Stepped on the center square");
    
        // Wait until user has touched the cube
        yield return new WaitUntil(() => cubeColliderChckr.isColliding);
        rotatingComp.isRotating = true; // Start rotating cube
        print("Touched the cube");

        // Wait one second while it rotates
        yield return new WaitForSecondsRealtime(1f);
        rotatingComp.isRotating = false; // Stop rotating cube
        print("One second has passed");
        
        // RESET everything
        standingMark.gameObject.SetActive(true); // Show
        movingComp.transform.position = movingComp.startPoint;
        rotatingComp.transform.rotation = Quaternion.identity;
        print("Everything's reset!");
    }
}
```

You should be able to read this script and understand what it's doing. The various `yield return new` lines halt the execution of the function (stopping the advancement to the next lines) until their `Wait` clauses are fulfilled, as explained in the comments for every line.

Save the script, go back to Unity, and make sure the cube is not set to rotate already.

Run the game, and try to advance through the different steps as you can see in the protocol! The console will print updates on the user's progress through the steps.

<div style='border-color: #018281; border-style: solid;'>
<div style='overflow: hidden; margin-top:-7.5%; margin-bottom:-34%; position:relative; padding-bottom:calc(70.80% + 33px); clip-path: inset(10% 0 45% 0)'>
<iframe src='https://gfycat.com/ifr/tartcornygrunion?controls=0&hd=1' frameborder='0' scrolling='no'' width='100%' height='100%' style='position:absolute;top:0;left:0;'></iframe>
</div></div>

!!! Example "Challenge: more triggering"
    For now, the controller's trigger isn't actually doing much other than interrupting the flow. Can you think of a way to use it more creatively here?