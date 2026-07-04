using UnityEditor;
using UnityEngine;

public class HandleForces : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public float gravity;
    public float attraction_scale;
    public float spacing;
    public int Particle_Count;
    Vector2[] positions;
    Vector2[] velocitys;

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

    void Start()
    {
        positions = new Vector2[Particle_Count];
        velocitys = new Vector2[Particle_Count];

        for(int i = 0; i < Particle_Count; i++)
        {
            positions[i] = new Vector2(i*2,i);
        }
        
        rp = new RenderParams(material);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < Particle_Count; i++)
        {
            //velocitys[i] += Vector2.down * gravity * Time.deltaTime;
            velocitys[i] += calcForce(i) * Time.deltaTime;
            positions[i] += velocitys[i] * Time.deltaTime;
        }

        for(int i = 0; i < Particle_Count; i++)
        {
            Graphics.RenderMesh(rp, mesh, 0, Matrix4x4.Translate(positions[i])); //new Vector3(-4.5f, 0.0f, 5.0f)
        }
    }
}
