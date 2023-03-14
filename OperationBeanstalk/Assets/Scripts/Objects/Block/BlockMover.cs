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

    private bool doneRotating;
    private bool donePositioning;
    private bool moveToPoint1;
    private bool moveToPoint2;

    public float moveDistancePerStep;

    private UnityAction movementEnd;

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

    /**
     * Activated externally by the dropzone collision box, once the block is in place.
     * TODO - after implementing SWS (simple waypoint system) see if we even need this
     *        chain of events or can just manually end the sequence ourselves.
     * */
    public void FinishDroppingBlockInPlace()
    {
        Destroy(block.gameObject.GetComponent<splineMove>());
        block.isBeingPlacedOnTop = false;
        block.blocksTouching = true;
        block.isInDropPosition = false;
        block.userCanDrag = false;
        block.userCanNudge = false;
        block.hasBlockBeenMovedByPlayerRecently = false;
        block.GetComponent<Rigidbody>().freezeRotation = false;
        block.hasBeenPlaced = true;
        StartCoroutine(cam.pivotBackToPreviousView(cam.transform.position));
        gameController.FinishTurn();
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
        point2.transform.position = block.transform.TransformPoint(towerDropZone.transform.position);
        point3 = CopyTransform(point3,towerDropZone.transform);
        pathManager.Create(new []{point1.transform, point2.transform, point3.transform});
        return pathManager;
    }

    private GameObject CopyTransform(GameObject EmptyObj, Transform transform)
    {
        EmptyObj.transform.position = transform.position;
        EmptyObj.transform.rotation = transform.rotation;
        return EmptyObj;
    }
    public void PlaceBlockInDroppingPosition(Block block)
    {
        this.block = block;
        block.userCanDrag = false;
        block.isBeingPlacedOnTop = true;
        block.GetComponent<Rigidbody>().useGravity = false;
        block.userCanNudge = false;
        gameController.GoToTurnState(TurnState.PlaceBlock);
        StartCoroutine(cam.pivotToDropView());
        midwayBlockMovePoint.transform.position = Vector3.zero;   
        pathManager = CreatePath();
        CreateSplineMove(pathManager);
        StartCoroutine(MoveBlockToDropRotation());
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

    public IEnumerator MoveBlockToDropRotation()
    {
        this.doneRotating = false;
        while (Quaternion.Angle(block.transform.rotation, block.originalRotation) > 1f)
        {
            block.gameObject.transform.rotation = Quaternion.RotateTowards(block.transform.rotation, block.originalRotation, 100f);
            yield return 0;
        }
        this.doneRotating = true;
    }
}
