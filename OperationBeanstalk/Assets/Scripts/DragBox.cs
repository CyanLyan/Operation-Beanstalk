using UnityEngine;
using System.Collections;

public class DragBox : MonoBehaviour
{
    public float spring = 50.0f;
    public float damper = 5.0f;
    public float drag = 10.0f;
    public float angularDrag = 5.0f;
    public float distance = 0.2f;
    public bool attachToCenterOfMass;
    public Camera mainCamera;

    private SpringJoint springJoint;

    void Update()
    {
        // Make sure the user pressed the mouse down
        if (!Input.GetMouseButtonDown(0))
            return;

        mainCamera = FindCamera();

        // We need to actually hit an object
        RaycastHit hit;
        if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            return;
        }
        // We need to hit a rigidbody that is not kinematic
        if (!hit.rigidbody || hit.rigidbody.isKinematic)
        {
            return;
        }

        if (!springJoint)
        {
            GameObject go = new GameObject("Rigidbody dragger");
            Rigidbody body = go.AddComponent<Rigidbody>() as Rigidbody;
            springJoint = go.AddComponent<SpringJoint>() as SpringJoint;
            body.isKinematic = true;
        }

        springJoint.transform.position = hit.point;
        if (attachToCenterOfMass)
        {
            Vector3 anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
            anchor = springJoint.transform.InverseTransformPoint(anchor);
            springJoint.anchor = anchor;
        }
        else
        {
            springJoint.anchor = Vector3.zero;
        }

        springJoint.spring = spring;
        springJoint.damper = damper;
        springJoint.maxDistance = distance;
        springJoint.connectedBody = hit.rigidbody;
        
        /**
        if (springJoint != null && springJoint.anchor != null && springJoint.connectedBody != null && springJoint.connectedBody.position != null)
        { 
            Debug.Log("DrawLine Running");
            DrawLine.Draw(mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(distance), springJoint.connectedBody.position, Color.cyan, 0.1f);
        }
        **/

        
        StartCoroutine("DragTheBox", hit.distance);
    }

    IEnumerator DragTheBox(float distance)
    {
        float oldDrag = springJoint.connectedBody.drag;
        float oldAngularDrag = springJoint.connectedBody.angularDrag;
        springJoint.connectedBody.drag = drag;
        
        springJoint.connectedBody.angularDrag = angularDrag;
        mainCamera = FindCamera();

        while (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            springJoint.transform.position = ray.GetPoint(distance);
            DrawLine.Draw(ray.GetPoint(distance), springJoint.connectedBody.position, Color.cyan, 0.1f);
            yield return distance;
        }

        if (springJoint.connectedBody)
        {
            springJoint.connectedBody.drag = oldDrag;
            springJoint.connectedBody.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        }

    }

    public Camera FindCamera()
    {
        Camera camera = GetComponent<Camera>();
        if (camera)
            return camera;
        else
            return Camera.main;
    }
}