using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
    private Vector3 previousViewPosition;
    private Quaternion previousViewRotation;
    public bool userCanMoveCamera = true;
    private float initDistanceFromTowerCenter;
    private GameObject mainView;
    private GameObject dropView;

    //Primarily controls where camera is, based on which keys are pressed by a user every frame.
    private void Awake()
    {
        this.initDistanceFromTowerCenter = Vector3.Distance(transform.position, GameObject.Find("EventSystem").transform.position);
        this.mainView = GameObject.Find("MainView").gameObject;
        this.previousViewPosition = Vector3.zero;
        this.previousViewRotation = new Quaternion(0,0,0,0);
    }

    void Update()
    {
        if(Input.anyKey && this.userCanMoveCamera) {
            HandleUserCameraInput();
        }
    }

    //Moves MainView - which the camera follows while we're removing blocks.
    //It also moves the x & z axis of the block drop camera view - DropView.
    //Having these two views be separate objects allows us to snap between the views easily
    public void HandleUserCameraInput()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            this.mainView.transform.RotateAround(towerCenter.transform.position, new Vector3(0, 1, 0), speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            this.mainView.transform.RotateAround(towerCenter.transform.position, new Vector3(0, -1, 0), speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            this.mainView.transform.position = Vector3.Lerp(this.mainView.transform.position, new Vector3(this.mainView.transform.position.x, maxHeight, this.mainView.transform.position.z), Time.deltaTime * yMoveSpeed);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            this.mainView.transform.position = Vector3.Lerp(this.mainView.transform.position, new Vector3(this.mainView.transform.position.x, 0, this.mainView.transform.position.z), Time.deltaTime * yMoveSpeed);
        }

        transform.position = this.mainView.transform.position;
        transform.rotation = this.mainView.transform.rotation;
    }

    // TODO: Eventually we should be picking dropview based on orientation or block being placed
    public Transform SelectClosestDropViewToMainCamera()
    {
        var dropViews = GetDropViews();
        var dropBlockRotation = gameObject.transform.rotation.x;
        var dropViewsFacingBlockOrientation = new string[2];
        var closest90DegreeAngle = Mathf.Round((dropBlockRotation*100) / 90) * 90;
        var dropView = FindClosest(dropViews, closest90DegreeAngle);
         
        return dropView.transform;
    }

    public List<GameObject> GetDropViews()
    {
        var dropViews = GameObject.FindGameObjectsWithTag("DropView");
        return new List<GameObject>(dropViews);
    }

    //TODO: Change dropview class names to variables declared upon view being settled.
    public GameObject FindClosest(List<GameObject> targets, float angle)
    {

        GameObject cameraAngleToReturn;
        switch (angle)
        {
            case 0:
                cameraAngleToReturn = targets.FirstOrDefault(x => x.name == "DropView2");
                break;

            case 90:
                cameraAngleToReturn = targets.FirstOrDefault(x => x.name == "DropView3");
                break;

            case 180:
                cameraAngleToReturn = targets.FirstOrDefault(x => x.name == "DropView4");
                break;

            case 270:
                cameraAngleToReturn = targets.FirstOrDefault(x => x.name == "DropView1");
                break;

            default:
                cameraAngleToReturn = GameObject.FindGameObjectWithTag("TopDownDropView");
                break;
        }

        return cameraAngleToReturn;
    }

       
    //Moves camera to a view atop the current jenga tower where we can see the tower top & the block being placed.
    public IEnumerator pivotToDropView()
    {
        this.previousViewPosition = this.transform.position;
        this.previousViewRotation = this.transform.rotation;
        this.userCanMoveCamera = false;
        var dropView = this.SelectClosestDropViewToMainCamera();
        dropView.transform.LookAt(this.towerTop);

        var duration = 2f;
        for (var t = 0.0f; t < duration; t += Time.deltaTime)
        {
            moveCameraBetween2Points(transform.position, dropView.transform.position, t / duration);
            rotateCameraBetween2Points((t / duration)*20, dropView.rotation);
            yield return null;
        }
        this.userCanMoveCamera = true;
        this.cameraIsInDropView = true;
    }

    //Returns camera to previous camera position
    public IEnumerator pivotBackToPreviousView()
    {
        this.userCanMoveCamera = false;

        var duration = 2.0f;
        for (var t = 0.0f; t < duration; t += Time.deltaTime)
        {
            moveCameraBetween2Points(transform.position, this.previousViewPosition, t / duration);
            rotateCameraBetween2Points((t / duration) * 20, this.previousViewRotation);
            yield return null;
        }
        this.userCanMoveCamera = true;
        this.cameraIsInDropView = true;
    }

    public void moveCameraBetween2Points(Vector3 pointA, Vector3 pointB, float duration)
    {
        transform.position = Vector3.Lerp(pointA, pointB, duration);
    }

    public void rotateCameraBetween2Points(float duration, Quaternion newRotation)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, duration);
    }
}
