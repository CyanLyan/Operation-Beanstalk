using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Block : MonoBehaviour
{
    // Block states
    public bool hasBlockBeenMovedByPlayerRecently { get; set; }
    public bool isBlockTouchingGround { get; set; }
    public bool blocksTouching { get; set; }
    
    public bool isBeingNudged;

    public bool blockIsInTowerZone = true;
    
    public bool isBeingPlacedOnTop;

    private bool _isInDropPosition;
    public bool isInDropPosition
    {
        get { return _isInDropPosition; }
        set { _isInDropPosition = value; }
    }

    
    public bool isBeingDragged;

    public bool isActive;

    public bool userCanDrag;
    public bool userCanNudge = true;
    public static int nBlocksOnGround { get; set; }

    public BoxCollider towerZone;

    public float nudgeForce = 500f;

    public float timeSpentNotTouching;

    private float startTime;
    private Vector3 mouseStartPos = new Vector3(0,0,0);

    public Vector3 blockStartPos;
    private float mouseDriftPermittedToDrag;

    public Quaternion originalRotation;

    private string blockObjTag = "Block";

    private Outline outline;

    public GameController gameController;
    public GameObject lineRenderer;

    public AudioSource soundEmitter;

    private CursorController cursorInstance;

    private Rigidbody rigidbody { get; set; }
    
    private float timeOnMouseDownNeededForDrag;
    public BlockMover blockMover;
    public bool hasBeenPlaced;

    public DragBoxTool dragBox;

    public MMFeedbacks NudgeEffect;
    private Camera _camera;

    public GameObject BlockMoverObj;

    //Function to call instead of Awake/Start, should be faster as it already has access to these components
    public void Init(GameController gameController,
                 CursorController cursorInstance,
                 BlockMover blockMover,
                 float mouseDriftNeededForNudge,
                 float timeOnMouseDownNeededForNudge)
    {
        this.gameController = gameController;
        blockStartPos = gameObject.transform.position;
        originalRotation = transform.rotation;
        outline = GetComponent<Outline>();
        this.cursorInstance = cursorInstance;
        rigidbody = GetComponent<Rigidbody>();
        gameObject.name = blockObjTag + GetInstanceID();
        isInDropPosition = false;
        mouseDriftPermittedToDrag = mouseDriftNeededForNudge;
        timeOnMouseDownNeededForDrag = timeOnMouseDownNeededForNudge;

        this.blockMover = blockMover;
        _camera = Camera.main;
    }

    //Runs every frame for each block. Does different actions depending on which states are enabled/disabled.
    void Update()
    {
        checkOutlineState();
        if(isActive)
        {
            if (!userCanDrag && !isBeingPlacedOnTop & Input.GetMouseButton(0) && (mouseMovedEnoughToDrag() && enoughTimeHasEllapsed()))
            {
                userCanNudge= false;
                userCanDrag = true;
            }
        }
    }

    // Called by Tower Collision box whenever a block leaves it's collision area
    // This is only triggered when a block has been properly removed
    public void HandleBlockTouchingNothing()
    {
        // Only move block to tower top IF player moved it
        if(hasBlockBeenMovedByPlayerRecently && !isBeingPlacedOnTop)
        {
            //Activate this once the first turn on the current tower is occuring so that we don't confuse the collider
            blockMover.PlaceBlockInDroppingPosition(this);
        } else
        {
            gameController.tower.TowerIsCollapsing();
        }
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
        else if (other.gameObject.tag == blockObjTag)
        {
            if (isBeingPlacedOnTop)
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
        else if (other.gameObject.tag == blockObjTag)
        {
            blocksTouching = false;
        }
    }

    //Event for when user clicks on block object
    private void OnMouseDown()
    {
        // After pulling block out, if block is in drop position, give user control again
        mouseStartPos = Input.mousePosition;
        if (isInDropPosition && userCanDrag) 
        {
            rigidbody.useGravity = true;
        }
        startTime = Time.time;
        if(!isBeingNudged && !isBeingDragged) isActive = false;
    }

    private void OnMouseOver()
    {
        if (dragBox.isDragging && !isBeingDragged)
        {
            isActive = false;
        } else
        {
            isActive = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseUp();
            if (mouseStartPos == new Vector3(0, 0, 0)) mouseStartPos = Input.mousePosition;
        } 
    }

    private void OnMouseExit()
    {
        if (!isBeingNudged && !isBeingDragged && !isInDropPosition)
        {
            isActive = false;
            startTime = 0;
        }
    }

    //Only use this to wait for user to release mouse after block is removed, so that they don't glitch the game out
    private void OnMouseUp()
    {
        userCanDrag= false;
        if(isInDropPosition)
        {
            userCanDrag = true;
        }
        if(!isInDropPosition) userCanNudge= true;
    }

    //Checks if the mouse has moved enough for the user to be able to drag it.
    public bool mouseMovedEnoughToDrag()
    {
        Vector3 changedMousePos = Input.mousePosition - mouseStartPos;
        bool mouseMovedEnough = (Mathf.Abs(changedMousePos.x) > mouseDriftPermittedToDrag) || 
            (Mathf.Abs(changedMousePos.y) > mouseDriftPermittedToDrag) || 
            (Mathf.Abs(changedMousePos.z) > mouseDriftPermittedToDrag);
        return mouseMovedEnough;
    }

    public bool enoughTimeHasEllapsed()
    {
        if (startTime == 0) return false;
        var endTime = Time.time;
        var timeDiff = Mathf.Abs(endTime - startTime);
        return timeDiff > timeOnMouseDownNeededForDrag;
    }

    //Event for when user releases mouse
    private void OnLeftMouseUp()
    {
        if (userCanNudge && (startTime != 0) && (!mouseMovedEnoughToDrag()))
        {
            NudgeBlock();
        } else if (userCanDrag && isBeingPlacedOnTop) {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        mouseStartPos = new Vector3(0, 0, 0);
        if(isActive) isActive = false;
    }

    //Takes raycast of user's mouse relative to where the block was clicked, finds which face of the block was touched on, then forces the block in that direction.
    private void NudgeBlock()
    {
        RaycastHit hit;
        Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit);
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject blockHit = hit.collider.gameObject;
            if (blockHit != null && (blockHit.GetInstanceID() == gameObject.GetInstanceID()))
            {
                isBeingNudged= true;
                NudgeEffect.PlayFeedbacks();
                NudgeBlockByFaceEdge(GetHitFace(hit));
                cursorInstance.DoCursorNudgeEffect(hit);
                cursorInstance.playSoundAfterDelay(soundEmitter);
                isBeingNudged = false;
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

            DrawLine.Draw(lineRenderer, traj, ray.GetPoint(10f), Color.cyan, (Time.deltaTime * 3f));
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
                pushBlock(new Vector3(-1, 0, 0));
                break;

            case MCFace.North:
                pushBlock(new Vector3(1, 0, 0));

                break;

            case MCFace.Up:
                pushBlock(new Vector3(0, -1, 0));
                break;

            case MCFace.Down:
                pushBlock(new Vector3(1, 1, 1));
                break;

            case MCFace.West:
                pushBlock(new Vector3(0, 0, 1));
                break;

            case MCFace.East:
                pushBlock(new Vector3(0, 0, -1));
                break;

        }
    }

    //Adds force to rigidbody so that block is moved in a direction, like it's being shoved.
    private void pushBlock(Vector3 velocity)
    {
        hasBlockBeenMovedByPlayerRecently = true;
        var adjustedVelocity = new Vector3(velocity.x * nudgeForce, velocity.y, velocity.z * nudgeForce);
        rigidbody.velocity = adjustedVelocity * nudgeForce;
    }


    //TODO: Find more uses for other outline colours
    private void checkOutlineState()
    {
        if (isActive)
        {
            if((isBeingDragged || isBeingNudged))
            {
                outline.updateOutlineState(CollisionColourState.green);
            } else 
            {
                //TODO - figure out why this is sometimes null, it's possibly being deleted after activation
                if (outline != null) outline.updateOutlineState(CollisionColourState.blue);
            }
        } else
        {
            if (outline == null)
            {
                //Debug.Log(this);
            }
            if (outline != null) outline.updateOutlineState(CollisionColourState.none);
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