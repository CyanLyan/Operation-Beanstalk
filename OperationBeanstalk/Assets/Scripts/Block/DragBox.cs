

using UnityEngine;
using System.Collections;

public class hitCoords: MonoBehaviour
{
    public float distance { get; set; }
    public Rigidbody itemHit { get; set; }
    public Rigidbody itemHitID { get; set; }
}

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

    public float minForce;
    public float maxForce;

    void Update()
    {
        // Make sure the user pressed the mouse down
        if (!Input.GetMouseButtonDown(0))
            return;

        if (!transform.gameObject.GetComponent<BlockState>() || transform.gameObject.GetComponent<BlockState>().isBeingNudged || !transform.gameObject.GetComponent<BlockState>().userCanDrag) return;

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
            springJoint.minDistance = this.minForce;
            springJoint.maxDistance = this.maxForce;
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

            springJoint.autoConfigureConnectedAnchor = true;
        }

        springJoint.spring = spring;
        springJoint.damper = damper;
        springJoint.maxDistance = distance;
        springJoint.connectedBody = hit.rigidbody;

        hitCoords p = new hitCoords();
        p.distance = hit.distance;
        p.itemHit = hit.collider.gameObject.GetComponent<Rigidbody>();
        StartCoroutine("DragTheBox", p);
    }

    IEnumerator DragTheBox(hitCoords stuffToFollow)
    {
        float oldDrag = springJoint.connectedBody.drag;
        float oldAngularDrag = springJoint.connectedBody.angularDrag;
        springJoint.connectedBody.drag = drag;

        springJoint.connectedBody.angularDrag = angularDrag;
        mainCamera = FindCamera();

        while (Input.GetMouseButton(0) && stuffToFollow.itemHit.gameObject.GetComponent<BlockState>().userCanDrag)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            springJoint.transform.position = ray.GetPoint(stuffToFollow.distance);
            Vector3 attatchedItem = stuffToFollow.itemHit.transform.TransformPoint(springJoint.connectedAnchor);

            DrawLine.Draw(attatchedItem, ray.GetPoint(stuffToFollow.distance), Color.cyan);
            //Debug.Log(springJoint.spring.ToString());
            yield return stuffToFollow.distance;
        }

        if (springJoint.connectedBody)
        {
            springJoint.connectedBody.drag = oldDrag;
            springJoint.connectedBody.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        }

    }

    public static Camera FindCamera()
    { 
        return Camera.main;
    }
}