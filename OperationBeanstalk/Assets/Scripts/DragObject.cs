using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    void OnMouseDown()
    {
        Debug.Log("mouse down");
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;

        return Camera.main.ScreenToViewportPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        Debug.Log("mouse drag");
        transform.position = GetMouseWorldPos() + mOffset;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel"));
    }

    // void OnMouse
}
