using UnityEngine;
using System.Collections;
using System.Linq;

public class hitCoords: MonoBehaviour
{
    public float distance { get; set; }
    public Rigidbody itemHit { get; set; }
    public Rigidbody itemHitID { get; set; }
}

public class DragBox : MonoBehaviour
{
    public float spring = 50.0f;
    public float damper = 0.2f;
    public float drag = 10.0f;
    public float angularDrag = 5.0f;
    public float distance = 0.2f;
    public bool attachToCenterOfMass;
    public Camera mainCamera;

    private SpringJoint springJoint;

    public float minForce;
    public float maxForce;
    public GameObject lineContainer;
    void Update()
    {
        // Make sure the user pressed the mouse down
        if (!Input.GetMouseButtonDown(0))
            return;

        hitCoords p = gameObject.AddComponent<hitCoords>();
        RaycastHit hit;
        // We need to actually hit an object
        if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            return;
        }
        p.distance = hit.distance;

        if (!hit.collider.gameObject.GetComponent<Block>() || hit.collider.gameObject.GetComponent<Block>().isBeingNudged || !hit.collider.gameObject.GetComponent<Block>().userCanDrag || hit.collider.gameObject.GetComponent<Block>().blockIsBeingDragged) return;
        if(!hit.collider.gameObject.GetComponent<Block>().blockIsBeingDragged)
        {
            if (!hit.collider.gameObject.GetComponent<Block>().mouseMovedEnoughToDrag())
            {
                return;
            }
            else
            {
                hit.collider.gameObject.GetComponent<Block>().blockIsBeingDragged = true;
            }
        }
        p.itemHit = hit.collider.gameObject.GetComponent<Rigidbody>();
        mainCamera = FindCamera();

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

        StartCoroutine("DragTheBox", p);
    }

    IEnumerator DragTheBox(hitCoords stuffToFollow)
    {
        float oldDrag = springJoint.connectedBody.drag;
        float oldAngularDrag = springJoint.connectedBody.angularDrag;
        springJoint.connectedBody.drag = drag;

        springJoint.connectedBody.angularDrag = angularDrag;
        lineContainer = GameObject.FindGameObjectWithTag("LineRenderer");

        while (Input.GetMouseButton(0) && stuffToFollow.itemHit.gameObject.GetComponent<Block>().userCanDrag)
        {
        Vector3 attatchedItem = stuffToFollow.itemHit.transform.TransformPoint(springJoint.connectedAnchor);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            springJoint.transform.position = ray.GetPoint(stuffToFollow.distance);

            DrawLine.Draw(this.lineContainer, attatchedItem, ray.GetPoint(stuffToFollow.distance), Color.cyan, (Time.deltaTime * 1.5f));
            //Debug.Log(springJoint.spring.ToString());
            yield return stuffToFollow.distance;
        }

        if (springJoint != null && springJoint.connectedBody)
        {
            springJoint.connectedBody.drag = oldDrag;
            springJoint.connectedBody.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        }
        DrawLine.ResetLine(this.lineContainer);
        destroyAllRigidBodies();
        stuffToFollow.itemHit.gameObject.GetComponent<Block>().blockIsBeingDragged = false;

    }

    public static Camera FindCamera()
    { 
        return Camera.main;
    }

    public static void destroyAllRigidBodies()
    {
        var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Rigidbody dragger");
        if(objects != null)
        {
            foreach(var obj in objects) 
            {
                GameObject.Destroy(obj);
            }
        }
    }
}