using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Embree;
using UnityEngine;
using UnityEngine.Rendering;

public class ComputeCPU : MonoBehaviour
{
    public ComputeShader computeShader;
    public ComputeShader sortShader;
    public Material material;
    public Mesh mesh;

    public int ParticleCount;
    public float ParticleScale;
    
    public float ParticleSpacing;
    public float dampening;

    public float frictionFactor;
    public float timeFactor;
    public float forceScale;

    public float minRandomMinRange;
    public float maxRandomMinRange;
    public float maxRandomMaxRange;
    public int colorCount;

    public Color[] colors;

    public Vector2 boundSize;

    

    //Buffers for Compute Shader
    ComputeBuffer positionsBuffer; //int2[]
    ComputeBuffer velocitiesBuffer; //int[]

    ComputeBuffer AttractionMatrixBuffer; //float[]

    ComputeBuffer minRangeBuffer; //float[]
    ComputeBuffer maxRangeBuffer; //float[]

    ComputeBuffer spatialLookUpBuffer; //int2[]
    ComputeBuffer startIndicesBuffer; //int[]

    //Buffers for normal shader
    ComputeBuffer colorsBuffer; //float4[]


    public bool update;
    public bool updateRendering;
    public bool pause;
    public int A,B,C;
    public float[] attractionMatrix;
    public float[] minRange;
    public float[] maxRange;
    public bool updateAttractionMatrix;
    public bool updateMinRangeMatrix;
    public bool updateMaxRangeMatrix;

    // Get the ID of all buffers and values
    static int positionsId = Shader.PropertyToID("_positions");
    static int velocitiesId = Shader.PropertyToID("_velocities");
    static int AttractionMatrixId = Shader.PropertyToID("_attractionMatrix");
    static int minRangeId = Shader.PropertyToID("_minRange");
    static int maxRangeId = Shader.PropertyToID("_maxRange");

    static int spatialLookUpId = Shader.PropertyToID("_spatialLookUp");
    static int startIndicesId = Shader.PropertyToID("_startIndices");
    static int particleCountId = Shader.PropertyToID("_particleCount");
    static int frictionFactorId = Shader.PropertyToID("_frictionFactor");
    static int timeFactorId = Shader.PropertyToID("_timeFactor");
    static int forceScaleId = Shader.PropertyToID("_forceScale");
    static int colorCountId = Shader.PropertyToID("_colorCount");
    static int maxRandomMaxRangeId = Shader.PropertyToID("_maxRandomMaxRange");
    static int boundSizeId = Shader.PropertyToID("_boundSize");

    //initiate Kerne Id values
    int updateSpatialLookupId;
    int updateStartIndicesId;
    int updateVelocitiesId;
    int updatePositionId;

    RenderParams rp;

    void BindAll(int k)
    {
        computeShader.SetBuffer(k, "_positions", positionsBuffer);
        computeShader.SetBuffer(k, "_velocities", velocitiesBuffer);
        computeShader.SetBuffer(k, "_attractionMatrix", AttractionMatrixBuffer);
        computeShader.SetBuffer(k, "_minRange", minRangeBuffer);
        computeShader.SetBuffer(k, "_maxRange", maxRangeBuffer);
        computeShader.SetBuffer(k, "_spatialLookUp", spatialLookUpBuffer);
        computeShader.SetBuffer(k, "_startIndices", startIndicesBuffer);
    }


