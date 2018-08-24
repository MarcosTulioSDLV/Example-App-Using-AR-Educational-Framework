using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickElement : MonoBehaviour, IPointerDownHandler {

    private bool onClick = false;
    public bool OnClick { get {return onClick;} set{onClick = value;}}

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();

        //elemento pressionado
        OnClick = true;
    }
}
