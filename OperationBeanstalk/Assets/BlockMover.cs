using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMover : MonoBehaviour
{

    public GameController gameController;
    public GameObject towerDropZone;
    public GameObject camObj;
    public CameraController cam;
    public GameObject midwayBlockMovePoint;

    public Vector3 positionBeforeMoving;

    private bool doneRotating;
    private bool donePositioning;
    private bool moveToPoint1;
    private bool moveToPoint2;

    // Start is called before the first frame update
    //void Start()
    //{
    //    this.cam = this.camObj.GetComponent<CameraController>();
    //}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinishDroppingBlockInPlace(Block block)
    {
        block.isBeingPlacedOnTop = false;
        block.blocksTouching = true;
        block.isInDropPosition = false;
        block.userCanDrag = false;
        block.userCanNudge = false;
        block.hasBlockBeenMovedByPlayerRecently = false;
        block.GetComponent<Rigidbody>().freezeRotation = false;
        block.hasBeenPlaced = true;
        StartCoroutine(this.cam.pivotBackToPreviousView(this.cam.transform.position));
        this.gameController.FinishTurn();
    }

    public void PlaceBlockInDroppingPosition(Block block)
    {
        block.userCanDrag = false;
        block.isBeingPlacedOnTop = true;
        var point1 = midwayBlockMovePoint.transform.TransformPoint(midwayBlockMovePoint.transform.position);
        var point2 = block.transform.TransformPoint(this.towerDropZone.transform.position);
        block.GetComponent<Rigidbody>().useGravity = false;
        block.userCanNudge = false;
        this.gameController.GoToTurnState(TurnState.PlaceBlock);
        StartCoroutine(this.cam.pivotToDropView());
        this.midwayBlockMovePoint.transform.position = Vector3.zero;

        StartCoroutine(MoveBlockToDropPosition(block));
        StartCoroutine(MoveBlockToDropRotation(block));
        StartCoroutine(WaitForBlockToBePositionedAndRotated(block));
    }

    public IEnumerator WaitForBlockToBePositionedAndRotated(Block block)
    {
        while (!this.donePositioning || !this.doneRotating)
        {
            yield return 0;
        }
        block.isInDropPosition = true;
        block.userCanNudge = false;
        block.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        block.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        block.GetComponent<Rigidbody>().angularDrag = 0.05f;
        block.GetComponent<Rigidbody>().drag = 1f;
    }

    public IEnumerator MoveBlockToDropPosition(Block block)
    {
        this.donePositioning = false;
        while ((this.towerDropZone.transform.position.y - block.transform.position.y > 1))
        {
            var dist = Vector3.Distance(block.transform.position, block.blockStartPos);
            if (dist < 5f)
            {
                //Calculate the vector between the object and the player
                Vector3 dir = block.transform.position - block.blockStartPos;
                //Cancel out the vertical difference
                dir.y = 0;
                //Translate the object in the direction of the vector
                block.gameObject.transform.position = Vector3.MoveTowards(block.transform.position, new Vector3(dir.normalized.x, this.towerDropZone.transform.position.y, 0), 5f);
            }
            else
            {
                block.gameObject.transform.position = Vector3.MoveTowards(block.transform.position, new Vector3(0, Camera.main.GetComponent<CameraController>().maxHeight - 1f, 0), 5f);
            }
            yield return 0;
        }
        this.donePositioning = true;
    }

    //TODO - get this mess working
    public IEnumerator MoveBlockToPoint1Then2(Block block, Vector3 point1, Vector3 point2)
    {
        this.donePositioning = false;
        while(!this.moveToPoint1)
        {
            yield return MoveBlockToPoint(block, point1, 15.0f, this.moveToPoint1);

        }
        while(!this.moveToPoint2)
        {
            yield return MoveBlockToPoint(block, point2, 1.0f, this.moveToPoint2);
        }
        if (this.moveToPoint1 && this.moveToPoint2)
        {
            this.donePositioning = true;
        } else
        {
            yield return null;

        }
    }

    public IEnumerator MoveBlockToPoint(Block block, Vector3 point, float distanceToPointNeeded, bool pointReached)

    {
        var blockPositionInWorld = gameObject.transform.TransformPoint(block.transform.position);
        Debug.Log(blockPositionInWorld);
        while ((point.y - block.transform.position.y) > distanceToPointNeeded)
        {
            block.gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, point, 5f);
            yield return 0;
        }
        pointReached = true;
    }

    public IEnumerator MoveBlockToDropRotation(Block block)
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
