using UnityEngine;

public class BlockPlacingZone : MonoBehaviour
{
    public bool BlockPlacingZoneIsReady;

    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("BlockPlacingZone Collided with " + other.gameObject.name);
        if (BlockPlacingZoneIsReady && (other is MeshCollider) && (other.gameObject.tag == "Block") && (other.gameObject.GetComponent<Block>().isInDropPosition == true))
        {
            var block = other.gameObject.GetComponent<Block>();
            block.blockMover.FinishDroppingBlockInPlace(block);
        }
    }
}
