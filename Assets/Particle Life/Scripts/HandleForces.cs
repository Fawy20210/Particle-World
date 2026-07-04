using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class HandleForces : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public float ParticleScale;
    //public float gravity;
    public float attraction_scale;
    public float ParticleSpacing;
    public float dampening;
    public int Particle_Count;
    public Color[] colors;
    public Vector2 boundSize;
    Vector2[] positions;
    Vector2[] velocitys;

    MaterialPropertyBlock block;
    RenderParams rp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Vector2 calcForce(int index)
    {
        Vector2 total_force = new Vector2();
        for (int i=0; i<Particle_Count; i++)
        {
            if(i != index)
            {
                Vector2 dir = positions[index]-positions[i];
                total_force += dir.normalized * attraction_scale;
            }
        }
        return total_force;
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
        positions = new Vector2[Particle_Count];
        velocitys = new Vector2[Particle_Count];

        int particlesPerRow = (int)math.sqrt(Particle_Count);
        int particlesPerCol = (Particle_Count -1) / particlesPerRow + 1;
        float spacing = ParticleScale + ParticleSpacing;

        for(int i = 0; i < Particle_Count; i++)
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
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < Particle_Count; i++)
        {
            //velocitys[i] += Vector2.down * gravity * Time.deltaTime;
            velocitys[i] += calcForce(i) * Time.deltaTime;
            /* positions[i] += velocitys[i] * Time.deltaTime;
            HandleBoundsCollisions(i); */
        }
        for(int i = 0; i < Particle_Count; i++)
        {
            positions[i] += velocitys[i] * Time.deltaTime;
            HandleBoundsCollisions(i);
        }

        for(int i = 0; i < Particle_Count; i++)
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
- Add the possibility for particles to have different colors. */