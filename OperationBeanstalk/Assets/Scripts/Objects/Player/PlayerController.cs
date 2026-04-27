using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;

public class PlayerController : MonoBehaviour
{

    public GameObject particleSystemInstance;

    public List<GameObject> CursorParts;
    public GameObject defaultCursorObj;
    private Camera viewCamera;
    public Quaternion cursorRotation;

    public GameObject laserBeam;

    public float cursorEffectDelay;

    //public GameObject cursorPrefab;
    public float maxCursorDistance = 30;

    public bool gunActive;
    public GameObject gunPrefab;
    public GameObject gunParticleSystemObj;
    public float distanceAwayFromSurface = 3f;
    public bool InGunDisplayLoop { get; private set; }

    public Color color { get; set; }
    public string playerName { get; set; }
    public int score { get; set; }

    private bool isHolding { get; set; }

    public PlayerInputActions playerControls;
    public CameraController cameraController;

    private Block lastBlockHit;
    public PlayerController(Color color, string name, int score = 0)
    {
        this.color = color;
        playerName = name;
        this.score = score;
    }
    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        isHolding = false;
        playerControls.Player.Fire.started += context =>
        {
            isHolding = false;
            //if(lastBlockHit)
            //{
            //    lastBlockHit.isBeingDragged = false;
            //    lastBlockHit.isActive = false;
            //}
        };
        playerControls.Player.Fire.performed += FirePerformed;

        // TODO: Put this into a function
        playerControls.Player.Fire.canceled += context =>
        {
            if (lastBlockHit)
            {
                lastBlockHit.StopDraggingBlock();
            }

            // TODO: Gotta find a better way to do this? Gravity toggle in function?
            if(lastBlockHit.isBeingPlacedOnTop && lastBlockHit.userIsPlacingBlockOnTop)
            {
                lastBlockHit.ToggleGravity(true);
            }
        };
        playerControls.Enable();

        playerControls.Player.CameraRotateLeft.started += CameraRotateLeft;
        playerControls.Player.CameraRotateRight.started += CameraRotateRight;
        
        playerControls.Player.CameraRotateLeft.canceled += StopCamera;
        playerControls.Player.CameraRotateRight.canceled += StopCamera;

        playerControls.Player.CameraZoomIn.started += CameraZoomIn;
        playerControls.Player.CameraZoomOut.started += CameraZoomOut;

        playerControls.Player.CameraZoomIn.canceled += StopCamera;
        playerControls.Player.CameraZoomOut.canceled += StopCamera;
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Block getBlockHitByUserAction()
    {
        Ray ray = viewCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Block blockHit = hit.collider.GetComponent<Block>();
            return blockHit;
        }
        return null;
    }

    private void FirePerformed(InputAction.CallbackContext context)
    {
        var blockHit = getBlockHitByUserAction();
        if(blockHit)
        {
            lastBlockHit = blockHit;
            if(context.interaction is TapInteraction && !isHolding)
            {
                blockHit.NudgeBlock();
            
            } else if (context.interaction is HoldInteraction)
            {
                isHolding = true;
                blockHit.DragBlock();
            }
        }
    }

    private void CameraRotateLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cameraController.SetCameraInputKey(Key.LeftArrow);
        }
    }

    private void CameraRotateRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cameraController.SetCameraInputKey(Key.RightArrow);
        }
    }

    private void CameraZoomIn(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cameraController.SetCameraInputKey(Key.UpArrow);
        }
    }

    private void CameraZoomOut(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cameraController.SetCameraInputKey(Key.DownArrow);
        }
    }


    private void StopCamera(InputAction.CallbackContext context)
    {
        cameraController.SetCameraInputKey(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        viewCamera = Camera.main;
    }

    private void Update()
    {
        UpdateCursorBasedOnMouse();
    }

    private void UpdateCursorBasedOnMouse()
    {
        Ray ray = viewCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Block blockHit = hit.collider.GetComponent<Block>();
            if(blockHit)
            {
                if (lastBlockHit && (blockHit != lastBlockHit)) lastBlockHit.isActive = false;
                lastBlockHit = blockHit;
                blockHit.isActive = true;
            }
            // If the ray hits something, set the position to the hit point and rotate based on the normal vector of the hit       
            defaultCursorObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            defaultCursorObj.transform.position = hit.point;
            DisplayDefaultCursor(true);
            Cursor.visible = false;
        }
        else
        {
            if (lastBlockHit) lastBlockHit.isActive = false;
            // If the ray doesn't hit anything, set the position to the maxCursorDistance and rotate to point away from the camera
            defaultCursorObj.transform.position = ray.origin + ray.direction.normalized * maxCursorDistance;
            defaultCursorObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
            DisplayDefaultCursor(false);
            Cursor.visible = true;
        }
    }

    private void PointGunAtBlock()
    {
        Ray ray = viewCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Quaternion newGunRotation = Quaternion.FromToRotation(Vector3.zero, hit.normal);

            DrawLine.Draw(laserBeam, hit.point, hit.point, Color.cyan, (Time.deltaTime * 1.5f));
            gunPrefab.transform.position = hit.point + hit.normal * distanceAwayFromSurface;
 
            gunPrefab.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.point - gunPrefab.transform.position);

            Cursor.visible = false;
        }
        else
        {
            gunPrefab.transform.position = ray.origin + ray.direction.normalized * maxCursorDistance;
            gunPrefab.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
            DisplayDefaultCursor(false);
            Cursor.visible = true;
        }
    }

    private void DisplayDefaultCursor(bool a)
    {
        CursorParts.ForEach(part => part.SetActive(a));
    }
}
