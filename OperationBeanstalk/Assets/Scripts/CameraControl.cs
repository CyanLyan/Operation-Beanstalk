using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public float maxHeight;
    private float speed = 100f;
    public Vector3 dropView;
    public Quaternion dropRotation;
    // Start is called before the first frame update

    // Update is called once per frame
 
    void Update()
    {
        
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("left");
            //transform.LookAt(target);
            //transform.Translate(Vector3.left * Time.deltaTime);
            transform.RotateAround(target.transform.position, new Vector3(0,1,0), speed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("right");
            //  transform.LookAt(target);
            //transform.Translate(Vector3.right * Time.deltaTime);
            transform.RotateAround(target.transform.position, new Vector3(0, -1, 0), speed * Time.deltaTime);
        } 
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("up");
            //transform.LookAt(target);
            //transform.Translate(Vector3.left * Time.deltaTime);
            transform.Translate(Vector3.up * 0.5f, Space.World);
            transform.position = (transform.position.y < maxHeight) ? transform.position : new Vector3(transform.position.x, maxHeight, transform.position.z);
        } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("down");
            //  transform.LookAt(target);
            //transform.Translate(Vector3.right * Time.deltaTime);
            transform.Translate(Vector3.down * 0.5f, Space.World);
            transform.position = (transform.position.y > 0) ? transform.position : new Vector3(transform.position.x, 0, transform.position.z);
        } 
        transform.LookAt(target);
    }

    public void pivotToDropView()
    {
        this.dropView.y = this.maxHeight + 1;

        //this.dropRotation.Set(26.6f, -0.75f, 0, 0f);

        transform.position = new Vector3(0.27f, 13.72f, -7.8f);
        transform.rotation = new Quaternion(49.681f, -2f, 0, 0f);
    }
}
