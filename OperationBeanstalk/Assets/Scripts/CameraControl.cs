using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public Transform towerTop;

    public bool showDropPosition;
    public float maxHeight;
    private float speed = 100f;
    public Vector3 dropView;
    public Quaternion dropRotation;
    // Start is called before the first frame update

    // Update is called once per frame
 
    //Primarily controls where camera is, based on which keys are pressed by a user every frame.
    void Update()
    {
        
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("left");
            transform.RotateAround(target.transform.position, new Vector3(0,1,0), speed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("right");
            transform.RotateAround(target.transform.position, new Vector3(0, -1, 0), speed * Time.deltaTime);
        } 
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("up");
            transform.Translate(Vector3.up * 0.5f, Space.World);
            transform.position = (transform.position.y < maxHeight) ? transform.position : new Vector3(transform.position.x, maxHeight, transform.position.z);
        } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("down");
            transform.Translate(Vector3.down * 0.5f, Space.World);
            transform.position = (transform.position.y > 0) ? transform.position : new Vector3(transform.position.x, 0, transform.position.z);
        }

        
        if(this.showDropPosition)
        {
            transform.LookAt(towerTop);
        } else
        {
            transform.LookAt(target);
        }
    }

    //Moves camera to a view atop the current jenga tower where we can see the tower top & the block being placed.
    public void pivotToDropView()
    {
        this.target = this.towerTop;
        transform.LookAt(towerTop);
        this.showDropPosition = true;
        this.dropView.y = this.maxHeight + 1;

        this.dropRotation.Set(26.6f, -0.75f, 0, 0f);

        gameObject.transform.position = new Vector3(0.27f, this.dropView.y, -7.8f);
        gameObject.transform.rotation = new Quaternion(49.681f, -2f, 0, 0f);
    }
}
