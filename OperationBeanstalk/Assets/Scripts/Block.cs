using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool hasBlockBeenMoved {get; set;} = false;
    public bool isBlockTouchingGround {get; set;} = false;

    public bool blocksTouching { get; set; } = false;
    public static int nBlocksOnGround {get; set;} = 0;

    public BoxCollider towerZone;

    private float nudgeForce = 2f;

    public bool isBeingNudged = false;

    public float timeSpentNotTouching = 0f;

    private Quaternion originalRotation;

    private float rotationTransitionTime = 1f;

    private bool rotating = false;

    public bool userCanDrag = true;

    private CameraControl cam;

    private float startTime;

    private void Awake()
    {
        this.originalRotation = transform.rotation;
        towerZone = GameObject.Find("Tower").GetComponent<BoxCollider>();
        this.gameObject.name = "block" + GetInstanceID().ToString();
        this.cam = GameObject.Find("Main Camera").GetComponent<CameraControl>();

    }

    void Update()
    {

        if (!this.blocksTouching && !this.isBlockTouchingGround && !this.rotating)
        {
                if (this.timeSpentNotTouching > 0 && transform.rotation != this.originalRotation)
                {
                    var currentTime = Time.time;
                    var timeDiff = Mathf.Abs(this.timeSpentNotTouching - currentTime);
                    if (timeDiff > 2f)
                    {
                    this.userCanDrag = false;
                    StartCoroutine("Rotate", this.originalRotation.eulerAngles);
                    StartCoroutine("moveBlockToDropPosition");
                    this.cam.pivotToDropView();
                    }
                }
                else
                {
                    this.timeSpentNotTouching = Time.time;
                }
            }

        if (!Input.GetMouseButtonDown(0))
        {
            this.isBeingNudged = false;
        }
    }

    private IEnumerator Rotate()
    {
        Debug.Log("Rotating");
        rotating = true;
        for (float t = 0; t < this.rotationTransitionTime; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, this.originalRotation, t / this.rotationTransitionTime);
            yield return null;
        }
        //transform.rotation = this.originalRotation;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        rotating = false;
    }

    private IEnumerator moveBlockToDropPosition()
    {
        Vector3 dropPosition = new Vector3(0, Camera.main.GetComponent<CameraControl>().maxHeight + 10f, 0);
        transform.position = dropPosition;
        yield return null;
    }

    void OnCollisionStay(Collision other)
    {
        if(other.gameObject.tag == "GroundPlane")
        {
            isBlockTouchingGround = true;
            nBlocksOnGround++;
        } else if (other.gameObject.tag == "Block")
        {
            blocksTouching = true;
        }

    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "GroundPlane")
        {
            isBlockTouchingGround = false;
            nBlocksOnGround--;
        } else if (other.gameObject.tag == "Block")
        {
            //Debug.Log("No touching");
            blocksTouching = false;

        }
    }

    private void OnMouseDown()
    {
        this.startTime = Time.time;
    }

    private void OnMouseUp()
    {
        if (this.startTime > 0)
        {
            var endTime = Time.time;
            var timeDiff = Mathf.Abs(this.startTime - endTime);

            if (timeDiff < 0.5f)
            {
                this.NudgeBlock();
            }
            Debug.Log(Mathf.Abs(this.startTime - endTime));

        }
    }

    private void NudgeBlock()
    {
        this.isBeingNudged = true;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit ray;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ray);
        Debug.Log(this.GetHitFace(ray));
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

    public MCFace GetHitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;
        Vector3 roundedVec = new Vector3(Mathf.Round(incomingVec.x), Mathf.Round(incomingVec.y), Mathf.Round(incomingVec.z));
        //Debug.Log(new Vector3(Mathf.Round(incomingVec.x), Mathf.Round(incomingVec.y), Mathf.Round(incomingVec.z)));

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

    private void NudgeBlockByFaceEdge(MCFace faceHit)
    {
        switch(faceHit)
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

    private void pushBlock(Vector3 velocity)
    {
        var adjustedVelocity = new Vector3(velocity.x * this.nudgeForce, velocity.y * this.nudgeForce, velocity.z * this.nudgeForce);
        this.GetComponent<Rigidbody>().velocity = adjustedVelocity* this.nudgeForce;
    }
}
