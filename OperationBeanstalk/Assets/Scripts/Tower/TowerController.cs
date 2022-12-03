using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockBuilder;

public class TowerController : MonoBehaviour
{   
    public GameObject towerCollisionBoxObj;
    public TowerCollisionBox towerCollisionBox;
    public GameObject towerTop;
    public GameObject towerDropZone;
    public GameObject blockPlacingZone;
    public float TowerSetUpWaitTime = 0.5f;
    public List<GameObject> BlocksInTower= new List<GameObject>();
    public bool GenerateTower(TowerInitDetails initDetails, int nPallets)
    {
        try
        {
            //Set initdetails to match current tower
            initDetails.TowerCollisionBox = this.towerCollisionBoxObj;
            initDetails.towerArea = gameObject;
            initDetails.towerTop = this.towerTop;
            initDetails.nPallets= nPallets;
            initDetails.TowerDropZone= this.towerDropZone;
            this.towerCollisionBox = this.towerCollisionBoxObj.GetComponent<TowerCollisionBox>();
            initDetails.blockSettings.BlockMover.towerDropZone = initDetails.TowerDropZone;
            initDetails.blockSettings.BlockMover.cam = initDetails.cam;
            initDetails.SetTowerCollisionBoxAndDropZone(towerCollisionBoxObj);
            this.blockPlacingZone.transform.position = initDetails.dropZonePosition;
            this.BlocksInTower = TowerBuilder.createTower(initDetails);
            this.towerCollisionBox.CalculateTowerBoundsAndSet(this.BlocksInTower, initDetails.nPallets);

            var towerSize = this.towerCollisionBox.GetComponent<BoxCollider>().size;
            initDetails.MidwayBlockMovePoint.transform.localPosition = new Vector3(0, Camera.main.GetComponent<CameraController>().maxHeight / 3, 0);
            this.towerDropZone.transform.position = new Vector3(0, Camera.main.GetComponent<CameraController>().maxHeight - 1, 0);
            
            SetBlockPlacingZoneDimensions(initDetails);

            this.ActivateBlockPlacingZone();
            return true;

        } catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public void ActivateBlockPlacingZone()
    {
        blockPlacingZone.GetComponent<BlockPlacingZone>().BlockPlacingZoneIsReady = true;
        blockPlacingZone.GetComponent<BoxCollider>().enabled = true;
    }

    private void SetBlockPlacingZoneDimensions(TowerInitDetails initDetails) 
    {
        BoxCollider boxCollider = this.blockPlacingZone.GetComponent<BoxCollider>();
        var newWidth = initDetails.blockSettings.NBlocksPerPallet;
        //TODO-figure out more convinient calculation for y dim than magic number
        boxCollider.size = new Vector3(newWidth, 0.12f, newWidth);
    }

    public void UpdateBlockPlacingZonePosition(float newYPosition)
    {
        BoxCollider box = this.blockPlacingZone.GetComponent<BoxCollider>();
        box.transform.position = new Vector3(box.transform.position.x, newYPosition, box.transform.position.z);
    }

    public void TowerIsCollapsing()
    {
        Debug.Log("AAA TOWER IS COLLAPSING");
    }
}