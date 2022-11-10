using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCollisionBox : MonoBehaviour
{
    public float CurrentTowerHeightInBlocks;
    // Update is called once per frame

    public void UpdateTowerBoxBounds(float currentTowerHeightInBlocks)
    {
        this.CurrentTowerHeightInBlocks = currentTowerHeightInBlocks;
        VerticallyAdjustTowerBounds();
    }

    private Bounds CalculateTowerBoundsDimensions(List<GameObject> blocksInTower)
    {

        Bounds bounds = gameObject.GetComponent<BoxCollider>().bounds;
        foreach (GameObject block in blocksInTower)
        {
            bounds.Encapsulate(block.gameObject.GetComponent<MeshCollider>().bounds);
        }
        return bounds;
    }

    public void CalculateTowerBoundsAndSet(List<GameObject> blocksInTower, int nPallets)
    {
        this.CurrentTowerHeightInBlocks = nPallets;
        Bounds newBounds = CalculateTowerBoundsDimensions(blocksInTower);
        gameObject.GetComponent<BoxCollider>().size = newBounds.size;
        VerticallyAdjustTowerBounds();
    }

    private void VerticallyAdjustTowerBounds()
    {
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        //Adjust vertical bounding box as it's too big
        float newYBoundValue = boxCollider.size.y - 1f;
        boxCollider.size = new Vector3(boxCollider.size.x, newYBoundValue, boxCollider.size.z);
    }

    public void OnTriggerExit(Collider other)
    {
        if(other is MeshCollider)
        {
            if (other.gameObject.tag == "Block") {
                other.gameObject.GetComponent<Block>().HandleBlockTouchingNothing();
            }
        }
    }
}
