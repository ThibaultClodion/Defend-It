using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstSelected : MonoBehaviour
{
    //Change which UI button is selected
    public void ChangeSelect(GameObject button)
    {
        this.GetComponent<EventSystem>().SetSelectedGameObject(button);
    }

}
