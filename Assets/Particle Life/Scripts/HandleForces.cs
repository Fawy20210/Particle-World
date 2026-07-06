using System;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
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

    int2[] spatialLookUp;
    int[] startIndices;
    (int offsetX, int offsetY)[] offsets =
        {
            (-1, 1), (0, 1), (1, 1),
            (-1, 0), (0,0), (1, 0),
            (-1, -1), (0, -1), (1, -1)
        };


    
   

    MaterialPropertyBlock block;
    RenderParams rp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int curspat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int hashfunc(int x, int y)
    {
        int cellX = math.abs(x) * (x < 0 ? 2377 : 2069);
        int cellY = math.abs(y) * (y < 0 ? 607 : 733);
        return (cellX + cellY) % ParticleCount;
    }
    int getCellKey(Vector2 val)
    {
        return hashfunc(Mathf.FloorToInt(val.x/maxRandomMaxRange), Mathf.FloorToInt(val.y/maxRandomMaxRange));
    }
    (int x, int y) PositionToCellPos(Vector2 val)
    {
        return (Mathf.FloorToInt(val.x/maxRandomMaxRange), Mathf.FloorToInt(val.y/maxRandomMaxRange));
    }
    
    void updateSpatialLookup()
    {
        Parallel.For(0, ParticleCount, i =>
        {
            int cellkey = getCellKey(positions[i]);
            spatialLookUp[i] = new int2(cellkey, i);
            startIndices[i] = -1;
        });

        Array.Sort(spatialLookUp, (a,b) => a.x.CompareTo(b.x));

        Parallel.For(0, ParticleCount, i =>
        {
            int key = spatialLookUp[i].x;
            int keyprev = (i == 0) ? -1 : spatialLookUp[i-1].x;

            if(key != keyprev)
            {
                //Debug.Log(key);
                startIndices[key] = i;
            }
        });  
    }
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
        spatialLookUp = new int2[ParticleCount];
        startIndices = new int[ParticleCount];

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
        //updateSpatialLookup();
        
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
        updateSpatialLookup();
        for(int i = 0; i < ParticleCount; i++)
        {   
            velocitys[i] *= frictionFactor;

            (int cellX, int cellY) = PositionToCellPos(positions[i]);

            foreach ((int offsetX, int offsetY) in offsets)
            {
                int key = hashfunc(cellX+offsetX, cellY+offsetY);
                int startIndex = startIndices[key];

                for(int j=startIndex; 0<=j && j<spatialLookUp.Length; j++)
                {
                    if (spatialLookUp[j].x != key) break;

                    int particleIndex = spatialLookUp[j].y;

                    Vector2 dir = positions[particleIndex]-positions[i];
                    float sqrDistance = dir.sqrMagnitude;


                    int RangeIndex = i % colors.Length + particleIndex % colors.Length * colors.Length;


                    if(sqrDistance < maxRange[RangeIndex] * maxRange[RangeIndex] && 0 < sqrDistance)
                    {
                        float distance = MathF.Sqrt(sqrDistance);

                        float attractionFactor = forceMultiplier[RangeIndex];
                        velocitys[i] += maxRange[RangeIndex] * dir.normalized * ForceFunction(distance/maxRange[RangeIndex], attractionFactor, minRange[RangeIndex]/maxRange[RangeIndex]) * forceScale * timeFactor;    
                    }
                }
            }


            /* for (int j=0; j<ParticleCount; j++)
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
            } */
        }
        
        for(int i = 0; i < ParticleCount; i++)
        {
            positions[i] += velocitys[i] * timeFactor; //Time.deltaTime; timefactor
            HandleBoundsCollisions(i);
        }

        int curKey = getCellKey(positions[curspat]);
        for(int i = 0; i < ParticleCount; i++)
        {
            //To-Do: add debug feature for sector highlightning. block.SetColor("_Color", getCellKey(positions[i]) == curKey ? Color.red : colors[i%colors.Length]);
            block.SetColor("_Color", getCellKey(positions[i]) == curKey ? Color.red : colors[i%colors.Length]); //colors[i%colors.Length]
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
- Add an GUI / Interface 
# Devlog 5: Speed up thanks to Spatial Partitioning
So now I've done it, the next thing on my To-Do list, as seen in my [last Devlog](https://stardance.hackclub.com/projects/14075/devlogs/18862) was to *Optimize and Improve calculations*, and I did that, mostly by implementing Spatial Partitioning.
How it works is that, if there is only a certain range around an particle, in this case the MaxRange, you can pretty much ignore everything outside of this range when calculating attraction, instead of having to check the distance to each particle, and getting the same result. To do that you split the area where the particles can be into squares(Cells) with length of MaxRange, and then assinging each particle a key, which you get like this:
```key = someHashFunction(CellPos.x, CellPos.y) % ParticleAmount```
And then with some sorting and such, you can calculate the attractions, only the Cell the particles in, and the 8 cells around it.

This was somewhat simplified, but I hope one could understand it.

To-Do:
- */