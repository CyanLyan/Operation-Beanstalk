using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform towerCenter;

    public Transform towerTop;

    public bool showDropPosition;

    public Vector3 initCameraPosition;
    public Quaternion initCameraRotation;

    public float yMoveSpeed = 1f;
    public float maxHeight;
    private float speed = 100f;

    private float cameraMoveSpeed = 10f;

    public Vector3 dropView;
    public Quaternion dropRotation;
    private Vector3 newDropPos;
    private bool cameraIsInDropViewPosition;
    private bool cameraIsInDropViewRotation;

    // Start is called before the first frame update

    // Update is called once per frame

    //Primarily controls where camera is, based on which keys are pressed by a user every frame.
    void Update()
    {
        if (this.showDropPosition && !CameraIsInDropView())
        {
            //moveCameraBetween2Points(transform.position, this.newDropPos);
            //rotateCameraBetween2Points(transform.rotation, this.dropRotation);
        } else if(Input.anyKey) {
            HandleUserCameraInput();
        }
    }

    public bool CameraIsInDropView()
    {
        return this.cameraIsInDropViewPosition && this.cameraIsInDropViewRotation;
    }

    public void HandleUserCameraInput()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Debug.Log("left");
            transform.RotateAround(towerCenter.transform.position, new Vector3(0, 1, 0), speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Debug.Log("right");
            transform.RotateAround(towerCenter.transform.position, new Vector3(0, -1, 0), speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("up");
            //transform.Translate(Vector3.up * 0.05f, Space.World);
            //transform.position = (transform.position.y < maxHeight) ? transform.position : new Vector3(transform.position.x, maxHeight, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, maxHeight, transform.position.z), Time.deltaTime * yMoveSpeed);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("down");
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 0, transform.position.z), Time.deltaTime * yMoveSpeed);
            //transform.Translate(Vector3.down * 0.05f, Space.World);
            //transform.position = (transform.position.y > 0) ? transform.position : new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    //Moves camera to a view atop the current jenga tower where we can see the tower top & the block being placed.
    public IEnumerator pivotToDropView()
    {
        this.initCameraPosition = transform.position;
        this.initCameraRotation = transform.rotation;
        this.dropView.y = this.maxHeight;
        this.newDropPos = new Vector3(0.27f, this.dropView.y, -7.8f);
        this.showDropPosition = true;

        var duration = 5.0f;
        for(var t=0.0f; t < duration; t+=Time.deltaTime)
        {
            moveCameraBetween2Points(transform.position, this.newDropPos, t/duration);
            rotateCameraBetween2Points(transform.rotation, this.dropRotation, t/duration);
            yield return null;
        }

    }

    public void moveCameraBetween2Points(Vector3 pointA, Vector3 pointB, float duration)
    {
        //if (transform.position != pointB)
        //{
            transform.position = Vector3.Lerp(pointA, pointB, duration);
        //} else
        //{
        //    this.cameraIsInDropViewPosition = true;
        //}
    }

    public void rotateCameraBetween2Points(Quaternion rotationA, Quaternion rotationB, float duration)
    {
        //if(transform.rotation != rotationB)
        //{
            //transform.rotation = Quaternion.Lerp(rotationA, rotationB, Time.deltaTime * 0.1f);
            Quaternion toRotation = Quaternion.FromToRotation(this.newDropPos, this.towerTop.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, duration);
        //} else
        //{
        //    this.cameraIsInDropViewRotation = true;
        //}
    }

    
}
