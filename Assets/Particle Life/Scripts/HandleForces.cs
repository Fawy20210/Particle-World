using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HandleForces : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    
    public int ParticleCount;
    public float ParticleScale;

    //public float gravity;
    
    public float ParticleSpacing;
    public float dampening;
    public float frictionFactor;
    public float timeFactor;

    public Color[] colors;
    public float forceScale;
    public float[] forceMultiplier;

    public float minRandomMinRange;
    public float maxRandomMinRange;
    public float maxRandomMaxRange;
    public float[] minRange;
    public float[] maxRange;


    public Vector2 boundSize;
    Vector2[] positions;
    Vector2[] velocitys;


    MaterialPropertyBlock block;
    RenderParams rp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float ForceFunction(float distance, float attractionFactor, float minDistance)
    {
        if (distance < minDistance)
        {
            return distance/minDistance - 1f;
        }
        else if (distance < 1)
        {
            return attractionFactor*(1-Math.Abs(2*distance-1-minDistance)/(1-minDistance));
        }
        else
        {
            return 0f;
        }
    }
    void HandleBoundsCollisions(int index)
    {
        
        Vector2 halfBoundSize = boundSize / 2 - Vector2.one * ParticleScale / 2;
        if (Math.Abs(positions[index].x) > halfBoundSize.x)
        {
            positions[index].x = halfBoundSize.x * math.sign(positions[index].x);
            velocitys[index].x *= -1 + dampening;
        }
        if (Math.Abs(positions[index].y) > halfBoundSize.y)
        {
            positions[index].y = halfBoundSize.y * math.sign(positions[index].y);
            velocitys[index].y *= -1 + dampening;
        }
    }

    void Start()
    {
        positions = new Vector2[ParticleCount*10];
        velocitys = new Vector2[ParticleCount*10];
        if (forceMultiplier.Length == 0) forceMultiplier = new float[colors.Length*colors.Length];
        if (minRange.Length == 0) minRange = new float[colors.Length*colors.Length];
        if (maxRange.Length == 0) maxRange = new float[colors.Length*colors.Length];

        int particlesPerRow = (int)math.sqrt(ParticleCount);
        int particlesPerCol = (ParticleCount -1) / particlesPerRow + 1;
        float spacing = ParticleScale + ParticleSpacing;

        for(int i = 0; i < ParticleCount; i++)
        {
            float x = (i % particlesPerRow - particlesPerRow / 2f + 0.5f) * spacing;
            float y = (i / particlesPerCol - particlesPerCol / 2f + 0.5f) * spacing;

            positions[i] = new Vector2(x,y);
        }        
        
        block = new MaterialPropertyBlock();
        rp = new RenderParams(material)
        {
            matProps = block
        };

        for(int i=0; i<colors.Length; i++)
        {
            for(int j=0; j<colors.Length; j++)
            {
                int index = i + j * colors.Length;
                forceMultiplier[index] = UnityEngine.Random.Range(-1f, 1f);
                minRange[index] = UnityEngine.Random.Range(minRandomMinRange, maxRandomMinRange);
                maxRange[index] = UnityEngine.Random.Range(maxRandomMinRange, maxRandomMaxRange);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < ParticleCount; i++)
        {   
            velocitys[i] *= frictionFactor;
            for (int j=0; j<ParticleCount; j++)
            {
                if(i != j)
                {
                    int index = i % colors.Length + j % colors.Length * colors.Length;
                    Vector2 dir = positions[j]-positions[i];
                    float distance = dir.magnitude;
                    
                    if (distance < maxRange[index] && 0 < distance)
                    {
                        float attractionFactor = forceMultiplier[index];
                        velocitys[i] += maxRange[index] * dir.normalized * ForceFunction(distance/maxRange[index], attractionFactor, minRange[index]/maxRange[index]) * forceScale * timeFactor;                    
                    }
                }
            }
        }
        
        for(int i = 0; i < ParticleCount; i++)
        {
            positions[i] += velocitys[i] * timeFactor; //Time.deltaTime; timefactor
            HandleBoundsCollisions(i);
        }

        for(int i = 0; i < ParticleCount; i++)
        {
            block.SetColor("_Color", colors[i%colors.Length]);
            Vector2 scale = new Vector2(ParticleScale,ParticleScale);
            Graphics.RenderMesh(rp, mesh, 0, Matrix4x4.TRS(positions[i], Quaternion.Euler(new Vector2(0,0)), scale)); //new Vector3(-4.5f, 0.0f, 5.0f)
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector2.zero, boundSize);
    }
}
/* # Devlog 2: Changing my approach
I have now decided to not have each particle be a GameObject, but instead handle all of them from 1 Script that stores each particles position and velocity, to then calculate interactions and draw them. Right now I'm doing the drawing using `Graphics.RenderMesh()` for each individual particle, and I can already have them be attracted or repelled by each other.
## What I plan on doing next:
- Add a border, so the particles don't escape.
- Make it so the particles starting positions are spread out.
- Add the possibility for particles to have different colors.

# Devlog 4: Getting to a working version
Like mentioned in my [last Devlog](https://stardance.hackclub.com/projects/14075/devlogs/18702), one thing I had to do was adding interaction based on color and distance, and hurray! I did both!
So I did it by having an array of floats, which contains the attraction matrix, but flattend, and did the same for the distance values, like this: 
```array[i + j * width] = matrix[i][j]```
I also made a function that based on a minRange and maxRange around a particle calculates it's attraction to other particles, if it's closer than minRange, it gets pushed away, otherwise if it's closer than maxRange, it gets attracted or repelled based on the attraction matrix.
I made it so instead of using `Time.deltaTime` it now uses a fixed value that you can change, and added friction, so the particles, making the particles alot more stable than they'd be otherwise.

So now, I'd have a usable version of Particle Life, but as you can see my To-Do list isn't quite finished yet:
- ~~Add interaction based on color and distance.~~
- Optimize and Improve calculations.
- Add an GUI / Interface */