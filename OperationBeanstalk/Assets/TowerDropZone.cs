using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDropZone : MonoBehaviour
{
    public bool TowerDropZoneIsReady = false;

    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        if (TowerDropZoneIsReady && (other is MeshCollider) && (other.gameObject.tag == "Block") && (other.gameObject.GetComponent<Block>().isInDropPosition == false))
        {
                other.gameObject.GetComponent<Block>().FinishDroppingBlockInPlace();
        }
    }
}
