using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This implementation has a notable flaw. If the camera isn't looking dead-on at the block,
 * the code here has trouble grabbing onto that object. I noticed it when I tried making the
 * drop view higher - so that the user could see more top-down and gauge the dropping location
 * - and it made it impossible to drag the block. Unfortunately, this implementation might
 * not be the best and we might have to go back to the old dragblock tool, moving objects
 * relative to the camera isn't working quite right. 
 * ...However I think the older drag tool was also using camera perspective as a raycast, 
 * so it's possible the issue has always existed. I don't love the framing of the camera and 
 * where it needs to be in order to move blocks, so we have to fix this at some point.
 */

public class DragBoxTool2 : MonoBehaviour
{
    public float forceAmount = 500;

    Rigidbody selectedRigidbody;
    public Camera targetCamera;
    Vector3 originalScreenTargetPosition;
    Vector3 originalRigidbodyPos;
    float selectionDistance;
    public bool isDragging = false;

    void Update()
    {
        if (!targetCamera)
            return;

        if(isDragging && !selectedRigidbody)
        {
            Debug.Log("IsDragging");
            //Check if we are hovering over Rigidbody, if so, select it
            selectedRigidbody = GetRigidbodyFromMouseClick();
        }
        else if (!isDragging && selectedRigidbody)
        {
            Debug.Log("Not dragging");
            //Release selected Rigidbody if there any
            selectedRigidbody = null;
        }
    }

    void FixedUpdate()
    {
        if (selectedRigidbody)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Vector3 mousePositionOffset = targetCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, selectionDistance)) - originalScreenTargetPosition;
            selectedRigidbody.freezeRotation = true;
            Vector3 dragVelocity = (originalRigidbodyPos + mousePositionOffset - selectedRigidbody.transform.position) * forceAmount * Time.deltaTime;
            dragVelocity.y = 0;
            selectedRigidbody.velocity = dragVelocity;
        }
    }

    Rigidbody GetRigidbodyFromMouseClick()
    {
        RaycastHit hitInfo = new RaycastHit();
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = targetCamera.ScreenPointToRay(mousePos);
        bool hit = Physics.Raycast(ray, out hitInfo);
        if (hit)
        {
            if (hitInfo.collider.gameObject.GetComponent<Rigidbody>())
            {
                selectionDistance = Vector3.Distance(ray.origin, hitInfo.point);
                originalScreenTargetPosition = targetCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, selectionDistance));
                originalRigidbodyPos = hitInfo.collider.transform.position;
                return hitInfo.collider.gameObject.GetComponent<Rigidbody>();
            }
        }

        return null;
    }

    public void ToggleDragBlock(bool dragBlock)
    {
        Debug.Log($"block is dragging: {dragBlock}");
        isDragging = dragBlock;
    }
}
