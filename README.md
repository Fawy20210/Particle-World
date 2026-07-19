# Particle World
A particle simulator, that by using simple interaction rules, can create complex patterns.

![alt text](README-Assets/README-Project-Showcase.gif)
[***TRY IT OUT***](https://fawy2020.itch.io/particle-world)

## Quick Start
### Windows
**Download**
- [Download on itch.io](fawy2020.itch.io/particle-world)  
- [Download from Github (latest release)](Releases/Windows/ParticleWorld-0.3.zip)

**Launch**
1. Unzip the downloaded file.
2. Open the unziped folder.
3. Execute `Particle World.exe`


### Linux
***Linux files are provided for download, but are not tested, so they might not work or have issues.***  
***The following instructions may not be accurate, but I assume they should be.***

**Download**
- [Download from Github (latest release)](Releases\Linux\ParticleWorld-linux-0.3.zip)

**Launch**
1. Unzip the downloaded file.
2. Open the unziped folder.
3. Execute `ParticleWorld.x86_64`

## Features

- Runs on your GPU using compute shaders.
- Ability to change Simulation, Physics and Rendering Parameters.
- Ability to view and change the values of the matrixes for the three interaction parameters.
- Input validation.
- Collapsable UI Panels.
- Pause button to pause the simulation.
- An FPS counter.

## Controls

**Move Camera:** Left click anywhere there isn't UI, and move around your mouse.  
**Zoom:** Scroll with your mouse wheel anywhere there isn't UI.

## Parameters 
<!-- ### Short Variant -->

### Simulation Details: 

**Amount of Simulated Particles:** How many particles get simulated.  
**Min inner Range:** The lower bound used by the random number generator for the Min Range Matrix.  
**Max inner Range / Min outer Range:** The upper bound use by the random number generator for the Min Range Matrix, and a the lower bound used by the random number generator for the Max Range Matrix.  
**Max outer Range:** The upper bound use by the random number generator for the Max Range Matrix.  
**Amount of Colors:** How many different colors there are, which influences the matrixes, and the appereance of the particles.  
**Border Scale X & Border Scale Y:** How big the area the particles can move in is, proportional to your screen size in pixels.

### Physics Details:
<details>
<summary> <b>Friction Factor:</b> How much velocity the particles keep each time step. </summary>
 Bigger value -> -Stability & +Particle speed.<br>
 Smaller value -> +Stability & -Particle speed.
</details> 

<details>
<summary> <b>Time Factor:</b> The size of each time step. </summary>
 Bigger steps -> -Accuracy & +Simulation speed.<br>
 Smaller steps -> +Accuracy & -Simulation speed.
</details>

<details>
<summary> <b>Force Scale:</b> Scales the interaction forces. </summary>
 Bigger value -> -Stability & +Amount of Movement.<br>
 Smaller value -> +Stability & -Amount of Movement.
</details>

### Camera Details:
**Camera Movement Speed:** Multiplier to how fast the camera moves.

### Rendering Details:
**Particle Scale:** How big the particles should be rendered.  
**Colors:** A list of the current colors, each color can be changed, HEX colors are used.



