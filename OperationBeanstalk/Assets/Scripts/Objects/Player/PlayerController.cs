using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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

    private InputAction playerDrag;
    private InputAction playerMoveCamera;
    private InputAction playerNudge;

    public Color color { get; set; }
    public string playerName { get; set; }
    public int score { get; set; }

    public PlayerInputActions playerControls;
    public CameraController cameraController;
    public PlayerController(Color color, string name, int score = 0)
    {
        this.color = color;
        playerName = name;
        this.score = score;
    }
    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        playerControls.Enable();
        playerNudge = playerControls.Player.Fire;
        playerControls.Player.CameraRotate.started += CameraRotate;
        playerControls.Player.CameraRotate.canceled += StopCamera;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerNudge.Disable();
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }

    private void Move(InputAction.CallbackContext context)
    {
        CameraRotate(context);
    }

    private void CameraRotate(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        //if (userCanMoveCamera)
        //{
        cameraController.SetCameraInputKey(((KeyControl)context.control).keyCode);
        //}
    }

    private void StopCamera(InputAction.CallbackContext context)
    {
        Debug.Log("STOP");
        Debug.Log(context);
        cameraController.SetCameraInputKey(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Todo - move to fire function?
        //gunActive = Input.GetMouseButton(1);
        //gunPrefab.SetActive(gunActive);
        //UpdateCursorBasedOnMouse();
        //if (gunActive)
        //{
        //    PointGunAtBlock();
        //}
    }

    public void DoCursorNudgeEffect(RaycastHit hit)
    {
        var cursorRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        // If the ray hits something, set the position to the hit point and rotate based on the normal vector of the hit                
        var localInstance = Instantiate(particleSystemInstance, hit.point, cursorRotation);
        var particalSystem = localInstance.GetComponent<ParticleSystem>();
        var main = particalSystem.main;
        main.startDelay = cursorEffectDelay;
        localInstance.SetActive(true);

        gunParticleSystemObj.ToString();
        if(gunActive) gunParticleSystemObj.SetActive(true);
    }

    public void playSoundAfterDelay(AudioSource audioSource)
    {
        randomizeAudioPitch(audioSource).PlayDelayed(cursorEffectDelay);
    }

    public AudioSource randomizeAudioPitch(AudioSource audioSource)
    {
        audioSource.pitch = (Random.Range(0.7f, 1f));
        return audioSource;
    }

    private void UpdateCursorBasedOnMouse()
    {
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // If the ray hits something, set the position to the hit point and rotate based on the normal vector of the hit       
            defaultCursorObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            defaultCursorObj.transform.position = hit.point;
            DisplayDefaultCursor(true);
            Cursor.visible = false;
        }
        else
        {
            // If the ray doesn't hit anything, set the position to the maxCursorDistance and rotate to point away from the camera
            defaultCursorObj.transform.position = ray.origin + ray.direction.normalized * maxCursorDistance;
            defaultCursorObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
            DisplayDefaultCursor(false);
            Cursor.visible = true;
        }
    }

    private void PointGunAtBlock()
    {
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
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
