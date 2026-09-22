using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
 
[RequireComponent(typeof(TMP_Text))]
public class TMProLinks : MonoBehaviour, IPointerClickHandler
{
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().text = GetComponent<TextMeshProUGUI>().text.Replace("<link=", "<u><color=#f0be37><link=").Replace("</link>", "</link></color></u>");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, null);
        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }

}