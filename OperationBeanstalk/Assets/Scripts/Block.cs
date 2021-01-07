using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool hasBlockBeenMoved { get; set; } = false;
    public bool isBlockTouchingGround { get; set; } = false;

    public bool blocksTouching { get; set; } = false;

    public bool isBeingNudged = false;

    public bool blockIsInTowerZone = true;

    public bool rotating = false;

    public bool userCanDrag = false;
    
    public bool isBeingPlacedOnTop = false;

    public bool blockIsBeingDragged = false;
    public static int nBlocksOnGround { get; set; } = 0;

    public BoxCollider towerZone;

    public float nudgeForce = 500f;

    public float timeSpentNotTouching = 0f;

    private float rotationTransitionTime = 1f;

    private float startTime;
    private Vector3 mouseStartPos = new Vector3(0,0,0);

    private Vector3 blockStartPos;
    public float mouseDriftPermittedToNudge = 100f;

    private CameraControl cam;
    private Quaternion originalRotation;

    private string blockObjTag = "Block";

    public block_text_debug text_debug;

    private void Awake()
    {
        this.blockStartPos = gameObject.transform.position;
        this.originalRotation = transform.rotation;
        towerZone = GameObject.Find("Tower").GetComponent<BoxCollider>();
        this.gameObject.name = blockObjTag + GetInstanceID().ToString();
        this.cam = GameObject.Find("Main Camera").GetComponent<CameraControl>();
        this.text_debug = gameObject.GetComponentInChildren<block_text_debug>();
    }
        
    //Runs every frame for each block. Does different actions depending on which states are enabled/disabled.
    void Update()
    {
        //If the block isn't touching another block, or the ground
        if (!this.blocksTouching && !this.isBlockTouchingGround)
        {
            //If block is not being rotated to a neutral position AND
            //block is not being dropped from the top - another custom game state
            if (!this.rotating && !this.isBeingPlacedOnTop)
            {
                //Check how long it's been since block has touched anything (ground, other blocks)
                //If it's been longer than 1.5f (time), switch Main Camera to a drop view.
                if ((this.timeSpentNotTouching > 0 && transform.rotation != this.originalRotation) && Input.GetMouseButton(0))
                {
                    var currentTime = Time.time;
                    var timeDiff = Mathf.Abs(this.timeSpentNotTouching - currentTime);
                    if (timeDiff > 1.5f)
                    {
                        this.userCanDrag = false;
                        this.isBeingPlacedOnTop = true;
                        this.cam.pivotToDropView();
                    }
                }
                else
                {
                    this.timeSpentNotTouching = Time.time;

                }
            //Checks if block is being dropped on top of the tower
            }
            else if (this.isBeingPlacedOnTop)

            {
                //I forget wtf this does but it could be important!?
                if (transform.position != new Vector3(transform.position.x, Camera.main.GetComponent<CameraControl>().maxHeight - 1f, 0))
                {
                    //transform.position = Vector3.MoveTowards(transform.position, new Vector3((blockStartPos.x + transform.position.x), blockStartPos.y, 0), Time.deltaTime * -100);

                    var dist = Vector3.Distance(transform.position, blockStartPos);

                    if (dist < 5f)
                    {
                        //Calculate the vector between the object and the player
                        Vector3 dir = transform.position - blockStartPos;
                        //Cancel out the vertical difference
                        dir.y = 0;
                        //Translate the object in the direction of the vector
                        //gameObject.transform.Translate(dir.normalized * 1f);
                        gameObject.transform.position = Vector3.MoveTowards(transform.position, dir.normalized, Time.deltaTime * -10);
                    }
                    else
                    {
                        gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, Camera.main.GetComponent<CameraControl>().maxHeight - 1f, 0), Time.deltaTime * 10);
                        //transform.Translate(new Vector3(0, Camera.main.GetComponent<CameraControl>().maxHeight - 1f, 0).normalized);
                    }
                    //transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, Camera.main.GetComponent<CameraControl>().maxHeight - 1f, 0), Time.deltaTime * 10);
                } else {
                    this.isBeingPlacedOnTop = false;
                }

                //Intended to stop collision between rigidbody and block, but idt it does anything yet
                if (this.rotating)
                {
                    //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    this.userCanDrag = false;
                }
            }

            //If we're holding left mouse, and have moved the block enough to actually change the block's position when dragging, we cannot nudge it
            if (!Input.GetMouseButtonDown(0) || this.mouseMovedEnoughToDrag())
            {
                this.isBeingNudged = false;
            } 

            //Else, we can nudge it!
            //TODO - make this an else to the if above
            if(!this.userCanDrag && Input.GetMouseButtonDown(0) && this.mouseMovedEnoughToDrag())
            {
                this.userCanDrag = true;
            } 
        }
    }

    //This operates independantly of other code and will execute until the condition is met. This is intended to rotate the block to a neutral position
    //so it can be dropped on top.
    private IEnumerator Rotate()
    {
        Debug.Log("Rotating");
        rotating = true;
        for (float t = 0; t < this.rotationTransitionTime; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, this.originalRotation, t / this.rotationTransitionTime);
            yield return null;
        }
        rotating = false;
    }

    //Much like Rotate, this will move a block to the drop position uninterrupted.
    //TODO - ensure the block's move path isn't THROUGH the tower

        /**
    private IEnumerator moveBlockToDropPositionX()
    {
        Debug.Log("movingX!");
        Vector3 dropPosition = new Vector3(0, Camera.main.GetComponent<CameraControl>().maxHeight - 1f, 0);
        transform.position = Vector3.MoveTowards(transform.position, dropPosition, Time.deltaTime*10);
        this.isBeingPlacedOnTop = true;


        yield return null;
    }

    private IEnumerator moveBlockToDropPositionY()
    {
        Debug.Log("movingY!");
        Vector3 dropPosition = new Vector3((blockStartPos.x + transform.position.x) , blockStartPos.y, 0);
        transform.position = Vector3.MoveTowards(transform.position, dropPosition, Time.deltaTime * 40);
        this.isBeingPlacedOnTop = true;
        yield return null;
    }
    **/

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
                //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            } else {
                this.cam.showDropPosition = false;
                this.isBeingPlacedOnTop = false;
                blocksTouching = true;

            }
        }

    }

    //Event which triggers when block stops colliding with another object
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
        if (!this.userCanDrag && !this.rotating && this.isBeingPlacedOnTop)
        {
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
        this.startTime = Time.time;
        if(this.mouseStartPos == new Vector3(0,0,0)) this.mouseStartPos = Input.mousePosition;
    }

    //Checks if the mouse has moved enough for the user to be able to drag it.
    public bool mouseMovedEnoughToDrag()
    {
        Vector3 changedMousePos = Input.mousePosition - this.mouseStartPos;
        bool mouseMovedEnough = (Mathf.Abs(changedMousePos.x) > this.mouseDriftPermittedToNudge || Mathf.Abs(changedMousePos.y) > this.mouseDriftPermittedToNudge || Mathf.Abs(changedMousePos.z) > this.mouseDriftPermittedToNudge);
        //Debug.Log(changedMousePos);
        return mouseMovedEnough;
    }

    //Event for when user releases mouse
    private void OnMouseUp()
    {
        if (this.startTime > 0)
        {
            var endTime = Time.time;
            var timeDiff = Mathf.Abs(this.startTime - endTime);

            if (timeDiff < 1f)
            {
                DragBox.destroyAllRigidBodies();
                this.NudgeBlock();
            }
        }

        if (this.userCanDrag)
        {
            if (this.isBeingPlacedOnTop)
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }

        this.mouseStartPos = new Vector3(0, 0, 0);
    }

    //Takes raycast of user's mouse relative to where the block was clicked, finds which face of the block was touched on, then forces the block in that direction.
    private void NudgeBlock()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit ray;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ray);
        this.NudgeBlockByFaceEdge(this.GetHitFace(ray));
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
       
        if (roundedVec == new Vector3(0, -1, -1))
            return MCFace.South;

        if (roundedVec == new Vector3(0, -1, 1))
            return MCFace.North;

        if (roundedVec == new Vector3(0, 0, 0))
            return MCFace.Up;

        if (roundedVec == new Vector3(1, 1, 1))
            return MCFace.Down;

        if (roundedVec == new Vector3(-1.0f, -1.0f, 0.0f))
            return MCFace.West;

        if (roundedVec == new Vector3(1, -1, 0))
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
                this.pushBlock(new Vector3(0, -1, 1));
                break;

            case MCFace.North:
                this.pushBlock(new Vector3(0, -1, -1));

                break;

            case MCFace.Up:
                this.pushBlock(new Vector3(0, 0, 0));
                break;

            case MCFace.Down:
                this.pushBlock(new Vector3(1, 1, 1));
                break;

            case MCFace.West:
                this.pushBlock(new Vector3(1, -1, 0));
                break;

            case MCFace.East:
                this.pushBlock(new Vector3(-1.0f, -1.0f, 0.0f));
                break;

            default:
                break;

        }
    }

    //Adds force to rigidbody so that block is moved in a direction, like it's being shoved.
    private void pushBlock(Vector3 velocity)
    {
        this.hasBlockBeenMoved = true;
        var adjustedVelocity = new Vector3(velocity.x * this.nudgeForce, velocity.y * this.nudgeForce, velocity.z * this.nudgeForce);
        this.GetComponent<Rigidbody>().velocity = adjustedVelocity * this.nudgeForce;
    }
}