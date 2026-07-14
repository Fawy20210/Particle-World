using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class SpacialLookuptest : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    
    public int ParticleCount;
    public float ParticleScale;

    //public float gravity;
    
    public float ParticleSpacing;

    public float minRandomMinRange;
    public float maxRandomMinRange;
    public float maxRandomMaxRange;
    public Color[] colors;


    public Vector2 boundSize;
    Vector2[] positions;
    Vector2[] velocitys;

    int2[] spatialLookUp;
    int[] startIndices;
    
    MaterialPropertyBlock block;
    RenderParams rp;
    public int curspat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int hashfunc(int x, int y)
    {
        return (x*2377+y*607)%ParticleCount;
    }

    void updateSpatialLookup()
    {
        Parallel.For(0, positions.Length, i =>
        {
            int cellkey = hashfunc(Mathf.FloorToInt(positions[i].x/maxRandomMaxRange), Mathf.FloorToInt(positions[i].y/maxRandomMaxRange));
            spatialLookUp[i] = new int2(cellkey, i);
            startIndices[i] = -1;
        });

        Array.Sort(spatialLookUp, (a,b) => a.x.CompareTo(b.x));

        Parallel.For(0, positions.Length, i =>
        {
            int key = spatialLookUp[i].x;
            int keyprev = (i == 0) ? -1 : spatialLookUp[i-1].x;

            if(key != keyprev)
            {
                startIndices[key] = i;
            }
        });  
    }
    void Start()
    {   
        positions = new Vector2[ParticleCount*10];
        velocitys = new Vector2[ParticleCount*10];
        spatialLookUp = new int2[ParticleCount];
        startIndices = new int[ParticleCount];

        colors = new Color[ParticleCount];


        int particlesPerRow = (int)math.sqrt(ParticleCount);
        int particlesPerCol = (ParticleCount -1) / particlesPerRow + 1;
        float spacing = ParticleScale + ParticleSpacing;

        for(int i = 0; i < ParticleCount; i++)
        {
            float x = (i % particlesPerRow - particlesPerRow / 2f + 0.5f) * spacing;
            float y = (i / particlesPerCol - particlesPerCol / 2f + 0.5f) * spacing;

            positions[i] = new Vector2(x,y);
            /* spatialLookUp[i] = hashfunc(Mathf.FloorToInt(x/maxRandomMaxRange), Mathf.FloorToInt(y/maxRandomMaxRange)); */
        }   
        updateSpatialLookup();
        
        block = new MaterialPropertyBlock();
        rp = new RenderParams(material)
        {
            matProps = block
        };
    }

    // Update is called once per frame
    void Update()
    {   
        int hash = hashfunc(Mathf.FloorToInt(positions[curspat].x/maxRandomMaxRange), Mathf.FloorToInt(positions[curspat].y/maxRandomMaxRange));
        for(int i = 0; i < ParticleCount; i++)
        {
            if (i == curspat)
            {
                colors[i]=Color.red;
            } else if(hashfunc(Mathf.FloorToInt(positions[i].x/maxRandomMaxRange), Mathf.FloorToInt(positions[i].y/maxRandomMaxRange)) == hash)
            {
                colors[i]=Color.blue;
            }
            else
            {
                colors[i]=Color.brown;
            }
        }


        for(int i = 0; i < ParticleCount; i++)
        {
            block.SetColor("_Color", colors[i]);
            Vector2 scale = new Vector2(ParticleScale,ParticleScale);
            Graphics.RenderMesh(rp, mesh, 0, Matrix4x4.TRS(positions[i], Quaternion.Euler(new Vector2(0,0)), scale)); //new Vector3(-4.5f, 0.0f, 5.0f)
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector2.zero, boundSize);
    }
}
