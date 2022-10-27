# Final Challenge: Gaze Saber

A quick review of what you should have learned so far:

+ Creating a Unity project and navigating its interface
+ Making, manipulating, and animating objects using C# scripting
+ Giving your application interactivity and controlling its execution flow
+ Reading and using external resources and recording things that happened during the game to external files
+ Utilizing eye tracking in your application for interaction and data gathering

All these things together should enable you to create a simple version of something like Beat Saber:

<div style='border-color: #018281; border-style: solid;'>
<div style='overflow: hidden; margin-top:-7.5%; margin-bottom:-15%; position:relative; padding-bottom:calc(70.80% + 33px); clip-path: inset(10% 0 20% 0)'>
<iframe src='https://gfycat.com/ifr/totalscrawnyhalicore?controls=0&hd=1' frameborder='0' scrolling='no'' width='100%' height='100%' style='position:absolute;top:0;left:0;'></iframe>
</div></div>

We encourage you to distill it to its essentials and add an eye-tracking twist:

+ Have a coroutine that creates cubes coming towards you to destroy
+ Have the cubes appear with specific intervals, with some options to try out:
    + Fixed (like the cube factory)
    + Variable/random
    + Scripted/sequenced, with playlists of intervals read from an external file
    + Increasing in frequency as time goes on
+ Impart parameters on each cube (random or scripted):
    + Color (or texture)
    + Initial position
    + Velocity (increasing over time?)
+ Have colliders (your controllers or a additional saber-like object, e.g., cylinders) to destroy the cubes on contact, with playback of a sound
    + Allow any collider to destroy them or do like Beat Saber, where only the red saber cuts red cubes, etc
+ Allow cube destruction through gaze:
    + Immediate, as in the previous tutorial (too easy)
    + Requiring a fixation for minimum time before destruction (would require each cube object to keep track of how much time it has been gazed on)
+ Record player performance
    + Success rate (e.g., how many cubes were destroyed vs. passed behind the player)
    + Timing (e.g., how long did a cube survive on average before being destroyed)
    + Preference (e.g., did the user prefer to cut cubes or to stare them down)
    + Accuracy (e.g., were cubes "cleanly" fixated on, or did the player switch gaze from cube to cube without finishing them off)

There is a lot of freedom to express yourself in designing this simple game! Something that could be particularly fun is to balance the minimum time it takes to "stare down" a cube before it disappears, so that the difficulty of continuously looking at it is about equal to that of cutting it — this is of course highly dependent on the number, speed, and frequency of incoming cubes!

We're excited to see what you can come up with. Have fun!

<div style='border-color: #018281; border-style: solid;'>
<div style='overflow: hidden; margin-top:-13%; margin-bottom:-20%; position:relative; padding-bottom:calc(70.80% + 33px); clip-path: inset(17.5% 0 27% 0)'>
<iframe src='https://gfycat.com/ifr/foolishfoolishjavalina?controls=0&hd=1' frameborder='0' scrolling='no'' width='100%' height='100%' style='position:absolute;top:0;left:0;'></iframe>
</div></div>