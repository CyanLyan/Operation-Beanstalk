using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

    public GameObject particleSystemInstance;

    public List<GameObject> CursorParts;
    public GameObject defaultCursorObj;
    private Camera viewCamera;
    private RaycastHit hit;
    public Quaternion cursorRotation;

    public GameObject laserBeam;

    public float cursorEffectDelay;

    //public GameObject cursorPrefab;
    public float maxCursorDistance = 30;

    public bool gunActive = false;
    public GameObject gunPrefab;
    public float distanceAwayFromSurface = 3f;
    public bool InGunDisplayLoop { get; private set; }

    public CursorController()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        gunActive = Input.GetMouseButton(1);
        gunPrefab.SetActive(gunActive);
        UpdateCursorBasedOnMouse();
        if (gunActive) PointGunAtBlock();
    }

    public void DoCursorNudgeEffect(RaycastHit hit)
    {
        var cursorRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        // If the ray hits something, set the position to the hit point and rotate based on the normal vector of the hit                
        var localInstance = Object.Instantiate(particleSystemInstance, hit.point, cursorRotation);
        var particalSystem = localInstance.GetComponent<ParticleSystem>();
        var main = particalSystem.main;
        main.startDelay = cursorEffectDelay;
        localInstance.SetActive(true);
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

            DrawLine.Draw(this.laserBeam, hit.point, hit.point, Color.cyan, (Time.deltaTime * 1.5f));
            gunPrefab.transform.position = hit.point + hit.normal * distanceAwayFromSurface;
 
            gunPrefab.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.point - gunPrefab.transform.position);


            Cursor.visible = false;
        }
        else
        {
            // If the ray doesn't hit anything, set the position to the maxCursorDistance and rotate to point away from the camera
            gunPrefab.transform.position = ray.origin + ray.direction.normalized * maxCursorDistance;
            gunPrefab.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
            DisplayDefaultCursor(false);
            Cursor.visible = true;
        }
            //yield return null;
        //}
        //this.InGunDisplayLoop = false;
    }

    private void DisplayDefaultCursor(bool a)
    {
        CursorParts.ForEach(part => part.SetActive(a));
    }
}
