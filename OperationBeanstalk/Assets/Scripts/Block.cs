using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // Block states
    public bool hasBlockBeenMovedByPlayerRecently { get; set; } = false;
    public bool isBlockTouchingGround { get; set; } = false;
    public bool blocksTouching { get; set; } = false;
    
    public bool isBeingNudged = false;

    public bool blockIsInTowerZone = true;
    
    public bool isBeingPlacedOnTop = false;
   
    public bool isInDropPosition = false;

    public bool isBeingDragged = false;

    public bool isActive = false;

    public bool userCanDrag = true;
    public bool userCanNudge = true;
    public static int nBlocksOnGround { get; set; } = 0;

    public BoxCollider towerZone;

    public float nudgeForce = 500f;

    public float timeSpentNotTouching = 0f;

    private float startTime;
    private Vector3 mouseStartPos = new Vector3(0,0,0);

    private Vector3 blockStartPos;
    public float mouseDriftPermittedToNudge = 100f;

    private CameraControl cam;
    private Quaternion originalRotation;

    private string blockObjTag = "Block";

    private Outline outline;

    public GameController gameController;
    public GameObject lineRenderer;

    public AudioSource soundEmitter;

    private CursorController cursorInstance;

    private Rigidbody rigidbody;
    private bool doneRotating;
    private bool donePositioning;

    //Function to call instead of Awake/Start, should be faster as it already has access to these components
    public void Init(GameController gameController, 
                 CameraControl cam,
                 CursorController cursorInstance)
    {
        this.gameController = gameController;
        this.blockStartPos = gameObject.transform.position;
        this.originalRotation = transform.rotation;
        this.cam = cam;
        this.outline = this.GetComponent<Outline>();
        this.cursorInstance = cursorInstance;
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.gameObject.name = blockObjTag + GetInstanceID().ToString();
    }

    //Runs every frame for each block. Does different actions depending on which states are enabled/disabled.
    void Update()
    {
        checkOutlineState();
        if(this.isActive)
        {
            //If the block isn't touching another block, or the ground, and not being set up
            if (!this.blocksTouching && !this.isBlockTouchingGround)
            {
                //If block is not being rotated to a neutral position AND
                //block is not being dropped from the top - another custom game state
                //this.HandleBlockTouchingNothing();

                //TODO - see if this condition below is redundant
                //If we're holding left mouse, and have moved the block enough to actually change the block's position when dragging, we cannot nudge it
                if (!Input.GetMouseButtonDown(0) || (this.mouseMovedEnoughToDrag() && !enoughTimeHasEllapsed()))
                {
                    this.hasBlockBeenMovedByPlayerRecently = true;
                    this.isBeingNudged = false;
                }

                //Else, we can nudge it!
                //TODO - make this an else to the if above
                if (!this.userCanDrag && Input.GetMouseButtonDown(0) && (this.mouseMovedEnoughToDrag() && enoughTimeHasEllapsed()))
                {
                    this.userCanDrag = true;
                }
            } 
        }
    }

    public void HandleBlockTouchingNothing()
    {
        this.userCanDrag = false;

        // Only move block to tower top IF player moved it
        if(this.hasBlockBeenMovedByPlayerRecently)
        {
            GameObject.FindGameObjectWithTag("TowerArea").GetComponent<Tower>().ActivateTowerDropZone();
            this.isBeingPlacedOnTop = true;
            PlaceBlockOnTopOfTower();
            this.gameController.GoToTurnState(TurnState.PlaceBlock);
            StartCoroutine(this.cam.pivotToDropView());
        }
    }

    public void FinishDroppingBlockInPlace()
    {
        this.isBeingPlacedOnTop = false;
        blocksTouching = true;
        this.isInDropPosition = false;
        this.gameController.GoToTurnState(TurnState.GetBlock);
        this.userCanDrag = false;
        this.userCanNudge= false;
        StartCoroutine(this.cam.pivotBackToPreviousView());
    }

    public void PlaceBlockOnTopOfTower()
    {
        this.rigidbody.useGravity = false;
        this.userCanNudge = false;
        StartCoroutine(MoveBlockToDropPosition());
        StartCoroutine(MoveBlockToDropRotation());
        WaitForBlockToBePositionedAndRotated();
    }

    public IEnumerator WaitForBlockToBePositionedAndRotated()
    {
        while(!this.donePositioning && !this.doneRotating)
        {
            yield return 0;
        }
        this.isBeingPlacedOnTop = false;
        this.isInDropPosition = true;
        this.userCanDrag = true;
        this.userCanNudge = false;
        this.rigidbody.useGravity = false;
        this.rigidbody.angularVelocity = new Vector3(0, 0, 0);
        this.rigidbody.velocity = new Vector3(0, 0, 0);
        this.rigidbody.angularDrag = 0.05f;
        this.rigidbody.drag = 1f;

        this.hasBlockBeenMovedByPlayerRecently = false;
    }

    public IEnumerator MoveBlockToDropPosition()
    {
        this.donePositioning = false;
        var distToDropPos = Vector3.Distance(transform.position, new Vector3(0, Camera.main.GetComponent<CameraControl>().maxHeight - 1f, 0));
        while (distToDropPos > 1f)
        {
            var dist = Vector3.Distance(transform.position, blockStartPos);
            if (dist < 5f)
            {
                //Calculate the vector between the object and the player
                Vector3 dir = transform.position - blockStartPos;
                //Cancel out the vertical difference
                dir.y = 0;
                //Translate the object in the direction of the vector
                gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(dir.normalized.x, Camera.main.GetComponent<CameraControl>().maxHeight - 1f, 0), 5f);
            }
            else
            {
                gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, Camera.main.GetComponent<CameraControl>().maxHeight - 1f, 0), 5f);
            }
            yield return 0;
        }
        this.donePositioning = true;
        Debug.Log("positioning done");
    }

    public IEnumerator MoveBlockToDropRotation()
    {
        this.doneRotating = false;
        while (Quaternion.Angle(transform.rotation, this.originalRotation) > 1f)
        {
            gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, this.originalRotation, 100f);
            yield return 0;
        }
        Debug.Log("rotation done");
        this.doneRotating = true;
    }

    //Event which triggers when collision state for a block's rigidbody doesn't change
    //Changes state variables depending on what block keeps in contact with.
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "GroundPlane")
        {
            isBlockTouchingGround = true;
            nBlocksOnGround++;
        }
        else if (other.gameObject.tag == this.blockObjTag)
        {
            if (this.isBeingPlacedOnTop)
            {
            } else {
                blocksTouching = true;

            }
        }

    }

    //Event which triggers when block stops colliding with another object
    //TODO - see if this code is actually redundant
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "GroundPlane")
        {
            isBlockTouchingGround = false;
            nBlocksOnGround--;
        }
        else if (other.gameObject.tag == this.blockObjTag)
        {
            blocksTouching = false;
        }
    }

    //Event for when user clicks on block object
    private void OnMouseDown()
    {
        if (this.isInDropPosition) 
        {
            this.rigidbody.useGravity = true;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
        this.startTime = Time.time;
        if(this.mouseStartPos == new Vector3(0,0,0)) this.mouseStartPos = Input.mousePosition;
    }

    private void OnMouseOver()
    {
        if ((!Input.GetMouseButton(0) && !this.isBeingDragged && !this.isBeingNudged) ||
            (Input.GetMouseButton(0) && (this.isBeingDragged || this.isBeingNudged)))
        {
            this.isActive = true;
        }

        if(Input.GetMouseButtonUp(0))
        {
            OnLeftMouseUp();
        }
    }

    private void OnMouseExit()
    {
        if (!this.isBeingNudged && !this.isBeingDragged)
        {
            this.isActive = false;
            this.startTime = 0;
        }
    }

    //Checks if the mouse has moved enough for the user to be able to drag it.
    public bool mouseMovedEnoughToDrag()
    {
        Vector3 changedMousePos = Input.mousePosition - this.mouseStartPos;
        bool mouseMovedEnough = (Mathf.Abs(changedMousePos.x) > this.mouseDriftPermittedToNudge || Mathf.Abs(changedMousePos.y) > this.mouseDriftPermittedToNudge || Mathf.Abs(changedMousePos.z) > this.mouseDriftPermittedToNudge);
        return mouseMovedEnough;
    }

    public bool enoughTimeHasEllapsed()
    {
        if (this.startTime == 0) return false;
        var endTime = Time.time;
        var timeDiff = Mathf.Abs(endTime - this.startTime);
        return timeDiff > 1f;
    }

    //Event for when user releases mouse
    private void OnLeftMouseUp()
    {
        if (this.userCanNudge && (this.startTime != 0) && !enoughTimeHasEllapsed())
        {
            DragBoxTool.destroyAllRigidBodies();
            this.NudgeBlock();
        }

        if (this.userCanDrag && this.isBeingPlacedOnTop) {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        if(this.outline != null) this.outline.updateOutlineState(CollisionColourState.none);

        this.mouseStartPos = new Vector3(0, 0, 0);
        if(this.isActive) this.isActive = false;
    }

    //Takes raycast of user's mouse relative to where the block was clicked, finds which face of the block was touched on, then forces the block in that direction.
    private void NudgeBlock()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject blockHit = hit.collider.gameObject;
            if (blockHit != null && (blockHit.GetInstanceID() == gameObject.GetInstanceID()))
            {
                this.NudgeBlockByFaceEdge(this.GetHitFace(hit));
                this.cursorInstance.DoCursorNudgeEffect(hit);
                this.cursorInstance.playSoundAfterDelay(this.soundEmitter);
            }
        }
    }

    IEnumerator DrawNudgeTrajectory(float timeLimit, RaycastHit hit, Ray ray, Vector3 traj)
    {
        // This will wait 1 second like Invoke could do, remove this if you don't need it
        yield return new WaitForSeconds(1);


        float timePassed = 0;
        while (timePassed < timeLimit)
        {
            // Code to go left here

            DrawLine.Draw(this.lineRenderer, traj, ray.GetPoint(10f), Color.cyan, (Time.deltaTime * 3f));
            timePassed += Time.deltaTime;

            yield return null;
        }
    }

    public enum MCFace
    {
        None,
        Up,
        Down,
        East,
        West,
        North,
        South
    }

    //Returns which face of a block was hit by the mouse. Uses enum defined above to define the 6 sides.
    public MCFace GetHitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;
        Vector3 roundedVec = new Vector3(Mathf.Round(incomingVec.x), Mathf.Round(incomingVec.y), Mathf.Round(incomingVec.z));

        if (roundedVec == new Vector3(1, -1, 0))
            return MCFace.South;

        //East
        if (roundedVec == new Vector3(-1, -1, 0))
            return MCFace.North;

        if (roundedVec == new Vector3(0, 0, 0))
            return MCFace.Up;

        if (roundedVec == new Vector3(0,-2,0))
            return MCFace.Down;

        if (roundedVec == new Vector3(0, -1, -1))
            return MCFace.West;

        if (roundedVec == new Vector3(0,-1, 1))
            return MCFace.East;

        return MCFace.None;

    }
    //Pushes block based on which face was hit, in the direction OF that face.
    //Think of it like a bullet going through something, where we determine the direction of the bullet coming out of the exit wound, based on where the entry wound is.
    private void NudgeBlockByFaceEdge(MCFace faceHit)
    {
        switch (faceHit)
        {
            case MCFace.South:
                this.pushBlock(new Vector3(-1, 0, 0));
                break;

            case MCFace.North:
                this.pushBlock(new Vector3(1, 0, 0));

                break;

            case MCFace.Up:
                this.pushBlock(new Vector3(0, -1, 0));
                break;

            case MCFace.Down:
                this.pushBlock(new Vector3(1, 1, 1));
                break;

            case MCFace.West:
                this.pushBlock(new Vector3(0, 0, 1));
                break;

            case MCFace.East:
                this.pushBlock(new Vector3(0, 0, -1));
                break;

        }
    }

    //Adds force to rigidbody so that block is moved in a direction, like it's being shoved.
    private void pushBlock(Vector3 velocity)
    {
        this.hasBlockBeenMovedByPlayerRecently = true;
        var adjustedVelocity = new Vector3(velocity.x * this.nudgeForce, velocity.y, velocity.z * this.nudgeForce);
        this.rigidbody.velocity = adjustedVelocity * this.nudgeForce;
    }


    //TODO: Find more uses for other outline colours
    private void checkOutlineState()
    {
        if (this.isActive)
        {
            if((this.isBeingDragged || this.isBeingNudged))
            {
                this.outline.updateOutlineState(CollisionColourState.green);
            } else 
            {
                this.outline.updateOutlineState(CollisionColourState.blue);
            }
        } else
        {
            if (this.outline != null) this.outline.updateOutlineState(CollisionColourState.none);
        }
    }

    public enum CollisionColourState
    {
        none,
        blue, //000BFF
        yellow, //FFEB00
        orange, //FF7D00
        red, //FF0500
        green
    }
}