using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

    public GameObject particleSystemInstance;
    GameObject localInstance;
    private Camera viewCamera;
    private RaycastHit hit;
    public Quaternion cursorRotation;

    public float cursorEffectDelay;

    // Start is called before the first frame update
    void Start()
    {
        //viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
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
}
