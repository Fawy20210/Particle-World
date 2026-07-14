using System;
using TMPro;
using UnityEngine;

public class CheckInputMatrixes : MonoBehaviour
{
    public ComputeCPU computeCPU;
    float MinValue;
    float MidValue;
    float MaxValue;

    void Start()
    {
        MinValue = computeCPU.minRandomMinRange;
        MidValue = computeCPU.maxRandomMinRange;
        MaxValue = computeCPU.maxRandomMaxRange;
    }
    public void UpdateValues()
    {
        MinValue = computeCPU.minRandomMinRange;
        MidValue = computeCPU.maxRandomMinRange;
        MaxValue = computeCPU.maxRandomMaxRange;
    }
    public void DecideType(TMP_InputField inputField)
    {
        switch (inputField.name[0])
        {
            case '0':
                {
                    checkValueAttraction(inputField);
                    break;
                }
            case '1':
                {
                    checkValueMinRanges(inputField);
                    break;
                }
            case '2':
                {
                    checkValueMaxRanges(inputField);
                    break;
                }
        }
    }
    public void checkValueAttraction(TMP_InputField inputField)
    {
        float value;
        if (!float.TryParse(inputField.text, out value))
        {
            inputField.text="0";
        }
    }
    public void checkValueMinRanges(TMP_InputField inputField)
    {
        float value;
        if (!float.TryParse(inputField.text, out value))
        {
            value = MinValue;
        }

        if (value < MinValue)
        {
            value = MinValue;
        } else if(value > MidValue)
        {
            value = MidValue;
        }

        inputField.text = value.ToString();
    }
    public void checkValueMaxRanges(TMP_InputField inputField)
    {
        float value;
        if (!float.TryParse(inputField.text, out value))
        {
            value = MidValue;
        }

        if (value < MidValue)
        {
            value = MidValue;
        } else if(value > MaxValue)
        {
            value = MaxValue;
        }

        inputField.text = value.ToString();
    }
    public string GetValidMinRangesValue(string text)
    {
        float value;
        if (!float.TryParse(text, out value))
        {
            value = MinValue;
        } 

        if (value < MinValue)
        {
            value = MinValue;
        } else if(value > MidValue)
        {
            value = MidValue;
        }
        return value.ToString();
    }
    public string GetValidMaxRangesValue(string text)
    {
        float value;
        if (!float.TryParse(text, out value))
        {
            value = MidValue;
        }

        if (value < MidValue)
        {
            value = MidValue;
        } else if(value > MaxValue)
        {
            value = MaxValue;
        }
        return value.ToString();
    }
    /* public void checkValueInt(int MaxValue, int MinValue, TMP_InputField inputField)
    {
        int value = int.Parse(inputField.text);

        if (value < MinValue)
        {
            value = MinValue;
        } else if(value > MaxValue)
        {
            value = MaxValue;
        } else
        {
            return;
        }

        inputField.text = value.ToString();
    } */
}
