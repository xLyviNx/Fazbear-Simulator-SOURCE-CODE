using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOnValueChange : MonoBehaviour
{
    public Slider mySlider;
    public TMP_InputField myInput;

    private void Update()
    {
        
    }
    public void ChangeInput()
    {
        float hm = float.Parse(myInput.text);
        if (hm > 0) {
            mySlider.value = hm;
        }
        else
        {
            myInput.text = mySlider.value +"";
        }
    }
    public void ChangeSlider()
    {
        float hm = float.Parse(myInput.text);
        if (mySlider.value > 0)
        {
            myInput.text = mySlider.value + "";
        }
        else if (hm>0)
        {
            mySlider.value = hm;
        }
    }
    public void ChangeSliderOffset()
    {
        mySlider.value = (float)(Math.Round(mySlider.value, 2));

        float hm = (float)Math.Round(float.Parse(myInput.text), 2);
        if (mySlider.value >= -0.4 && mySlider.value <= 0.4)
        {
            myInput.text = mySlider.value + "";
        }
        else if (hm >= -0.4 && hm <= 0.4)
        {
            mySlider.value = hm;
        }
    }
    public void ChangeInputOffset()
    {
        mySlider.value = (float)(Math.Round(mySlider.value, 2));
        float hm = (float)Math.Round(float.Parse(myInput.text), 2);
        if (hm >= -0.4 && hm <= 0.4)
        {
            mySlider.value = hm;
        }
        else
        {
            myInput.text = mySlider.value + "";
        }
    }

    public void ChangeSliderIntensityRange()
    {
        mySlider.value = (float)(Math.Round(mySlider.value, 2));

        float hm = (float)Math.Round(float.Parse(myInput.text), 2);
        if (mySlider.value >= 0f && mySlider.value <= 50f)
        {
            myInput.text = mySlider.value + "";
        }
        else if (hm >= 0f && hm <= 50f)
        {
            mySlider.value = hm;
        }
    }
    public void ChangeInputIntensityRange()
    {
        mySlider.value = (float)(Math.Round(mySlider.value, 2));
        float hm = (float)Math.Round(float.Parse(myInput.text), 2);
        if (hm >= 0f && hm <= 50f)
        {
            mySlider.value = hm;
        }
        else
        {
            myInput.text = mySlider.value + "";
        }
    }
    public void ChangeInputH()
    {
        mySlider.value = (float)(Math.Round(mySlider.value, 2));
        float hm = (float)Math.Round(float.Parse(myInput.text), 2);
        if (hm >= 6f && hm <= 24f)
        {
            mySlider.value = hm;
        }
        else
        {
            myInput.text = mySlider.value + "";
        }
    }
    public void ChangeSliderH()
    {
        mySlider.value = Mathf.RoundToInt(mySlider.value);
        float hm = Mathf.RoundToInt(float.Parse(myInput.text));
        if (mySlider.value >= 6f && mySlider.value <= 24f)
        {
            myInput.text = mySlider.value + "";
        }
        else if (hm >= 6f && hm <= 24f)
        {
            mySlider.value = hm; // chyba jakims chujem to sie odpierdala
        }
    }
}
