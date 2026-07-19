# Particle World
A particle simulator, that by using simple interaction rules, can create complex patterns.


![alt text](README-Assets/README-Project-Showcase.gif)
[***TRY IT OUT***](https://fawy2020.itch.io/particle-world)

## How it works
Every Particle has a color, and that color defines how it interacts with other particles, because every color can get either attracted or repelled by another color, as long as that other particle of that color isn't to close, but also not to far away, where those three values, are different for every color to color combination, where for example, red to blue and blue to red, don't have to be the same value.

## Quick Start
### Windows
**Download**
- [Download on itch.io](https://fawy2020.itch.io/particle-world)  
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

### Matrix Details:
The matrixes show the interaction parameters for every color with every color, and let you change them.

<details>
<summary> <b>Attraction Matrix:</b> How different colors attract each other when in an valid range. </summary>
 Positive values -> Attraction.<br>
 Negativ values -> Repulsion.
</details>

<details>
<summary> <b>Min Range Matrix:</b> The closest a color can be to another color / the lower bounds of the valid range. </summary>
 A color closer than their respective Min Range value to another color, always gets repelled by that other color.
</details>

<details>
<summary> <b>Max Range Matrix:</b> The biggest distance a color can be to another color to still be affected by it / the upper bounds of the valid range. </summary>
 A color further apart than their respective Max Range value with another color, isn't affected by that other color.
</details>


## Some insights

<details>
<summary> <b>What is spatial partitioning, and why did I use it?</b> </summary>

Spatial partitioning is a method where you partition a space, into smaller subspaces of a certain size, to be able to efficently find objects that are in a certain area of your space.   

Before implementing spatial partitioning, every particle had to check for every other particle whether it was close enough to be affected, which is pretty slow with large amounts of particles.  
But by partitioning space into squares with the size of the maximum possible range, it only needs to check the square it is in, and the ones around it, ignoring all particles too far away to even remotly be close enough, and greatly increasing performance.
</details>

<details>
<summary> <b>Why did I choose to run the computations on the GPU?</b> </summary>

I chose to run the computation on the GPU because GPUs are normally really good at doing things in parallel, which is great if you need to compute many things that are pretty much the same, and the CPU would be to slow to do the same.
 
</details>


## Credits
I used [Sandbox Science's implementation of particle life](https://sandbox-science.com/particle-life) as inspiration, mostly for the UI, and as a reference to see how the simulation should roughly work.

I used Sebastian Lagues [Fluid-Sim](https://github.com/SebLague/Fluid-Sim/) and his videos about making it as references on how to implement certain things, like spatial partitioning, and used his [BitonicMergeSort implementation](https://github.com/SebLague/Fluid-Sim/tree/Episode-01/Assets/Scripts/Compute%20Helpers/GPU%20Sort/Resources) for sorting using the GPU.
