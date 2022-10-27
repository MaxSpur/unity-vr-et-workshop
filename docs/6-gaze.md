# Using Gaze Data

It's time for the final piece of the puzzle: using actual gaze data provided by the built-in eye tracker in our HTC Vive Pro Eye devices. Look out!

!!! info "Complex code, complex concepts"
    In this part, we will provide some code that would a bit too complex to fully explain in the time we have.
    
    You can of course try to understand it (and we encourage that!), but feel free to take this as an exercise in reusing existing solutions to problems that you don't have to reinvent yourself.

## Reuse or Rebuild

Again, we can reuse our previous VR Unity projects to save time, practice the necessary steps to set one up from scratch.

Have a Scrips folder in our assets, as well as a Resource folder, which contains the [pop.mp3 audio sample](3-runtime.md#making-it-pop) we've used before.

Have a [Plane 3D object as a floor](2-vr-interaction.md#adding-objects) in the scene — name it `Floor`.

## Setting up gaze tracking

Make sure that SteamVR and the SRAnipal packages are [installed and imported](2-vr-interaction.md#installing-and-importing-packages) into our project, and that we have a [\[CameraRig\] object instead of the default MainCamera](2-vr-interaction.md#the-camera-rig) in our hierarchy.

From the `ViveSR/Prefabs` folder in the projects folder, drag the __SRanipal Eye Tracking Framework__ into your scene's hierarchy — anything will do.

!!! warning "Potential issues with this prefab"
    Normally this should be all that you need to do, but if the console throws errors when you're starting the eye tracker, go to the __SRanipal Eye Tracking Framework__ object in your scene, and change its *Enable Eye Version* parameter to `2`.

There are a number of procedures necessary to do in code in order to have an eye tracker running and being useful in Unity, and they can be quite lengthy. __Download__ this script file and place it in your Scripts folder in the Project browser: <a href="EyeTrackingCtrlr.cs" download>EyeTrackingCtrlr.cs</a>.

Using the `ViveSR.anipal.Eye` library, it interfaces directly with the eye tracker inside the Vive Pro Eye — within it's `Start()` enumerator it waits for the hardware to be ready before continuing.

`EyeTrackingCtrlr.cs` defines a new class called `GazePoint` within itself, which holds information about where a user is looking with each eye (as well as a *combination* of both), and which (if any) objects the gaze *rays* are colliding with.

This "camera data" is updated each frame by calling `SRanipal_Eye.GetGazeRay()` functions that interface with the eye tracker, while "Eye data" is gathered when the function `EyeCallback()` is called directly by the eye tracker via a *callback*.

!!! info "General-purpose script"
    Overall, this script is very generalized and can be used for all sorts of projects with minimal alterations.

    The only specialized functionality in this script is that it specifically checks if a gaze collision happens with objects that have `CollidableCube` in their name, to which it then sends a `OnTriggerEnter` [message](https://docs.unity3d.com/ScriptReference/GameObject.SendMessage.html):

    ```csharp
    if (gazePoint.LeftCollide != null && gazePoint.LeftCollide.name == "CollidableCube")
            gazePoint.LeftCollide.SendMessage("OnTriggerEnter", new Collider(), SendMessageOptions.DontRequireReceiver);
    ```

    If you want to create your own projects building on this script, this would be the only part you'd really need to change.


Attach this script to the __Floor__ object as a component, so that it runs within our scene.



## Visualizing gaze

We shall now create cubes that will change their color once we gaze at them.

This will be done in a similar manner to our [Cube Factory](3-runtime.md#factory-scripting): new cubes will be created and outfitted with the necessary functionality.

### Animating cubes

We don't want cubes to [just disappear](3-runtime.md#destroyer-script) for now as we did with the cube factory, but let them instead change colors (suddenly, then gradually).

Create a new script in the Scripts folder named `AnimateOnCollide.cs`:

```csharp title="AnimateOnCollide.cs"
using System.Collections;
using UnityEngine;

public class AnimateOnCollide : MonoBehaviour
{
    public bool isColliding;
    
    public Color[] colors;
    private Material _material;

    private float _animationDuration = .25f;

    private IEnumerator Animate()
    {   
        float animationTime = _animationDuration;

        // Interpolate between color one and two
        while (animationTime >= 0)
        {
            animationTime -= Time.deltaTime;
            
            _material.color = Color.Lerp(colors[1], colors[0],1 - animationTime /  _animationDuration );

            yield return null; // Wait one frame
        }
    }
}
```

It is again similar to our previous `…OnCollide` scripts in structure. The IEnumerator `Animate()` will count down from the duration given in `_animationDuration` while steadily ["lerping"](1-first-project.md#interpolation), or interpolating between the first two colors in its `colors` array, and assign them to the objects's material.

Give this script now its `Start()` function to set an initial color, and an `OnTriggerEnter()` function which will (re)start the `Animate()` coroutine when a collision is detected:

```csharp title="AnimateOnCollide.cs"
void Start()
{
    _material = GetComponent<MeshRenderer>().material;
    _material.color = colors[0];
}

// Now sent by EyeTrackingCtrlr.cs!
public void OnTriggerEnter(Collider other)
{
    StopAllCoroutines();
    StartCoroutine(Animate());
}
```

### Creating cubes for gaze visualization

Our floor will now be a kind of cube factory. Create a new script for the floor plane object called `ProtocolVisualiseGaze`, which for now contains only its library calls, a single private variable (the EyeTrackingCtrlr), and a method for creating new, *interactive* cubes:

```csharp title="ProtocolVisualiseGaze.cs"
using System;
using System.Collections;
using UnityEngine;
using ViveSR.anipal.Eye;
using Random = UnityEngine.Random;

public class ProtocolVisualiseGaze : MonoBehaviour
{
    private EyeTrackingCtrlr eyeTrackingCtrlr;

    private static void CreateInteractiveCube(Vector3 position, Quaternion rotation, Color col1)
    {
        GameObject cubeGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeGo.name = "CollidableCube";
        Transform cubeTrans = cubeGo.transform;

        cubeTrans.position = position;
        cubeTrans.rotation = rotation;
        cubeTrans.localScale *= .15f;

       AnimateOnCollide cubeCollChk = cubeGo.AddComponent<AnimateOnCollide>();
       
       cubeCollChk.colors = new[]
       {
           col1,
           new Color (1f-col1.r, 1f-col1.g, 1f-col1.b)
       };
    }
}
```

`CreateInteractiveCube()` can create cubes with a given `position`, `rotation`, and `color`, and outfits them with our `AnimateOnCollide` component. It gives them the name "CollidableCube", and assigns their `AnimateOnCollide` component two colors: one given to the `CreateInteractiveCube()` function via the `col1` parameter, and its *opposite* via the `new Color (1f-col1.r, 1f-col1.g, 1f-col1.b)` command.

Now add a `Start()` IEnumerator to this script to execute cube generation:

```csharp title="ProtocolVisualiseGaze.cs"
IEnumerator Start()
{
    eyeTrackingCtrlr = EyeTrackingCtrlr.instance;
    yield return new WaitUntil(() => eyeTrackingCtrlr.isReady);
    
    // Calibrate eye tracker once at the start - comment out after first time
    bool calibrationSuccess = false;
    while (!calibrationSuccess)
    {
        int calibReturnCode = SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero);
        print($"calibReturnCode: {calibReturnCode} == {(int) ViveSR.Error.WORK}");
        calibrationSuccess = calibReturnCode == (int) ViveSR.Error.WORK;
    }

    // Create floating cubes in a square formation around the room's origin
    Vector2[] moveVec = new[]
    {
        new Vector2(0,-1),
        new Vector2(1,0),
        new Vector2(0,1),
        new Vector2(-1,0),
    };
    
    Vector3 startPos = new Vector3(1.8f, 1.6f, 1.8f);
    
    for (int iBorder = 0; iBorder < 4; iBorder++)
    {
        float tmpVal = startPos.x; 
        startPos.x = -startPos.z;
        startPos.z = tmpVal;
        
        for (int iCube = 0; iCube < 4; iCube++)
        {
            Vector3 position = startPos;
            position.x += moveVec[iBorder].x * (3.6f/4f * iCube);
            position.z += moveVec[iBorder].y * (3.6f/4f * iCube);
            
            CreateInteractiveCube(position, Random.rotation, Random.ColorHSV());
            yield return new WaitForSeconds(.1f);
        }
    }
}
```

While this function looks long, it's rather simple in what it does:

+ It *instantiates* a new `EyeTrackingCtrlr`, thereby getting our [code above](#setting-up-gaze-tracking) to run
+ It pauses until the eye tracker is ready
+ Attempts to [*calibrate* the eye tracker](https://developer.vive.com/us/support/sdk/category_howto/how-to-calibrate-eye-tracking.html), and finally
+ Creates four interactive cubes around the center of the room with random colors and orientations.

!!! example "Challenge: Add a toggle for calibration"
    The block of code responsible for calibrating the eye tracker does not need to run every time, and will be rather annoying if you have to go through it each time you try the script.

    Currently, there is a line that says "comment out after first time" — you can surely do that, and uncomment again whenever you do want run the calibration routine again, but this, too, can get old, fast.

    As a challenge you can try create a new `bool calibrate` variable that's visible in the inspector, and have it determine whether the calibration block is executed!

Save the code and try it out. See if you can activate cubes by just looking at them:

<div style='border-color: #018281; border-style: solid;'>
<div style='overflow: hidden; margin-top:-7%; margin-bottom:-32%; position:relative; padding-bottom:calc(70.80% + 33px); clip-path: inset(9% 0 42% 0)'>
<iframe src='https://gfycat.com/ifr/farflungindeliblealleycat?controls=0&hd=1' frameborder='0' scrolling='no'' width='100%' height='100%' style='position:absolute;top:0;left:0;'></iframe>
</div></div>

<!-- 
!!! example "Challenge: Fix blinking cubes"
    As you can see, the cubes just blink briefly as we graze them with our gaze.

    Do you think you can change `AnimateOnCollide` in such a way that the cubes stay in their activated color until we look away from them? -->

### If looks could destroy

Instead of animating our cubes by looking at them, let's destroy them as we did the surplus output from our [Cube Factory](3-runtime.md#cube-removal).

Create a new script called `DestroyOnCollide` in our scripts folder:

```csharp title="DestroyOnCollide.cs"
using System.Collections;
using UnityEngine;

public class DestroyOnCollide : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.clip = Resources.Load<AudioClip>("pop");
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(PlayAudioThenDestroy());
    }

    private IEnumerator PlayAudioThenDestroy()
    {
        print($"Destoyed {name}");
        // Hide object
        Destroy(GetComponent<MeshRenderer>());
        // Delete collider component to prevent calling this coroutine twice
        Destroy(GetComponent<Rigidbody>());
        // Play pop sound
        _audioSource.Play();
        yield return new WaitUntil(() => !_audioSource.isPlaying);
        // Actually destroy the object now
        Destroy(gameObject);
    }
}
```

Its structure should be nothing new by now.

Go back to our `ProtocolVisualiseGaze` script and change its `CreateInteractiveCube()` function to look like this:

```csharp title="ProtocolVisualiseGaze.cs"
private static void CreateInteractiveCube(Vector3 position, Quaternion rotation, Color col1)
{
    GameObject cubeGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
    cubeGo.name = "CollidableCube";
    Transform cubeTrans = cubeGo.transform;

    cubeTrans.position = position;
    cubeTrans.rotation = rotation;
    cubeTrans.localScale *= .15f;

    cubeGo.AddComponent<DestroyOnCollide>();
    cubeGo.GetComponent<MeshRenderer>().material.color = col1;
}
```

Save the code, run the game, and get popping!

<div style='border-color: #018281; border-style: solid;'>
<div style='overflow: hidden; margin-top:-7%; margin-bottom:-32%; position:relative; padding-bottom:calc(70.80% + 33px); clip-path: inset(9% 0 42% 0)'>
<iframe src='https://gfycat.com/ifr/foolhardygenuinecod?controls=0&hd=1' frameborder='0' scrolling='no'' width='100%' height='100%' style='position:absolute;top:0;left:0;'></iframe>
</div></div>

## Sampling gaze

While it can be fun to pop some cubes from time to time, we're here to learn about getting some science from all this, so let's actually collect data and save it in external files.

### Gaze sampler script

<a href="EyeTrackingSmplr.cs" download>Download</a> a modified version of our `EyeTrackingCtrlr` here, now called <a href="EyeTrackingSmplr.cs" download>EyeTrackingSmplr.cs</a> and put in the Scripts folder.

It is very similar to our previous `EyeTrackingCtrlr.cs` script, differing only in its inclusion of a `StreamWriter` which it will use to record data to a file, and its `EyeCallback()` method, which will now use that StreamWriter to add new entries to our recording every time the eye tracker updates its data.

You can see how it does that in its `EyeCallback()` method, but here is its part that determines the format of its output:

```csharp title="EyeTrackingSmplr.cs"
instance.writer.WriteLine(
    $"{gazePoint.data.timestamp},{instance.UnityTimeStamp}," +
    $"{instance.cameraPosition.x},{instance.cameraPosition.y},{instance.cameraPosition.z}," +
    $"{instance.cameraQuaternion.x},{instance.cameraQuaternion.y}," +
    $"{instance.cameraQuaternion.z},{instance.cameraQuaternion.w}," +
    $"{meanBasePoint.x},{meanBasePoint.y},{meanBasePoint.z}," +
    $"{meanGazeDirection.x},{meanGazeDirection.y},{meanGazeDirection.z}," +
    $"{leftBasePoint.x},{leftBasePoint.y},{leftBasePoint.z}," +
    $"{leftGazeDirection.x},{leftGazeDirection.y},{leftGazeDirection.z}," +
    $"{rightBasePoint.x},{rightBasePoint.y},{rightBasePoint.z}," +
    $"{rightGazeDirection.x},{rightGazeDirection.y},{rightGazeDirection.z}," +
    $"{letPupilDiam},{rightPupilDiam}," +
    $"{valC},{valL},{valR}", false
);
```

Looking a bit ungainly, you should still be able to decipher it. The `+` signs at the end of each line except the last are needed to keep it all one string, and the `$` signs at the beginnings of blocks within `""` quotation marks lets Unity convert all the variables (e.g., rightPupilDiam) to plain text for the writer (and us).

Replace the *EyeTracking Ctrlr* component in our floor object with the new EyeTrackingSmplr.cs script (or keep both the old one but deactivate it).

### New protocol script

To conduct a repeatable experiment we need a new protocol.

<a href="CubeSequenceSampling.cs" download>Download</a> a new version of `ProtocolVisualiseGaze.cs`, now called <a href="CubeSequenceSampling.cs" download>CubeSequenceSampling.cs</a>. You can of course update the ProtocolVisualiseGaze script manually with the changes you see in ProtocolVisualiseGaze.

There are now a few more variables, which we will need to tell the script where to write data to, and a *container* which will hold our generated cubes to keep the organization a bit more tidy.

```csharp title="CubeSequenceSampling.cs"
public class CubeSequenceSampling : MonoBehaviour
{
    private EyeTrackingSmplr eyeTrackingSmplr;
    
    public static string dirpathname = "subjData/";
    public static string dirpath;

    private Transform CubeContainerTrans;
}
```

Within its `Start()` function, it now creates a new file path to store our data:

```csharp title="CubeSequenceSampling.cs"
dirpath = Directory.GetParent(Application.dataPath).ToString() + Path.DirectorySeparatorChar + dirpathname;
Directory.CreateDirectory($"{dirpath}");

eyeTrackingSmplr = EyeTrackingSmplr.instance;

yield return new WaitUntil(() => eyeTrackingSmplr.isReady);
```

When creating our randomly-colored cubes, we now store them inside the (so far empty) `CubeContainerTrans` GameObject, and promptly *deactivate* them.

```csharp title="CubeSequenceSampling.cs" hl_lines="18"
CubeContainerTrans = new GameObject("CubeContainer").transform;

Vector3 startPos = new Vector3(1.8f, 1.6f, 1.8f);

for (int iBorder = 0; iBorder < 4; iBorder++)
{
    float tmpVal = startPos.x; 
    startPos.x = -startPos.z;
    startPos.z = tmpVal;
    
    for (int iCube = 0; iCube < 4; iCube++)
    {
        Vector3 position = startPos;
        position.x += moveVec[iBorder].x * (3.6f/4f * iCube);
        position.z += moveVec[iBorder].y * (3.6f/4f * iCube);
        
        GameObject cube = CreateInteractiveCube(position, Random.rotation, Random.ColorHSV());
        cube.SetActive(false);
    }
}
```

This storing happens in the modified `CreateInteractiveCube()` method, where the new cube gets a *parent* assigned — our CubeContainerTrans:

```csharp title="CubeSequenceSampling.cs" hl_lines="12"
private GameObject CreateInteractiveCube(Vector3 position, Quaternion rotation, Color col1)
{
    GameObject cubeGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
    cubeGo.name = "CollidableCube";
    Transform cubeTrans = cubeGo.transform;

    cubeTrans.position = position;
    cubeTrans.rotation = rotation;
    cubeTrans.localScale *= .15f;
    
    // Assign this cube as a child of "CubeContainer" GameObject
    cubeTrans.SetParent(CubeContainerTrans);
    
    cubeGo.AddComponent<DestroyOnCollide>();
    cubeGo.GetComponent<MeshRenderer>().material.color = col1;
    
    return cubeGo;
}
```

### Popping cubes for science

Coming back to its `Start()` function, its last part now randomly reactivates cubes. This makes them susceptible to being destroyed by a gaze from the participant, which the code is waiting for before reacting the next one in line until they are all gone. A variable `itrial` keeps track of which cube we're currently at, by being incremented (`itrial++`) in each iteration of the `while()` loop.

```csharp title="CubeSequenceSampling.cs" hl_lines="11"
int itrial = 0;

while (CubeContainerTrans.childCount > 0)
{
    int iCube = (int)(Random.value * CubeContainerTrans.childCount);

    GameObject cubeGO = CubeContainerTrans.GetChild(iCube).gameObject;
    // Show cube
    cubeGO.SetActive(true);
    
    eyeTrackingSmplr.writer = new StreamWriter($"{dirpath}/Cube_{itrial++}.csv");
    eyeTrackingSmplr.isSampling = true;
    
    // Wait for cube to be destroyed
    yield return new WaitUntil(() => cubeGO == null);
    
    eyeTrackingSmplr.isSampling = false;
    eyeTrackingSmplr.writer.Close();
}
```

It also creates a new `writer` within its `eyeTrackingSmplr`, so that it can pass a new file name for each cube. The `eyeTrackingSmplr` is turned on by setting its `isSampling` value to `true` until the cube is popped, which is also when we close the existing writer.

With each iteration we thus create one new recording and decrease the count of remaining cubes by one, until the `#!csharp while (CubeContainerTrans.childCount > 0)` loop exits.

And that's it! Replace the old ProtocolVisualiseGaze component of our floor object with this new script (or just deactivate it before adding the new one), and give it a go. You can see the resulting recordings in the automatically created *SubjData* folder, which you can find in the project's main folder, *outside* Assets.

<div style='border-color: #018281; border-style: solid;'>
<div style='overflow: hidden; margin-top:-7%; margin-bottom:-32%; position:relative; padding-bottom:calc(70.80% + 33px); clip-path: inset(9% 0 42% 0)'>
<iframe src='https://gfycat.com/ifr/selfishrewardingarkshell?controls=0&hd=1' frameborder='0' scrolling='no'' width='100%' height='100%' style='position:absolute;top:0;left:0;'></iframe>
</div></div>

That wasn't so hard, was it ;-)

## Challenges


Here are some suggestions to practice what you've hopefully learned here:

!!! example "Challenge: Analyze the data!"
    Have a look at the generated .csv files. What can you see from them?

    Also, we currently only store the raw values and nothing else. Can you let our game create a __header__ for the files that tells software like *R* or *Numbers* what each column means?

!!! example "Challenge: Signal the start and end of a session?"
    Can you think of a way to signal to the participant in VR that all cubes are popped and their quest is over?

    What about a count down at the beginning, so that the data for the first popped cube is more consistent, i.e. waiting with the recording of the first trial until some interaction or visible event occuring?

!!! example "Challenge: New folders for each participant?"
    Right now, all files are written when you restart the game. Can you think of way to e.g., enter a participant's number, so that a new folder is created for each run?

More formally, we would ask you to apply all you've learned from these tutorials up to now to a [final challenge on the next page](7-challenge.md), which may also challenge your creativity!

!!! question "Give us your feedback!"
    At the end of this workshop we would appreciate your feedback — did it work for you? Suggestions? Criticism?

    Please fill out this form just before you leave, or shortly after — a fresh memory would be best ;-)

    [https://sgl.uni-frankfurt.de/GiessenWorkshopFeedback/](https://sgl.uni-frankfurt.de/GiessenWorkshopFeedback/)