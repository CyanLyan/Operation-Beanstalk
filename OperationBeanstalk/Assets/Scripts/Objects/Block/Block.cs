using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Block : InteractiveGameObject
{
    // Block states
    public bool hasBlockBeenMovedByPlayerRecently { get; set; }
    public bool isBlockTouchingGround { get; set; }
    public bool blocksTouching { get; set; }
    
    public bool blockIsInTowerZone = true;
    
    public bool isInDropPosition;

    public bool userIsPlacingBlockOnTop { get; set; }
    public static int nBlocksOnGround { get; set; }

    public Vector3 blockStartPos;
    
    public BlockMover blockMover;
    public DragBoxTool2 dragBoxTool;

    public bool isBeingPlaced;
    public bool hasBeenPlaced;

    public GameObject BlockMoverObj;

    public float nudgeForce = 500f;

    public DropBlock dropBlock;

    private GameController GameController;

    public GameObject particleSystemInstance;

    //Function to call instead of Awake/Start, should be faster as it already has access to these components
    public void Init(GameController gameController,
                 BlockMover blockMover,
                 float mouseDriftNeededForNudge,
                 float timeOnMouseDownNeededForNudge)
    {
        this.GameController = gameController;
        blockStartPos = gameObject.transform.position;
        originalRotation = transform.rotation;
        outline = GetComponent<Outline>();
        rigidbody = GetComponent<Rigidbody>();
        gameObject.name = tag + GetInstanceID();
        isInDropPosition = false;
        mouseDriftPermittedToDrag = mouseDriftNeededForNudge;
        timeOnMouseDownNeededForDrag = timeOnMouseDownNeededForNudge;

        this.blockMover = blockMover;
        _camera = Camera.main;
        this.userCanDrag = true;
    }

    //Runs every frame for each block. Does different actions depending on which states are enabled/disabled.
    void Update()
    {
        if (outline != null) checkOutlineState();
        //if(isActive)
        //{
        //    if (!userCanDrag && !isBeingPlacedOnTop (mouseMovedEnoughToDrag() && enoughTimeHasEllapsed()))
        //    {
        //        userCanNudge= false;
        //        userCanDrag = true;
        //    }
        //}
    }

    // Called by Tower Collision box whenever a block leaves it's collision area
    // This is only triggered when a block has been properly removed
    public void HandleBlockTouchingNothing()
    {
        if ((GameController.gameReady))
        {
            // Only move block to tower top IF player moved it
            var playerHasRemovedBlock = hasBlockBeenMovedByPlayerRecently && !isBeingPlacedOnTop;
        
            //If the player removed this block OR they accidentally knocked off this block
            if(playerHasRemovedBlock || (!this.GameController.CheckIfTowerIsCollapsing()))
            {
                //Activate this once the first turn on the current tower is occuring so that we don't confuse the collider
                StopDraggingBlock();
                this.userCanDrag = false;
                isBeingPlacedOnTop = true;
                blockMover.PlaceBlockInDroppingPosition(this);
            }
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
        else if (other.gameObject.tag == tag)
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
        else if (other.gameObject.tag == tag)
        {
            blocksTouching = false;
        }
    }

    //Event for when user clicks on block object
    private void OnMouseDown()
    {
        // After pulling block out, if block is in drop position, give user control again
        mouseStartPos = Mouse.current.position.ReadValue();
        if (isInDropPosition && userCanDrag && isBeingDragged) 
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
            //OnLeftMouseUp();
            if (mouseStartPos == new Vector2(0, 0)) mouseStartPos = Mouse.current.position.ReadValue();
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


    //TODO: Replace magic number with actual block dimensions, which can be gotten through the game controller
    private void OnMouseDrag()
    {
        if(isBeingPlacedOnTop)
        {
            dropBlock.SetDropBlockPlacement(gameObject.transform.position, 1.5f);
        }
    }

    //Checks if the mouse has moved enough for the user to be able to drag it.
    public bool mouseMovedEnoughToDrag()
    {
        Vector2 changedMousePos = Mouse.current.position.ReadValue() - mouseStartPos;
        bool mouseMovedEnough = (Mathf.Abs(changedMousePos.x) > mouseDriftPermittedToDrag) || 
            (Mathf.Abs(changedMousePos.y) > mouseDriftPermittedToDrag) || 
            (Mathf.Abs(changedMousePos.x) > mouseDriftPermittedToDrag);
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
    public void OnNudgeBlockInput()
    {
        if (userCanNudge && (!mouseMovedEnoughToDrag()))
        {
            NudgeBlock();
        } else if (userCanDrag && isBeingPlacedOnTop) {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        mouseStartPos = new Vector2(0, 0);
        if(isActive) isActive = false;
    }

    //Takes raycast of user's mouse relative to where the block was clicked, finds which face of the block was touched on, then forces the block in that direction.
    public void NudgeBlock()
    {
        if (GameController.gameReady && !userIsPlacingBlockOnTop)
        {
            RaycastHit hit;
            Physics.Raycast(_camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit);
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject blockHit = hit.collider.gameObject;
                if (blockHit != null && (blockHit.GetInstanceID() == gameObject.GetInstanceID()))
                {
                    DoCursorNudgeEffect(hit);
                    playSoundAfterDelay(this.soundEmitter);
                    isBeingNudged = true;
                    NudgeEffect.PlayFeedbacks();
                    NudgeBlockByFaceEdge(GetHitFace(hit));
                    isBeingNudged = false;
                }
            }
        }
    }

    public void playSoundAfterDelay(AudioSource audioSource)
    {
        audioSource.pitch = (Random.Range(0.7f, 1f));
        audioSource.PlayDelayed(0);
    }

    public void DragBlock()
    {
        if (GameController.gameReady && this.userCanDrag)
        {
            /* TODO: Find a better way to toggle gravity on and off, the issue here is
             *       that we're disabling it in BlockMover, then enabling it again here.
             */
            if(this.isBeingPlacedOnTop)
            {
                this.ToggleGravity(false);
                userIsPlacingBlockOnTop = true;
            }
            dragBoxTool.ToggleDragBlock(true);
        }
    }

    public void StopDraggingBlock()
    {
        if(dragBoxTool.isDragging)
        {
            Debug.Log("Stop dragging");
            dragBoxTool.ToggleDragBlock(false);
            this.isBeingDragged = false;
            this.isActive = false;
            
            // If the user has dragged a block out of the tower and it's touching nothing,
            // but they HAVE NOT clicked the block again to place it
            if(!userIsPlacingBlockOnTop && !isBlockTouchingGround)
            {

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
            outline.updateOutlineState(CollisionColourState.green);
            if ((isBeingDragged || isBeingNudged))
            {
                outline.updateOutlineState(CollisionColourState.green);
            } else 
            {
                //TODO - figure out why this is sometimes null, it's possibly being deleted after activation
                if (outline != null) outline.updateOutlineState(CollisionColourState.blue);
            }
        } else
        {
            outline.updateOutlineState(CollisionColourState.none);
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

    //TODO - move this elsewhere, shouldn't be in PlayerController
    public void DoCursorNudgeEffect(RaycastHit hit)
    {
        var cursorRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        // If the ray hits something, set the position to the hit point and rotate based on the normal vector of the hit                
        var localInstance = Instantiate(particleSystemInstance, hit.point, cursorRotation);
        var particalSystem = localInstance.GetComponent<ParticleSystem>();
        var main = particalSystem.main;
        main.startDelay = 0;
        localInstance.SetActive(true);

        //gunParticleSystemObj.ToString();
        //if (gunActive) gunParticleSystemObj.SetActive(true);
    }

    public void ToggleGravity(bool enableGravity)
    {
        this.GetComponent<Rigidbody>().useGravity = enableGravity;
    }
}