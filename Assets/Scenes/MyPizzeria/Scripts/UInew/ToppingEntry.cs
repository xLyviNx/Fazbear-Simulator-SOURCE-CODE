using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static LyviN.Player.Baking.Pizza;

namespace LyviN.UI
{
    public class ToppingEntry : MonoBehaviour
    {
        public TextMeshProUGUI myText;
        public int howmany;
        public PossibleToppings me;

        public void SelectMe()
        {
            if (me != PossibleToppings.Unknown)
            {
                if (StructureInteraction.Instance != null)
                {

                    if (StructureInteraction.Instance.isOn)
                    {
                        StructureInteraction.Instance.ToppingNameText.text = myText.text;
                        StructureInteraction.Instance.selectedTop = me;
                        StructureInteraction.Instance.selectedTopAm = howmany;
                        StructureInteraction.Instance.FillAmountSlider.maxValue = Mathf.Clamp(30, 1, howmany);
                    }
                }
            }
        }
    }
}