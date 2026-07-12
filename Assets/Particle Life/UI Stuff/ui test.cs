using System;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class uitest : MonoBehaviour
{
    public ComputeCPU computeCPU;
    public Transform ColorParent;
    public GameObject ColorObjects;
    public MatrixLayout AttractionMatrixLayout;

    public TMP_InputField ParticleCountInput;
    public TMP_InputField minRandomMinRangeInput;
    public TMP_InputField maxRandomMinRangeInput;
    public TMP_InputField maxRandomMaxRangeInput;
    public TMP_InputField ColorCountInput;
    public TMP_InputField boundsXInput;
    public TMP_InputField boundsYInput;

    public TMP_InputField frictionFactorInput;
    public TMP_InputField timeFactorInput;
    public TMP_InputField forceScaleInput;

    public TMP_InputField ParticleScaleInput;

    public Transform LeftCollapsable;
    public Transform LeftCollapseButtonImage;
    bool IsLeftCollapsed = false;
    void Awake()
    {
        computeCPU.ParticleCount = int.Parse(ParticleCountInput.text);
        computeCPU.minRandomMinRange = int.Parse(minRandomMinRangeInput.text);
        computeCPU.maxRandomMinRange = int.Parse(maxRandomMinRangeInput.text);
        computeCPU.maxRandomMaxRange = int.Parse(maxRandomMaxRangeInput.text);
        computeCPU.colorCount = int.Parse(ColorCountInput.text);
        computeCPU.boundSize.x = Screen.width*float.Parse(boundsXInput.text);
        computeCPU.boundSize.y = Screen.height*float.Parse(boundsYInput.text);
        computeCPU.ParticleScale = float.Parse(ParticleScaleInput.text);
        computeCPU.updateRendering = true;
        computeCPU.frictionFactor = float.Parse(frictionFactorInput.text);
        computeCPU.timeFactor = float.Parse(timeFactorInput.text);
        computeCPU.forceScale = float.Parse(forceScaleInput.text);
        computeCPU.enabled = true;
    }
    void Start()
    {
        updateUIColors();
    }

    void updateUIColors()
    {
        Color[] colors = computeCPU.colors;
        foreach(Transform child in ColorParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Color color in colors)
        {
            GameObject newItem = Instantiate(ColorObjects, ColorParent);
            //newItem.transform.SetParent(parent);
            Image image = newItem.GetComponentInChildren<Image>();
            TMP_InputField text = newItem.GetComponent<TMP_InputField>();

            if (image != null)
            {
                image.color=color;
            }
            if (text != null)
            {
                //Debug.Log((color, text));
                text.text="#"+color.ToHexString()[..6];
            }
        }
        AttractionMatrixLayout.UpdateLayout();
    }



    public void ColorsUpdate()
    {
        Color[] colors = computeCPU.colors;
        TMP_InputField[] texts = ColorParent.GetComponentsInChildren<TMP_InputField>();
        Image[] image = ColorParent.GetComponentsInChildren<Image>();

        for(int i=0; i<colors.Length; i++)
        {
            Color newCol;
            if(ColorUtility.TryParseHtmlString(texts[i].text+"ff", out newCol))
            {
                colors[i]=newCol;
                image[i].color=newCol;
            }
            else
            {
                colors[i]=Color.white;
                texts[i].text = "Not a valid color";
            }
            //colors[i]=texts[i].text;
        }
        computeCPU.updateRendering=true;
        updateUIColors();
    }

    public void SizeUpdate()
    {
        computeCPU.ParticleScale = float.Parse(ParticleScaleInput.text);
        computeCPU.updateRendering = true;
    }

    public void PauseUpdate()
    {
        if(computeCPU.pause){
            computeCPU.pause=false;
        }
        else
        {
            computeCPU.pause=true;
        }
    }
    public void PhysicsUpdate()
    {
        computeCPU.frictionFactor = float.Parse(frictionFactorInput.text);
        computeCPU.timeFactor = float.Parse(timeFactorInput.text);
        computeCPU.forceScale = float.Parse(forceScaleInput.text);
        computeCPU.update=true;
    }
    public void Reload()
    {
        computeCPU.ParticleCount = int.Parse(ParticleCountInput.text);
        computeCPU.minRandomMinRange = int.Parse(minRandomMinRangeInput.text);
        computeCPU.maxRandomMinRange = int.Parse(maxRandomMinRangeInput.text);
        computeCPU.maxRandomMaxRange = int.Parse(maxRandomMaxRangeInput.text);
        computeCPU.colorCount = int.Parse(ColorCountInput.text);
        computeCPU.boundSize.x = Screen.width*float.Parse(boundsXInput.text);
        computeCPU.boundSize.y = Screen.height*float.Parse(boundsYInput.text);
        PhysicsUpdate();


        computeCPU.enabled = false;
        computeCPU.enabled = true;
        updateUIColors();
    }

    public void CollapseLeft()
    {
        if (IsLeftCollapsed)
        {
            LeftCollapsable.position += new Vector3(300,0,0);
            LeftCollapseButtonImage.Rotate(0,0,180);
            IsLeftCollapsed = false;
        }
        else
        {
            LeftCollapsable.position -= new Vector3(300,0,0);
            LeftCollapseButtonImage.Rotate(0,0,180);
            IsLeftCollapsed = true;
        }
    }
}
