using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBlock : InteractiveGameObject
{
    public bool isValidDropPosition { get; private set; }

    public BoxCollider BlockPlacingZone { get; set; }

    public List<Block> PlacedBlocks { get; set; }

    public int numPlacedBlocksBeingTouched { get; set; }
    public bool isWithinDropZone { get; set; }

    public Vector3 TowerTop { get; set; }

    public void Start()
    {
        
    }

    public void Update()
    {
        //if (!isValidDropPosition)
        //{
        var currentDropBlockState = IsCurrentDropPositionValid();
        if (currentDropBlockState != isValidDropPosition)
        {
            SetDropBlockAppearance();
        }
        isValidDropPosition = IsCurrentDropPositionValid();

        //}
    }


    private void SetDropBlockAppearance()
    {

    }

    public void SetDropBlockPlacement(Vector3 actualBlockDropPosition, float blockHeight)
    {
        var dropPosition = new Vector3(actualBlockDropPosition.x, TowerTop.y + blockHeight, actualBlockDropPosition.z);
        gameObject.transform.position = dropPosition;
    }

    public bool IsCurrentDropPositionValid()
    {
        /**
         * -Check whether block is being placed in correct rotation
         * -If we are placing the 1st block:
         *      ->Check whether we are leaving room for 2 other blocks to be placed
         * -......................2nd block:
         *      ->..................................... 1 other block
         * 
         * */
        var isValidPlacement = (numPlacedBlocksBeingTouched == 0 && isWithinDropZone);
        return isValidPlacement;
    }

    /**
     * Gets the current area and bounds of the dropzone/s
     * 
     * */

    public bool IsWithinDropZone()
    {
        return false;
    }

    public void DropUntilColliderIsHit()
    {

    }

    void OnCollisionExit(Collision other)
    {
        Debug.Log(other);
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log(other);
    }
}
