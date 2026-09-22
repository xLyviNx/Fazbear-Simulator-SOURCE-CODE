using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LyviN.Player.Build;

public class CustomizeColorEntry : MonoBehaviour
{
    public TMP_InputField HexInput;
    public Slider RS;
    public Slider GS;
    public Slider BS;
    public TextMeshProUGUI rgbText;
    public float R;
    public float G;
    public float B;
    public bool isPropColor;
    public Color currColor;
    private void Update()
    {
        if (!HexInput.isFocused)
        {
            R = (float)System.Math.Round(RS.value, 2);
            G = (float)System.Math.Round(GS.value, 2);
            B = (float)System.Math.Round(BS.value, 2);
            rgbText.text = $"rgb({R},{G},{B})";
            currColor = new Color(R, G, B);
            HexInput.text = "#" + ColorUtility.ToHtmlStringRGB(currColor);
            ColorBlock cb = new ColorBlock();
            cb.normalColor = currColor;
            cb.highlightedColor = currColor - Color.white * 0.3f;
            cb.pressedColor = currColor + Color.white * 0.2f;
            cb.selectedColor = currColor - Color.white * 0.15f;
            cb.colorMultiplier = 1f;
            HexInput.colors = cb;
        }

    }
    public void SetInputHEX()
    {
        Color c = hexToColor(HexInput.text.ToUpper().Trim());
        RS.value = c.r;
        GS.value = c.g;
        BS.value = c.b;
    }
    public static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace("#", "");
        float r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber)/255f;
        float g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber)/255f;
        float b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)/255f;
        return new Color(r, g, b);
    }
    public void ApplyPreviewEntity()
    {
        if (CatalogueMain.Instance != null)
        {
            if (CatalogueMain.Instance.Category == CatalogueMain.Categories.Customize && isPropColor)
            {
                CataloguePreviewEntity cpe = GameObject.FindObjectOfType<CataloguePreviewEntity>();
                if (cpe != null)
                {
                    cpe.SetColors();
                }
            }
        }
    }
}
