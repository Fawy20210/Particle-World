using TMPro;
using UnityEngine;

public class CheckInputDetails : MonoBehaviour
{
    public TMP_InputField MinInput;
    public TMP_InputField MidInput;
    public TMP_InputField MaxInput;
    
    public void KeepAboveZeroInt(TMP_InputField inputField)
    {
        int value;
        if (!int.TryParse(inputField.text, out value))
        {
            inputField.text = "1";
        } else if (value < 1)
        {
            inputField.text = "1";
        }
    }
    public void KeepAboveZeroFloat(TMP_InputField inputField)
    {
        float value;
        if (!float.TryParse(inputField.text, out value))
        {
            inputField.text = "1";
        } else if (value <= 0.0f)
        {
            inputField.text = "1";
        }
    }
    public void KeepRangeRules(TMP_InputField inputField)
    {
        int value;
        if (!int.TryParse(inputField.text, out value))
        {
            inputField.text = "1";
        }
        else
        {
            int MinValue = int.Parse(MinInput.text);
            int MidValue = int.Parse(MidInput.text);
            int MaxValue = int.Parse(MaxInput.text);
            if (inputField == MinInput)
            {
                if(value < 0)
                {
                    if(MidValue != 1)
                    {
                        MinInput.text = "1";
                    } 
                    else if(MaxValue != 2)
                    {
                        MidInput.text = "2";
                        MinInput.text = "1";
                    }
                    else
                    {
                        MaxInput.text = "3";
                        MidInput.text = "2";
                        MinInput.text = "1";
                    }
                } 
                else if(MidValue <= value)
                {
                    if(1 != MidValue)
                    {
                        MinInput.text = (MidValue-1).ToString();
                    } 
                    else if(MaxValue != 2)
                    {
                        MidInput.text = "2";
                        MinInput.text = "1";
                    }
                    else
                    {
                        MaxInput.text = "3";
                        MidInput.text = "2";
                        MinInput.text = "1";
                    }
                }
            } 
            else if (inputField == MidInput)
            {
                if(value < MinValue)
                {
                    if(MaxValue != MinValue+1)
                    {
                        MidInput.text = (MinValue+1).ToString();
                    } 
                    else
                    {
                        MaxInput.text = (MinValue+2).ToString();
                        MidInput.text = (MinValue+1).ToString();
                    }
                } else if(MaxValue <= value)
                {
                    if(MinValue < MaxValue - 1)
                    {
                        MidInput.text = (MaxValue-1).ToString(); 
                    }
                    else
                    {
                        MaxInput.text = (MinValue+2).ToString();
                        MidInput.text = (MinValue+1).ToString();
                    }
                }
            }
            else
            {
                if(value < MidValue)
                {
                    MaxInput.text = (MidValue+1).ToString();
                }
            }
        }
    }

    
}
