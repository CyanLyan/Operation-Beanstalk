using System;
using System.Collections;
using DG.Tweening;
using SWS;
using UnityEngine;
using UnityEngine.Events;

public class BlockMover : MonoBehaviour
{

    public GameController gameController;
    public GameObject towerDropZone;
    public GameObject camObj;
    public CameraController cam;
    public GameObject midwayBlockMovePoint;

    public Vector3 positionBeforeMoving;

    public PathManager pathManager;

    public float moveDistancePerStep;

    private UnityAction movementEnd;

    private bool blockIsBeingPlaced;

    private Block block;
    private GameObject point1;
    private GameObject point2;
    private GameObject point3;

    private void Start()
    {
        point1 = new GameObject("tmp1");
        point2 = new GameObject("tmp2");
        point3 = new GameObject("tmp3");
    }

    private void CreateSplineMove(PathManager pathManager)
    {
        var splineMove = block.gameObject.AddComponent<splineMove>();
        splineMove.onStart = true;
        splineMove.speed = 50;
        splineMove.easeType = Ease.InOutQuart;
        splineMove.pathContainer = pathManager;
        splineMove.movementEnd.AddListener(SetBlockStatsForDropPosition);
        splineMove.pathMode = PathMode.Ignore; 
        splineMove.StartMove();
    }

    private PathManager CreatePath()
    {
        point1 = CopyTransform(point1,block.transform);
        point2 = CopyTransform(point2,midwayBlockMovePoint.transform);
        //point2.transform.position = block.transform.TransformPoint(towerDropZone.transform.position);
        var towerTop = GameObject.FindGameObjectWithTag("DropZone"); 
        //var newDropPosition = new Trans(towerTop.transform.position.x, towerTop.transform.position.y - 100, towerTop.transform.position.z);
        //towerTop.transform.position = new Vector3(towerTop.transform.position.x, towerTop.transform.position.y - 100, towerTop.transform.position.z);
        point3 = CopyTransform(point3, towerTop.transform);
        pathManager.Create(new []{point1.transform, point2.transform, point3.transform});
        return pathManager;
    }

    private GameObject CopyTransform(GameObject EmptyObj, Transform transform)
    {
        EmptyObj.transform.position = transform.position;
        EmptyObj.transform.rotation = transform.rotation;
        return EmptyObj;
    }

    //TODO - fix all these direct value accesses 
    public void PlaceBlockInDroppingPosition(Block block)
    {
        if(blockIsBeingPlaced) { return; }
        block.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
        blockIsBeingPlaced = true;
        this.block = block;
        block.userCanDrag = false;
        block.isBeingPlacedOnTop = true;
        block.ToggleGravity(false);
        block.userCanNudge = false;
        gameController.GoToTurnState(TurnState.PlaceBlock);
        StartCoroutine(cam.pivotToDropView());
        //var closestTransform = GetClosestDropViewToBlock(block);
        //var cameraTransform = GameObject.Find("MainView").gameObject.transform.position;
        Vector3 direction = (block.transform.position - towerDropZone.transform.position).normalized;
        Vector3 safePosition = block.transform.position + direction * 3.0f;
               
        midwayBlockMovePoint.transform.position = new Vector3(safePosition.x, midwayBlockMovePoint.transform.position.y, safePosition.z*2);   
        pathManager = CreatePath();
        CreateSplineMove(pathManager);
        StartCoroutine(MoveBlockToDropRotation(block));
    }

    /**
 * Activated externally by the dropzone collision box, once the block is in place.
 * TODO - after implementing SWS (simple waypoint system) see if we even need this
 *        chain of events or can just manually end the sequence ourselves.
 * */
    public void FinishDroppingBlockInPlace()
    {
        Debug.Log("Finished Putting Block in Dropping Position");
        Destroy(block.gameObject.GetComponent<splineMove>());
        block.isBeingPlacedOnTop = false;
        block.blocksTouching = true;
        block.isInDropPosition = false;
        block.userCanDrag = false;
        block.userCanNudge = false;
        block.userIsPlacingBlockOnTop = false;
        block.hasBlockBeenMovedByPlayerRecently = false;
        block.GetComponent<Rigidbody>().freezeRotation = false;
        block.hasBeenPlaced = true;
        StartCoroutine(cam.pivotBackToPreviousView(cam.transform.position));
        blockIsBeingPlaced = false;
        gameController.FinishTurn();
    }

    private void SetBlockStatsForDropPosition()
    {
        block.isInDropPosition = true;
        block.userCanNudge = false;
        block.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        block.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        block.GetComponent<Rigidbody>().angularDrag = 2f;
        block.GetComponent<Rigidbody>().drag = 1f;
    }


    //TODO - figure out why this doesn't work
    public IEnumerator MoveBlockToDropRotation(Block block)
    {
        //this.doneRotating = false;
        while (Quaternion.Angle(block.transform.rotation, block.originalRotation) > 1f)
        {
            block.gameObject.transform.rotation = Quaternion.RotateTowards(block.transform.rotation, block.originalRotation, 100f);
            yield return 0;
        }
        Debug.Log("Finished moving block!");
        block.userCanDrag = true;
        //this.doneRotating = true;
    }

    public Transform GetClosestDropViewToBlock(Block block)
    {
        var dropViews = GameObject.FindGameObjectsWithTag("DropView");
        var maxDistance = -Mathf.Infinity;
        Transform closestTransform = null;
        foreach(GameObject dropView in dropViews)
        {
            var distance = dropView.transform.position - block.transform.position;
            float sqrDistance = distance.sqrMagnitude;
            if (maxDistance < sqrDistance)
            {
                maxDistance = sqrDistance;
                closestTransform = dropView.transform;
            }
        }
        return closestTransform;
    }
}
