using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class OpenHyperLinks : MonoBehaviour, IPointerClickHandler
{
    TextMeshProUGUI pTextMeshPro;
    Camera pCamera;

    private void Start()
    {
        pTextMeshPro = GetComponent<TextMeshProUGUI>();
        pCamera = Camera.main;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, Input.mousePosition, pCamera);
        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

            // open the link id as a url, which is the metadata we added in the text field
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
