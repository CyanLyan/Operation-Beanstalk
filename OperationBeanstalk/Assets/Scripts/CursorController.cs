using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

    public GameObject particleSystemInstance;

    public List<GameObject> CursorParts;
    GameObject localInstance;
    private Camera viewCamera;
    private RaycastHit hit;
    public Quaternion cursorRotation;

    public float cursorEffectDelay;

    //public GameObject cursorPrefab;
    public float maxCursorDistance = 30;


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
        UpdateCursorBasedOnMouse();
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
            Debug.Log("Hit");
            // If the ray hits something, set the position to the hit point and rotate based on the normal vector of the hit
            gameObject.transform.position = hit.point;
            gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            DisplayCursor(true);
            Cursor.visible = false;
        }
        else
        {
            Debug.Log("Miss");
            // If the ray doesn't hit anything, set the position to the maxCursorDistance and rotate to point away from the camera
            gameObject.transform.position = ray.origin + ray.direction.normalized * maxCursorDistance;
            gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
            DisplayCursor(false);
            Cursor.visible = true;
        }
    }

    private void DisplayCursor(bool a)
    {
        CursorParts.ForEach(part => part.SetActive(a));
    }
}
