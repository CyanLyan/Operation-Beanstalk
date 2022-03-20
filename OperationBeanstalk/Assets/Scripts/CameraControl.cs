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

    public Quaternion dropRotation;
    private Vector3 newDropPos;
    public bool cameraIsInDropView = false;

    public bool userCanMoveCamera = true;
    private float initDistanceFromTowerCenter;
    private GameObject mainView;
    private GameObject dropView;

    //Primarily controls where camera is, based on which keys are pressed by a user every frame.
    private void Awake()
    {
        this.initDistanceFromTowerCenter = Vector3.Distance(transform.position, GameObject.Find("EventSystem").transform.position);
        this.mainView = GameObject.Find("MainView").gameObject;
        this.dropView = GameObject.Find("DropView").gameObject;
    }

    void Update()
    {
        if(Input.anyKey && this.userCanMoveCamera) {
            HandleUserCameraInput();
        }
    }

    public void HandleUserCameraInput()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Debug.Log("left");
            this.mainView.transform.RotateAround(towerCenter.transform.position, new Vector3(0, 1, 0), speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Debug.Log("right");
            this.mainView.transform.RotateAround(towerCenter.transform.position, new Vector3(0, -1, 0), speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("up");
            this.mainView.transform.position = Vector3.Lerp(this.mainView.transform.position, new Vector3(this.mainView.transform.position.x, maxHeight, this.mainView.transform.position.z), Time.deltaTime * yMoveSpeed);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("down");
            this.mainView.transform.position = Vector3.Lerp(this.mainView.transform.position, new Vector3(this.mainView.transform.position.x, 0, this.mainView.transform.position.z), Time.deltaTime * yMoveSpeed);
        }
        
        transform.position = this.mainView.transform.position;
        transform.rotation = this.mainView.transform.rotation;
    }
       
    //Moves camera to a view atop the current jenga tower where we can see the tower top & the block being placed.
    public IEnumerator pivotToDropView()
    {
        this.initCameraPosition = transform.position;
        this.initCameraRotation = transform.rotation;
        var dropView = Vector3.zero;
        dropView.y = this.maxHeight;
        this.newDropPos = new Vector3(0.27f, dropView.y, -7.8f);

        this.userCanMoveCamera = false;

        var duration = 5.0f;
        for (var t = 0.0f; t < duration; t += Time.deltaTime)
        {
            moveCameraBetween2Points(transform.position, this.newDropPos, t / duration);
            rotateCameraBetween2Points(transform.rotation, this.dropRotation, t / duration);
            yield return null;
        }
        this.userCanMoveCamera = true;
        this.cameraIsInDropView = true;
    }

    //Returns camera to previous camera position
    public IEnumerator pivotBackToPreviousView()
    {
        this.userCanMoveCamera = false;

        var newCameraRotation = new Quaternion(this.initCameraRotation.x, this.initCameraRotation.y, this.initCameraRotation.z, this.initCameraRotation.w);
        var newCameraPosition = new Vector3(transform.position.x, this.initCameraPosition.y, this.initDistanceFromTowerCenter);
        var duration = 5.0f;
        for (var t = 0.0f; t < duration; t += Time.deltaTime)
        {
            moveCameraBetween2Points(transform.position, newCameraPosition, t / duration);
            rotateCameraBetween2Points(transform.rotation, newCameraRotation, t / duration);
            yield return null;
        }
        this.userCanMoveCamera = true;
        this.cameraIsInDropView = true;
    }

    public void moveCameraBetween2Points(Vector3 pointA, Vector3 pointB, float duration)
    {
        transform.position = Vector3.Lerp(pointA, pointB, duration);
    }

    public void rotateCameraBetween2Points(Quaternion rotationA, Quaternion rotationB, float duration)
    {
            Quaternion toRotation = Quaternion.FromToRotation(this.newDropPos, this.towerTop.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, duration);
    }
}
