using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform towerCollisionBox;

    public Transform towerTop;
    public Transform towerDroppingZone;

    public bool showDropPosition;

    public Vector3 initCameraPosition;
    public Quaternion initCameraRotation;

    public float yMoveSpeed = 17f;
    public float maxHeight;
    private float speed = 100f;

    private float _cameraViewPivotSpeed;
    public float cameraViewPivotSpeed 
    {
        get { return _cameraViewPivotSpeed; }
        set { _cameraViewPivotSpeed = value;}
    }

    public Quaternion dropRotation;
    public bool cameraIsInDropView;
    private Vector3 previousViewPosition;
    private Quaternion previousViewRotation;
    public bool userCanMoveCamera = true;

    public Transform CurrentCameraFocus;
    private float initDistanceFromTowerCollisionBox;
    private GameObject mainView;
    private GameObject dropView;
    private GameObject topDownView;

    private Key currentUserInput;

    //Primarily controls where camera is, based on which keys are pressed by a user every frame.
    private void Awake()
    {
        initDistanceFromTowerCollisionBox = Vector3.Distance(transform.position, GameObject.Find("EventSystem").transform.position);
        mainView = GameObject.Find("MainView").gameObject;
        topDownView = GameObject.Find("TopDownDropView");
        previousViewPosition = Vector3.zero;
        previousViewRotation = new Quaternion(0,0,0,0);

        CurrentCameraFocus = towerCollisionBox;
    }

    public void Update()
    {
        
        HandleUserCameraInput(currentUserInput);
    }

    public void SetCameraInputKey(Key keyCode)
    {
        if(keyCode == Key.None)
        {
            currentUserInput = 0;
        }
        currentUserInput = keyCode;
    }

    //Moves MainView - which the camera follows while we're removing blocks.
    //It also moves the x & z axis of the block drop camera view - DropView.
    //Having these two views be separate objects allows us to snap between the views easily
    public void HandleUserCameraInput(Key keyCode)
    {
        switch (keyCode)
        {
            case Key.Q:

            case Key.LeftArrow:
            case Key.A:
                mainView.transform.RotateAround(CurrentCameraFocus.transform.position, new Vector3(0, 1, 0), speed * Time.deltaTime);
                break;

            case Key.RightArrow:
            case Key.E:
            case Key.D:
                mainView.transform.RotateAround(CurrentCameraFocus.transform.position, new Vector3(0, -1, 0), speed * Time.deltaTime);
                break;
            case Key.W:
            case Key.UpArrow:
                mainView.transform.position = Vector3.Lerp(mainView.transform.position, new Vector3(mainView.transform.position.x, maxHeight, mainView.transform.position.z), Time.deltaTime * yMoveSpeed);
                break;
        
            case Key.DownArrow:
            case Key.S:
                mainView.transform.position = Vector3.Lerp(mainView.transform.position, new Vector3(mainView.transform.position.x, 0, mainView.transform.position.z), Time.deltaTime * yMoveSpeed);        
                break;

            default:
                break;
        }

        transform.position = mainView.transform.position;
        transform.rotation = mainView.transform.rotation;
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
                cameraAngleToReturn = targets.FirstOrDefault(x => x.name == "DropViewPosition2");
                break;

            case 90:
                cameraAngleToReturn = targets.FirstOrDefault(x => x.name == "DropViewPosition3");
                break;

            case 180:
                cameraAngleToReturn = targets.FirstOrDefault(x => x.name == "DropViewPosition4");
                break;

            case 270:
                cameraAngleToReturn = targets.FirstOrDefault(x => x.name == "DropViewPosition1");
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
        previousViewPosition = mainView.transform.position;
        previousViewRotation = mainView.transform.rotation;
        var dropView = SelectClosestDropViewToMainCamera();
        dropView.transform.LookAt(towerTop);

        for (var t = 0.0f; t < cameraViewPivotSpeed; t += Time.deltaTime)
        {
            //Debug.Log(t);
            moveCameraBetween2Points(mainView.transform.position, dropView.transform.position, t / cameraViewPivotSpeed);
            rotateCameraBetween2Points((t / cameraViewPivotSpeed) *20, dropView.rotation);
            transform.position = mainView.transform.position;
            transform.rotation = mainView.transform.rotation;
            yield return null;
        }
        cameraIsInDropView = true;
        CurrentCameraFocus = towerDroppingZone;
    }

    //Returns camera to previous camera position
    public IEnumerator pivotBackToPreviousView(Vector3 startingPosition)
    {

        for (var t = 0.0f; t < cameraViewPivotSpeed; t += Time.deltaTime)
        {
            moveCameraBetween2Points(mainView.transform.position, previousViewPosition, (t / cameraViewPivotSpeed));
            rotateCameraBetween2Points((t / cameraViewPivotSpeed)*20, previousViewRotation);
            transform.position = mainView.transform.position;
            transform.rotation = mainView.transform.rotation;
            yield return null;
        }
        cameraIsInDropView = false;
        CurrentCameraFocus = towerCollisionBox;
    }

    public void moveCameraBetween2Points(Vector3 pointA, Vector3 pointB, float duration)
    {
        mainView.transform.position = Vector3.Lerp(pointA, pointB, duration);
    }

    public void rotateCameraBetween2Points(float duration, Quaternion newRotation)
    {
        mainView.transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, duration);
    }

    public void adjustDropViewHeight(float maxViewHeight)
    {
        List<GameObject> dropViewCameras = GetDropViews();
        foreach (var dropView in dropViewCameras)
        {
            dropView.transform.position = new Vector3(dropView.transform.position.x, maxViewHeight, dropView.transform.position.z);
        }
    }

    public IEnumerator pivotToDropCam()
    {
        Debug.Log("Pivoting to tower drop cam");
        for (var t = 0.0f; t < cameraViewPivotSpeed; t += Time.deltaTime)
        {
            moveCameraBetween2Points(topDownView.transform.position, previousViewPosition, (t / cameraViewPivotSpeed));
            rotateCameraBetween2Points((t / cameraViewPivotSpeed) * 20, previousViewRotation);
            transform.position = topDownView.transform.position;
            transform.rotation = topDownView.transform.rotation;
            yield return null;
        }
    }
}
