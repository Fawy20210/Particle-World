using System;
using Unity.Mathematics;
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

    public int A,B,C;

    // Get the ID of all buffers and values
    static int positionsId = Shader.PropertyToID("_positions");
    static int velocitiesId = Shader.PropertyToID("_Velocities");
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




    void OnEnable()
    {   
        updateSpatialLookupId = computeShader.FindKernel("updateSpatialLookup");
        updateStartIndicesId = computeShader.FindKernel("updateStartIndices");
        updateVelocitiesId = computeShader.FindKernel("updateVelocities");
        updatePositionId = computeShader.FindKernel("updatePosition");


        int colorCountSqr = colors.Length * colors.Length;

        positionsBuffer = new ComputeBuffer(ParticleCount, sizeof(float) * 2);
        velocitiesBuffer = new ComputeBuffer(ParticleCount, sizeof(float) * 2);

        AttractionMatrixBuffer = new ComputeBuffer(colorCountSqr, sizeof(float));

        minRangeBuffer = new ComputeBuffer(colorCountSqr, sizeof(float));
        maxRangeBuffer = new ComputeBuffer(colorCountSqr, sizeof(float));

        spatialLookUpBuffer = new ComputeBuffer(ParticleCount, sizeof(int) * 2);
        startIndicesBuffer = new ComputeBuffer(ParticleCount, sizeof(int));

        startIndicesBuffer = new ComputeBuffer(ParticleCount, sizeof(int));

        colorsBuffer = new ComputeBuffer(colors.Length, sizeof(float) * 4);

        computeShader.SetBuffer(updateSpatialLookupId, positionsId, positionsBuffer);
        computeShader.SetBuffer(updateSpatialLookupId, spatialLookUpId, spatialLookUpBuffer);
        computeShader.SetBuffer(updateSpatialLookupId, startIndicesId, startIndicesBuffer);

        computeShader.SetBuffer(updateStartIndicesId, spatialLookUpId, spatialLookUpBuffer);
        computeShader.SetBuffer(updateStartIndicesId, startIndicesId, startIndicesBuffer);

        computeShader.SetBuffer(updateVelocitiesId, positionsId, positionsBuffer);
        computeShader.SetBuffer(updateVelocitiesId, velocitiesId, velocitiesBuffer);
        computeShader.SetBuffer(updateVelocitiesId, AttractionMatrixId, AttractionMatrixBuffer);
        computeShader.SetBuffer(updateVelocitiesId, minRangeId, minRangeBuffer);
        computeShader.SetBuffer(updateVelocitiesId, maxRangeId, maxRangeBuffer);
        computeShader.SetBuffer(updateVelocitiesId, spatialLookUpId, spatialLookUpBuffer);
        computeShader.SetBuffer(updateVelocitiesId, startIndicesId, startIndicesBuffer);

        /* computeShader.SetBuffer(updatePositionId, positionsId, positionsBuffer);
        computeShader.SetBuffer(updatePositionId, velocitiesId, velocitiesBuffer);


        computeShader.SetBuffer(updatePositionId,velocitiesId, velocitiesBuffer);

        computeShader.SetBuffer(0,AttractionMatrixId, AttractionMatrixBuffer);

        computeShader.SetBuffer(0,minRangeId, minRangeBuffer);
        computeShader.SetBuffer(0,maxRangeId, maxRangeBuffer);

        computeShader.SetBuffer(0,spatialLookUpId, spatialLookUpBuffer);
        computeShader.SetBuffer(0,startIndicesId, startIndicesBuffer); */

        computeShader.SetInt(particleCountId, ParticleCount);
        computeShader.SetFloat(frictionFactorId, frictionFactor);
        computeShader.SetFloat(timeFactorId, timeFactor);
        computeShader.SetFloat(forceScaleId, forceScale);
        computeShader.SetFloat(colorCountId, colors.Length);
        computeShader.SetFloat(maxRandomMaxRangeId, maxRandomMaxRange);
        computeShader.SetVector(boundSizeId, boundSize);


        Vector2[] positions = new Vector2[ParticleCount];
        float[] attractionMatrix = new float[colorCountSqr];
        float[] minRange = new float[colorCountSqr];
        float[] maxRange = new float[colorCountSqr];

        int particlesPerRow = (int)math.sqrt(ParticleCount) + 1;
        int particlesPerCol = (ParticleCount -1) / particlesPerRow + 1;
        float spacing = ParticleScale + ParticleSpacing;

        for(int i = 0; i < ParticleCount; i++)
        {
            float x = (i % particlesPerRow - particlesPerRow / 2 + 0.5f) * spacing;
            float y = (i / particlesPerCol - particlesPerCol / 2f + 0.5f) * spacing;

            positions[i] = new Vector2(x,y);
        }


        for(int i=0; i<colors.Length; i++)
        {
            for(int j=0; j<colors.Length; j++)
            {
                int index = i + j * colors.Length;
                attractionMatrix[index] = UnityEngine.Random.Range(-1f, 1f);
                minRange[index] = UnityEngine.Random.Range(minRandomMinRange, maxRandomMinRange);
                maxRange[index] = UnityEngine.Random.Range(maxRandomMinRange, maxRandomMaxRange);
            }
        }
        positionsBuffer.SetData(positions);
        AttractionMatrixBuffer.SetData(attractionMatrix);
        minRangeBuffer.SetData(minRange);
        maxRangeBuffer.SetData(maxRange);
        colorsBuffer.SetData(colors);

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
        sortShader.SetBuffer(0, "Values", spatialLookUpBuffer);
        sortShader.SetInt("_numValues", ParticleCount);


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

                sortShader.Dispatch(0,A,B,C);
            }
        }
    }

    void Update()
    {

        runSiumulationStep();
        RenderParams rp = new RenderParams(material);
        rp.worldBounds = new Bounds(Vector3.zero, 10000*Vector3.one); // use tighter bounds
        rp.matProps = new MaterialPropertyBlock();
        rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.Translate(new Vector3(-4f, 0, 0)));
        rp.matProps.SetFloat("_NumInstances", (float)ParticleCount);
        rp.matProps.SetBuffer("_positions", positionsBuffer);
        rp.matProps.SetBuffer("_colors", colorsBuffer);
        rp.matProps.SetFloat("_particleScale", ParticleScale);
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
}