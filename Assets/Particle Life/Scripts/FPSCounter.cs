using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    TMP_Text text;
    float step = 0;
    int count = 0;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        step += Time.deltaTime;
        count++;
        if(step>=0.2f)
        {
            text.text = ((int)(1 / (step/count))).ToString() + " FPS";
            step = 0;
            count = 0;
        }
    }
}