    void OnEnable()
    {   
        updateSpatialLookupId = computeShader.FindKernel("updateSpatialLookup");
        updateStartIndicesId = computeShader.FindKernel("updateStartIndices");
        updateVelocitiesId = computeShader.FindKernel("updateVelocities");
        updatePositionId = computeShader.FindKernel("updatePosition");
        Debug.Log(updateSpatialLookupId);
        Debug.Log(updateStartIndicesId);
        Debug.Log(updateVelocitiesId);
        Debug.Log(updatePositionId);

        int colorCountSqr = colorCount * colorCount;

        positionsBuffer = new ComputeBuffer(ParticleCount, sizeof(float) * 2);
        velocitiesBuffer = new ComputeBuffer(ParticleCount, sizeof(float) * 2);

        AttractionMatrixBuffer = new ComputeBuffer(colorCountSqr, sizeof(float));

        minRangeBuffer = new ComputeBuffer(colorCountSqr, sizeof(float));
        maxRangeBuffer = new ComputeBuffer(colorCountSqr, sizeof(float));

        spatialLookUpBuffer = new ComputeBuffer(ParticleCount, sizeof(int) * 2);
        startIndicesBuffer = new ComputeBuffer(ParticleCount, sizeof(int));

        startIndicesBuffer = new ComputeBuffer(ParticleCount, sizeof(int));

        colorsBuffer = new ComputeBuffer(colorCount, sizeof(float) * 4);

        BindAll(updateSpatialLookupId);
        BindAll(updateStartIndicesId);
        BindAll(updateVelocitiesId);
        BindAll(updatePositionId);
        
        

        computeShader.SetInt(particleCountId, ParticleCount);
        computeShader.SetFloat(frictionFactorId, frictionFactor);
        computeShader.SetFloat(timeFactorId, timeFactor);
        computeShader.SetFloat(forceScaleId, forceScale);
        computeShader.SetFloat(colorCountId, colorCount);
        computeShader.SetFloat(maxRandomMaxRangeId, maxRandomMaxRange);
        computeShader.SetVector(boundSizeId, boundSize);
        computeShader.SetFloat("_particleScale", ParticleScale);

        sortShader.SetBuffer(0, "_values", spatialLookUpBuffer);
        sortShader.SetInt("_numValues", ParticleCount);

        Vector2[] positions = new Vector2[ParticleCount];
        attractionMatrix = new float[colorCountSqr];
        minRange = new float[colorCountSqr];
        maxRange = new float[colorCountSqr];

        /* int particlesPerRow = (int)math.sqrt(ParticleCount) + 1;
        int particlesPerCol = (ParticleCount -1) / particlesPerRow + 1;
        float spacing = ParticleScale + ParticleSpacing; */

        float minX = -boundSize.x/2;
        float maxX = boundSize.x/2;
        float minY = -boundSize.y/2;
        float maxY = boundSize.y/2;

        for(int i = 0; i < ParticleCount; i++)
        {
            float x = UnityEngine.Random.Range(minX,maxX); //(i % particlesPerRow - particlesPerRow / 2 + 0.5f) * spacing;
            float y = UnityEngine.Random.Range(minY,maxY); //(i / particlesPerCol - particlesPerCol / 2f + 0.5f) * spacing;

            positions[i] = new Vector2(x,y);
        }


        for(int i=0; i<colorCount; i++)
        {
            for(int j=0; j<colorCount; j++)
            {
                int index = i + j * colorCount;
                attractionMatrix[index] = UnityEngine.Random.Range(-1f, 1f);
                minRange[index] = UnityEngine.Random.Range(minRandomMinRange, maxRandomMinRange);
                maxRange[index] = UnityEngine.Random.Range(maxRandomMinRange, maxRandomMaxRange);
            }
        }

        if (colors.Length != colorCount)
        {
            Color[] temp = colors;
            colors = new Color[colorCount];
            
            for(int i=0; i<colorCount; i++)
            {
                if (i < temp.Length)
                {
                    colors[i]=temp[i];
                }
                else
                {
                    colors[i] = UnityEngine.Random.ColorHSV(0,1,1,1,1,1);
                }
            }
        }


        positionsBuffer.SetData(positions);
        AttractionMatrixBuffer.SetData(attractionMatrix);
        minRangeBuffer.SetData(minRange);
        maxRangeBuffer.SetData(maxRange);
        colorsBuffer.SetData(colors);
        A = ParticleCount/64;

        rp = new RenderParams(material);
        rp.worldBounds = new Bounds(Vector3.zero, 100000000*Vector3.one); // use tighter bounds
        rp.matProps = new MaterialPropertyBlock();
        rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.Translate(new Vector3(-4f, 0, 0)));
        rp.matProps.SetFloat("_NumInstances", (float)ParticleCount);
        rp.matProps.SetBuffer("_positions", positionsBuffer);
        rp.matProps.SetBuffer("_colors", colorsBuffer);
        rp.matProps.SetFloat("_particleScale", ParticleScale);

    }
    void OnDisable()
    {
        positionsBuffer.Dispose();
        velocitiesBuffer.Dispose();

        AttractionMatrixBuffer.Dispose();

        minRangeBuffer.Dispose();
        maxRangeBuffer.Dispose();

        spatialLookUpBuffer.Dispose();
        startIndicesBuffer.Dispose();

        colorsBuffer.Dispose();
    }

    void Sort()
    {
        int numPairs = Mathf.NextPowerOfTwo(ParticleCount)/2;
        int numStages = (int)Mathf.Log(numPairs *2, 2);

        for(int stageIndex = 0; stageIndex < numStages; stageIndex++)
        {
            for(int stepIndex = 0; stepIndex < stageIndex + 1; stepIndex++)
            {
                int groupWidth = 1 << (stageIndex - stepIndex);
                int groupHeight = 2 * groupWidth -1;
                sortShader.SetInt("_groupWidth", groupWidth);
                sortShader.SetInt("_groupHeight", groupHeight);
                sortShader.SetInt("_stepIndex", stepIndex);

                sortShader.Dispatch(0,ParticleCount/128,B,C);
            }
        }
    }

    void Update()
    {
        if (update)
        {
            computeShader.SetFloat(frictionFactorId, frictionFactor);
            computeShader.SetFloat(timeFactorId, timeFactor);
            computeShader.SetFloat(forceScaleId, forceScale);
            computeShader.SetVector(boundSizeId, boundSize);
            computeShader.SetFloat("_particleScale", ParticleScale);
            update = false;
        }
        if (updateRendering)
        {
            colorsBuffer.SetData(colors);
            rp.matProps.SetFloat("_particleScale", ParticleScale);
            updateRendering = false;
        }

        if (updateAttractionMatrix)
        {
            AttractionMatrixBuffer.SetData(attractionMatrix);
            updateAttractionMatrix = false;
        }
        if (updateMinRangeMatrix)
        {
            minRangeBuffer.SetData(minRange);
            updateMinRangeMatrix = false;
        }
        if (updateMaxRangeMatrix)
        {
            maxRangeBuffer.SetData(maxRange);
            computeShader.SetFloat(maxRandomMaxRangeId, maxRandomMaxRange);
            updateMaxRangeMatrix = false;
        }

        if (!pause) runSiumulationStep();
        
        Graphics.RenderMeshPrimitives(rp, mesh, 0, ParticleCount);
    }

    void runSiumulationStep()
    {
        computeShader.Dispatch(updateSpatialLookupId, A,B,C);
        Sort();
        computeShader.Dispatch(updateStartIndicesId, A,B,C);
        computeShader.Dispatch(updateVelocitiesId, A,B,C);
        computeShader.Dispatch(updatePositionId, A,B,C);
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector2.zero, boundSize);
    }

}
