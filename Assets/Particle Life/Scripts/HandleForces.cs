using UnityEngine;

public class HandleForces : MonoBehaviour
{
    GameObject[] g;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        g = GameObject.FindGameObjectsWithTag("Particle");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(g);
    }
}
