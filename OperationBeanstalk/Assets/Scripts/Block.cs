using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool hasBlockBeenMoved {get; set;} = false;
    public bool isBlockTouchingGround {get; set;} = false;
    public static int nBlocksOnGround {get; set;} = 0;

    public BoxCollider towerZone;

    private void Awake()
    {
        towerZone = GameObject.Find("Tower").GetComponent<BoxCollider>();
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag != "GroundPlane") return; //for now, drop this function for all collisions but that with the ground
        isBlockTouchingGround = true;
        nBlocksOnGround++;
    }

    void OnCollisionExit(Collision other)
    {
        if(other.gameObject.tag != "GroundPlane") return; //for now, drop this function for all collisions but that with the ground
        isBlockTouchingGround = false;
        if(nBlocksOnGround-- < 3); //Tower collapse condition
    }

    private Vector3 mOffset;
    private float mZCoord;

    void OnMouseDown()
    {
        Debug.Log("mouse down");
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        // Store offset = gameobject world pos - mouse world pos
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

    private void OnMouseOver()
    {
        Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            transform.Translate(new Vector3(0, -1, 0) * Input.GetAxis("Mouse ScrollWheel"));
            //transform.position += (transform.forward * Input.GetAxis("Mouse ScrollWheel"));
        } else if(Input.GetAxis("Mouse ScrollWheel") < 0.0f) {
            transform.Translate(new Vector3(0,1,0)* Input.GetAxis("Mouse ScrollWheel") * -1);
        }
    }
}
