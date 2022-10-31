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

    // Start is called before the first frame update
    void Start()
    {
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                cursorRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                // If the ray hits something, set the position to the hit point and rotate based on the normal vector of the hit                
                localInstance = Object.Instantiate(particleSystemInstance, hit.point, cursorRotation);
                localInstance.SetActive(true);

            }
        }
    }
    }
