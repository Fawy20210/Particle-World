using UnityEngine;

public class HandleOthers : MonoBehaviour
{

    public float detectionRadius;
    public float minRadius;
    public float force_mutliplier;

    Rigidbody2D rigid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }
    

    // Update is called once per frame
    void Update()
    {
        Collider2D[] particles = Physics2D.OverlapCircleAll(
            transform.position, 
            detectionRadius          
        );
        

        foreach (Collider2D particle in particles)
        {
            if (particle.gameObject != gameObject)
            {
                Vector3 direction = transform.position - particle.transform.position;
                float sqrdistance = direction.sqrMagnitude;
                Debug.Log((direction, direction.normalized, sqrdistance, sqrdistance >= minRadius * minRadius));
                if(sqrdistance >= minRadius * minRadius)
                {
                    rigid.AddForce(direction.normalized * force_mutliplier);
                }
                
            }
        }
    }
}
