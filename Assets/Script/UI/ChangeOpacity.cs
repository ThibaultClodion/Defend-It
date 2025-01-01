using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeOpacity : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Color transparentColor;
    private Color opaqueColor;
    [SerializeField] TextMeshProUGUI textMeshPro;

    public void Start()
    {
        transparentColor = new Color(255f, 255f, 255f, 0.5f);
        opaqueColor = new Color(255f, 255f, 255f, 1f);

        //Initially transparent
        GetComponent<Image>().color = transparentColor;
        textMeshPro.color = transparentColor;

    }

    //Make the select button Opaque
    public void OnSelect(BaseEventData eventData)
    {
        GetComponent<Image>().color = opaqueColor;
        textMeshPro.color = opaqueColor;
    }

    //Make deselect button transparent
    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Image>().color = transparentColor;
        textMeshPro.color = transparentColor;
    }

    //Make disable Button transparent
    private void OnDisable()
    {
        GetComponent<Image>().color = transparentColor;
        textMeshPro.color = transparentColor;
    }
}
