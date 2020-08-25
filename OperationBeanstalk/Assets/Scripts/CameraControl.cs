using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    private float speed = 100f;
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
        } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("down");
            //  transform.LookAt(target);
            //transform.Translate(Vector3.right * Time.deltaTime);
            transform.Translate(Vector3.down * 0.5f, Space.World);
        } 
        transform.LookAt(target);
    }
}
