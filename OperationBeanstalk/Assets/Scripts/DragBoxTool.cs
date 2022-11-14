using UnityEngine;
using System.Collections;
using System.Linq;

public class ItemHitDetails: MonoBehaviour
{
    public float distance { get; set; }
    public Rigidbody itemHitRigidBody { get; set; }
    public Block block { get; set; }
}

public class DragBoxTool : MonoBehaviour
{
    public float spring = 50.0f;
    public float damper = 0.2f;
    public float drag = 0f;
    public float angularDrag = 5.0f;
    public float distance = 0.2f;
    public bool attachToCenterOfMass;
    public Camera mainCamera;

    private SpringJoint springJoint;

    public float minForce;
    public float maxForce;
    public GameObject lineContainer;

    public bool dragBlockByPoint = false;

    public bool isDragging;
    void Update()
    {
        // Make sure the user pressed the mouse down
        if (!Input.GetMouseButton(0))
            return;

        ItemHitDetails itemHit = gameObject.AddComponent<ItemHitDetails>();
        RaycastHit hit;
        // We need to actually hit an object
        if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            return;
        }
        itemHit.distance = hit.distance;

        var hitBlock = hit.collider.gameObject.GetComponent<Block>();

        if (!hitBlock || hitBlock.hasBeenPlaced || hitBlock.isBeingNudged || !hitBlock.userCanDrag || hitBlock.isBeingDragged) return;
        itemHit.itemHitRigidBody = hit.collider.gameObject.GetComponent<Rigidbody>();
        itemHit.block = hitBlock;
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

        hitBlock.isBeingDragged = true;
        isDragging = true;
        StartCoroutine("DragTheBox", itemHit);
    }

    IEnumerator DragTheBox(ItemHitDetails stuffToFollow)
    {
        stuffToFollow.itemHitRigidBody.gameObject.GetComponent<Block>().hasBlockBeenMovedByPlayerRecently = true;

        stuffToFollow.itemHitRigidBody.GetComponent<Rigidbody>().freezeRotation = !dragBlockByPoint;
        
        springJoint.connectedBody.drag = drag;

        springJoint.connectedBody.angularDrag = angularDrag;
        lineContainer = GameObject.FindGameObjectWithTag("LineRenderer");

        while (Input.GetMouseButton(0) && stuffToFollow.itemHitRigidBody.gameObject.GetComponent<Block>().userCanDrag)
        {
            Vector3 attatchedItem = stuffToFollow.itemHitRigidBody.transform.TransformPoint(springJoint.connectedAnchor);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            springJoint.transform.position = ray.GetPoint(stuffToFollow.distance);

            DrawLine.Draw(this.lineContainer, attatchedItem, ray.GetPoint(stuffToFollow.distance), Color.cyan, (Time.deltaTime * 1.5f));
            yield return stuffToFollow.distance;
        }

        if (springJoint != null && springJoint.connectedBody)
        {
            springJoint.connectedBody.drag = 1f;
            springJoint.connectedBody.angularDrag = 0.05f;
            springJoint.connectedBody = null;
        }

        DrawLine.ResetLine(this.lineContainer);
        destroyAllRigidBodies();
        stuffToFollow.itemHitRigidBody.gameObject.GetComponent<Block>().isBeingDragged = false;
        stuffToFollow.itemHitRigidBody.gameObject.GetComponent<Block>().isActive = false;
        isDragging = false;
        if (dragBlockByPoint) stuffToFollow.itemHitRigidBody.freezeRotation = false;

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