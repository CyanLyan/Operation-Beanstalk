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

    private bool _isInDropPosition = false;
    public bool isInDropPosition
    {
        get { return _isInDropPosition; }
        set { _isInDropPosition = value; }
    }

    private GameObject towerDropZone;
    public bool isBeingDragged = false;

    public bool isActive = false;

    public bool userCanDrag = false;
    public bool userCanNudge = true;
    public static int nBlocksOnGround { get; set; } = 0;

    public BoxCollider towerZone;

    public float nudgeForce = 500f;

    public float timeSpentNotTouching = 0f;

    private float startTime;
    private Vector3 mouseStartPos = new Vector3(0,0,0);

    private Vector3 blockStartPos;
    private float mouseDriftPermittedToDrag;

    private CameraController cam;
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
    private float timeOnMouseDownNeededForDrag;
    public bool hasBeenPlaced;

    public DragBoxTool dragBox;

    //Function to call instead of Awake/Start, should be faster as it already has access to these components
    public void Init(GameController gameController, 
                 CameraController cam,
                 CursorController cursorInstance,
                 GameObject towerDropZone,
                 float mouseDriftNeededForNudge,
                 float timeOnMouseDownNeededForNudge)
    {
        this.gameController = gameController;
        this.blockStartPos = gameObject.transform.position;
        this.originalRotation = transform.rotation;
        this.cam = cam;
        this.outline = this.GetComponent<Outline>();
        this.cursorInstance = cursorInstance;
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.gameObject.name = blockObjTag + GetInstanceID().ToString();
        this.isInDropPosition = false;
        this.towerDropZone = towerDropZone;
        this.mouseDriftPermittedToDrag = mouseDriftNeededForNudge;
        this.timeOnMouseDownNeededForDrag = timeOnMouseDownNeededForNudge;
    }

    //Runs every frame for each block. Does different actions depending on which states are enabled/disabled.
    void Update()
    {
        checkOutlineState();
        if(this.isActive)
        {
            //If the block isn't touching another block, or the ground, and not being set up
            //if (!this.blocksTouching && !this.isBlockTouchingGround)
            //{
            //If block is not being rotated to a neutral position AND
            //block is not being dropped from the top - another custom game state

            //TODO - see if this condition below is redundant
            //If we're holding left mouse, and have moved the block enough to actually change the block's position when dragging, we cannot nudge it
            //if (!Input.GetMouseButton(0) || (this.mouseMovedEnoughToDrag() && !enoughTimeHasEllapsed()))
            //{
            //    this.hasBlockBeenMovedByPlayerRecently = true;
            //    this.isBeingNudged = false;
            //}

            //Else, we can nudge it!
            //TODO - make this an else to the if above
            //if (Input.GetMouseButton(0))
            //{
            //    Debug.Log(!this.userCanDrag && Input.GetMouseButton(0));
            //    Debug.Log(this.mouseMovedEnoughToDrag());
            //    Debug.Log(enoughTimeHasEllapsed());
            //}
            //}
            //if(this.isBeingNudged)
            //{
            //    Debug.Log(this.rigidbody.velocity.magnitude);
            //}

            if (!this.userCanDrag && !this.isBeingPlacedOnTop & Input.GetMouseButton(0) && (this.mouseMovedEnoughToDrag() && enoughTimeHasEllapsed()))
            {
                //Debug.Log("UserCanDrag");
                this.userCanDrag = true;
            }
        }
    }

    public void HandleBlockTouchingNothing()
    {
        // Only move block to tower top IF player moved it
        if(this.hasBlockBeenMovedByPlayerRecently)
        {
            //Activate this once the first turn on the current tower is occuring so that we don't confuse the collider
            PlaceBlockInDroppingPosition();
        }
    }

    public void FinishDroppingBlockInPlace()
    {
        this.isBeingPlacedOnTop = false;
        blocksTouching = true;
        this.isInDropPosition = false;
        this.userCanDrag = false;
        this.userCanNudge= false;
        this.hasBlockBeenMovedByPlayerRecently = false;
        this.rigidbody.freezeRotation = false;
        this.hasBeenPlaced = true;
        StartCoroutine(this.cam.pivotBackToPreviousView(this.cam.transform.position));
        this.gameController.FinishTurn();
    }

    public void PlaceBlockInDroppingPosition()
    {
        this.userCanDrag = false;
        this.isBeingPlacedOnTop = true;
        this.rigidbody.useGravity = false;
        this.userCanNudge = false;
        this.gameController.GoToTurnState(TurnState.PlaceBlock);
        StartCoroutine(this.cam.pivotToDropView());

        StartCoroutine(MoveBlockToDropPosition());
        StartCoroutine(MoveBlockToDropRotation());
        StartCoroutine(WaitForBlockToBePositionedAndRotated());
    }

    public IEnumerator WaitForBlockToBePositionedAndRotated()
    {
        while(!this.donePositioning || !this.doneRotating)
        {
            yield return 0;
        }
        this.isInDropPosition = true;
        this.userCanNudge = false;
        this.rigidbody.angularVelocity = new Vector3(0, 0, 0);
        this.rigidbody.velocity = new Vector3(0, 0, 0);
        this.rigidbody.angularDrag = 0.05f;
        this.rigidbody.drag = 1f;
    }

    public IEnumerator MoveBlockToDropPosition()
    {
        this.donePositioning = false;
        while ((this.towerDropZone.transform.position.y - transform.position.y > 1))
        {
            var dist = Vector3.Distance(transform.position, blockStartPos);
            if (dist < 5f)
            {
                //Calculate the vector between the object and the player
                Vector3 dir = transform.position - blockStartPos;
                //Cancel out the vertical difference
                dir.y = 0;
                //Translate the object in the direction of the vector
                gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(dir.normalized.x, this.towerDropZone.transform.position.y, 0), 5f);
            }
            else
            {
                gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, Camera.main.GetComponent<CameraController>().maxHeight - 1f, 0), 5f);
            }
            yield return 0;
        }
        this.donePositioning = true;
    }

    public IEnumerator MoveBlockToDropRotation()
    {
        this.doneRotating = false;
        while (Quaternion.Angle(transform.rotation, this.originalRotation) > 1f)
        {
            gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, this.originalRotation, 100f);
            yield return 0;
        }
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
        // After pulling block out, if block is in drop position, give user control again
        if (this.isInDropPosition && this.userCanDrag) 
        {
            this.rigidbody.useGravity = true;
        }
        this.startTime = Time.time;
        if(!this.isBeingNudged && !this.isBeingDragged) this.isActive = false;
    }

    private void OnMouseOver()
    {
        if (dragBox.isDragging && !this.isBeingDragged)
        {
            this.isActive = false;
        } else
        {
            this.isActive = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseUp();
            if (this.mouseStartPos == new Vector3(0, 0, 0)) this.mouseStartPos = Input.mousePosition;
        } 
    }

    private void OnMouseExit()
    {
        if (!this.isBeingNudged && !this.isBeingDragged && !this.isInDropPosition)
        {
            this.isActive = false;
            this.startTime = 0;
        }
    }

    //Only use this to wait for user to release mouse after block is removed, so that they don't glitch the game out
    private void OnMouseUp()
    {
        this.userCanDrag= false;
        if(this.isInDropPosition)
        {
            this.userCanDrag = true;
        }
    }

    //Checks if the mouse has moved enough for the user to be able to drag it.
    public bool mouseMovedEnoughToDrag()
    {
        Vector3 changedMousePos = Input.mousePosition - this.mouseStartPos;
        //Debug.Log(changedMousePos);
        //Debug.Log(this.mouseDriftPermittedToNudge);
        bool mouseMovedEnough = (Mathf.Abs(changedMousePos.x) > this.mouseDriftPermittedToDrag) || 
            (Mathf.Abs(changedMousePos.y) > this.mouseDriftPermittedToDrag) || 
            (Mathf.Abs(changedMousePos.z) > this.mouseDriftPermittedToDrag);
        //Debug.Log(mouseMovedEnough);
        return mouseMovedEnough;
    }

    public bool enoughTimeHasEllapsed()
    {
        if (this.startTime == 0) return false;
        var endTime = Time.time;
        var timeDiff = Mathf.Abs(endTime - this.startTime);
        return timeDiff > this.timeOnMouseDownNeededForDrag;
    }

    //Event for when user releases mouse
    private void OnLeftMouseUp()
    {
        if (this.userCanNudge && (this.startTime != 0) && !(enoughTimeHasEllapsed() && mouseMovedEnoughToDrag()))
        {
            //DragBoxTool.destroyAllRigidBodies();
            this.NudgeBlock();
        }

        if (this.userCanDrag && this.isBeingPlacedOnTop) {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

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
                this.isBeingNudged= true;
                this.NudgeBlockByFaceEdge(this.GetHitFace(hit));
                this.cursorInstance.DoCursorNudgeEffect(hit);
                this.cursorInstance.playSoundAfterDelay(this.soundEmitter);
                this.isBeingNudged = false;
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
                //TODO - figure out why this is sometimes null, it's possibly being deleted after activation
                if (this.outline != null) this.outline.updateOutlineState(CollisionColourState.blue);
            }
        } else
        {
            if (this.outline == null)
            {
                //Debug.Log(this);
            }
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