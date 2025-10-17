using MoreMountains.Feedbacks;
using UnityEngine;

public class InteractiveGameObject : MonoBehaviour
{
    // Object states
    public bool hasObjectBeenMovedByPlayerRecently { get; set; }
    public bool isObjectTouchingGround { get; set; }
    public bool ObjectsTouching { get; set; }
    
    public bool isBeingNudged;

    public bool ObjectIsInTowerZone = true;
    
    public bool isBeingPlacedOnTop;
        
    public bool isBeingDragged;

    public bool isActive;

    public bool userCanDrag;
    public bool userCanNudge = true;
    public static int nObjectsOnGround { get; set; }

    public BoxCollider towerZone;

    public float timeSpentNotTouching;

    public float startTime;
    public Vector3 mouseStartPos = new Vector3(0,0,0);

    public Vector3 startPos;
    public float mouseDriftPermittedToDrag;

    public Quaternion originalRotation;

    public string tag = "Object";

    public Outline outline;

    public GameController gameController;
    public GameObject lineRenderer;

    public AudioSource soundEmitter;

    public PlayerController cursorInstance;

    public Rigidbody rigidbody { get; set; }
    
    public float timeOnMouseDownNeededForDrag;

    public DragBoxTool dragBox;

    public MMFeedbacks NudgeEffect;
    public Camera _camera;


    //Event which triggers when collision state for a object's rigidbody doesn't change
    //Changes state variables depending on what object keeps in contact with.
     

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

    //Returns which face of a object was hit by the mouse. Uses enum defined above to define the 6 sides.
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
}
