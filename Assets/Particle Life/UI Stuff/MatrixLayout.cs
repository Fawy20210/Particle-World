using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MatrixLayout : MonoBehaviour
{
    public ComputeCPU computeCPU;
    public CheckInputMatrixes checkInputMatrixes;
    public GameObject ContentFieldObject;
    public GameObject ColorFieldObject;

    public TMP_InputField AttractionEditField;
    public GridLayoutGroup TopColorsGridAttraction;
    public GridLayoutGroup LeftColorsGridAttraction;
    public GridLayoutGroup ContentGridAttraction;

    public TMP_InputField MinEditField;
    public GridLayoutGroup TopColorsGridMin;
    public GridLayoutGroup LeftColorsGridMin;
    public GridLayoutGroup ContentGridMin;

    public TMP_InputField MaxEditField;
    public GridLayoutGroup TopColorsGridMax;
    public GridLayoutGroup LeftColorsGridMax;
    public GridLayoutGroup ContentGridMax;

    TMP_InputField CurrentlySelectedAttraction;
    TMP_InputField CurrentlySelectedMin;
    TMP_InputField CurrentlySelectedMax;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLayout()
    {
        int ColorCount = computeCPU.colorCount;
        float size = 250.0f/(ColorCount+1.0f);

        TopColorsGridAttraction.padding.left = (int)(size+size/20);
        LeftColorsGridAttraction.padding.top = (int)(size+size/20);
        TopColorsGridMin.padding.left = (int)(size+size/20);
        LeftColorsGridMin.padding.top = (int)(size+size/20);
        TopColorsGridMax.padding.left = (int)(size+size/20);
        LeftColorsGridMax.padding.top = (int)(size+size/20);

        Vector2 CellSize = new Vector2(size,size);
        GridColors(TopColorsGridAttraction,CellSize);
        GridColors(LeftColorsGridAttraction,CellSize);
        ContentGridAttraction.cellSize = CellSize;
        GridColors(TopColorsGridMin,CellSize);
        GridColors(LeftColorsGridMin,CellSize);
        ContentGridMin.cellSize = CellSize;
        GridColors(TopColorsGridMax,CellSize);
        GridColors(LeftColorsGridMax,CellSize);
        ContentGridMax.cellSize = CellSize;
        
        TopColor(TopColorsGridAttraction.transform, size);
        TopColor(TopColorsGridMin.transform, size);
        TopColor(TopColorsGridMax.transform, size);
        LeftColor(LeftColorsGridAttraction.transform, size);
        LeftColor(LeftColorsGridMin.transform, size);
        LeftColor(LeftColorsGridMax.transform, size);
        Content(ContentGridAttraction.transform, computeCPU.attractionMatrix, size, "0 ");
        Content(ContentGridMin.transform, computeCPU.minRange, size, "1 ");
        Content(ContentGridMax.transform, computeCPU.maxRange, size, "2 ");

        

    }
    void GridColors(GridLayoutGroup grid, Vector2 CellSize)
    {
        grid.cellSize = CellSize-CellSize/10;
        grid.spacing = CellSize/10;
    }
    
    void TopColor(Transform Parent, float size)
    {
        RectTransform rect = Parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(250.0f, size);
        
        Color[] colors = computeCPU.colors;
        foreach(Transform child in Parent)
        {
            Destroy(child.gameObject);
        }

        foreach (Color color in colors)
        {
            GameObject newItem = Instantiate(ColorFieldObject, Parent);
            Image image = newItem.GetComponent<Image>();
            image.color = color;
        }
        
    }
    void LeftColor(Transform Parent, float size)
    {
        RectTransform rect = Parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size, 250.0f);

        Color[] colors = computeCPU.colors;
        foreach(Transform child in Parent)
        {
            Destroy(child.gameObject);
        }

        foreach (Color color in colors)
        {
            GameObject newItem = Instantiate(ColorFieldObject, Parent);
            Image image = newItem.GetComponent<Image>();
            image.color = color;
        }
    }

    void Content(Transform Parent, float[] Matrix, float size, string num)
    {
        RectTransform rect = Parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(250.0f-size,250.0f-size);

        int ColorCount = computeCPU.colorCount;
        foreach(Transform child in Parent)
        {
            Destroy(child.gameObject);
        }
        
        for(int i=0; i<ColorCount*ColorCount; i++)
        {
            GameObject newItem = Instantiate(ContentFieldObject, Parent);
            newItem.name=num+i.ToString();
            TMP_InputField text = newItem.GetComponent<TMP_InputField>();
            text.text = Matrix[i].ToString();
            text.pointSize = size/5;
        }
    }

    public void UpdateAttractionValues()
    {
        int ColorCount = computeCPU.colorCount;
        TMP_InputField[] texts = ContentGridAttraction.transform.GetComponentsInChildren<TMP_InputField>();

        float[] Matrix = new float[ColorCount*ColorCount];

        for (int i=0; i<ColorCount*ColorCount; i++)
        {
            Matrix[i] = float.Parse(texts[i].text);
        }

        computeCPU.attractionMatrix = Matrix;
        computeCPU.updateAttractionMatrix = true;
    }
    public void UpdateMinValues()
    {
        int ColorCount = computeCPU.colorCount;
        TMP_InputField[] texts = ContentGridMin.transform.GetComponentsInChildren<TMP_InputField>();

        float[] Matrix = new float[ColorCount*ColorCount];

        for (int i=0; i<ColorCount*ColorCount; i++)
        {
            Matrix[i] = float.Parse(texts[i].text);
        }

        computeCPU.minRange = Matrix;
        computeCPU.updateMinRangeMatrix = true;
    }
    public void UpdateMaxValues()
    {
        int ColorCount = computeCPU.colorCount;
        TMP_InputField[] texts = ContentGridMax.transform.GetComponentsInChildren<TMP_InputField>();

        float[] Matrix = new float[ColorCount*ColorCount];
        float maxVal = 0.0f;

        for (int i=0; i<ColorCount*ColorCount; i++)
        {
            Matrix[i] = float.Parse(texts[i].text);
            if(maxVal<Matrix[i]) maxVal = Matrix[i];
        }

        computeCPU.maxRange = Matrix;
        computeCPU.maxRandomMaxRange = maxVal;
        computeCPU.updateMaxRangeMatrix = true;
    }
    
    public void ChangeEditField(TMP_InputField field)
    {
        //Debug.Log(field.text);
        switch (field.name[0])
        {
            case '0':
                {
                    CurrentlySelectedAttraction = field;
                    AttractionEditField.text = field.text;
                    break;
                }
            case '1':
                {
                    CurrentlySelectedMin = field;
                    MinEditField.text = field.text;
                    break;
                }
            case '2':
                {
                    CurrentlySelectedMax = field;
                    MaxEditField.text = field.text;
                    break;
                }
        }

    }
    public void UpdateAttractionField()
    {
        CurrentlySelectedAttraction.text = AttractionEditField.text;
    }
    public void UpdateMinField()
    {
        CurrentlySelectedMin.text = MinEditField.text;
    }
    public void UpdateMaxField()
    {
        CurrentlySelectedMax.text = MaxEditField.text;
    }
    public void UpdateAttractionFieldCheck()
    {
        if (AttractionEditField.text != "")
        {
            CurrentlySelectedAttraction.text = AttractionEditField.text;
        }
        else
        {
            CurrentlySelectedAttraction.text = "0";
            AttractionEditField.text = "0";
        }
    }
    public void UpdateMinFieldCheck()
    {
        string text = checkInputMatrixes.GetValidMinRangesValue(MinEditField.text);
        MinEditField.text = text;
        CurrentlySelectedMin.text = text;
    }
    public void UpdateMaxFieldCheck()
    {
        string text = checkInputMatrixes.GetValidMaxRangesValue(MaxEditField.text);
        MaxEditField.text = text;
        CurrentlySelectedMax.text = text;
    }


}
