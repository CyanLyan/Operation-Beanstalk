using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockBuilder;
using static TowerInitDetails;

public class TowerController : MonoBehaviour
{   
    public GameObject towerCollisionBoxObj;
    public TowerCollisionBox towerCollisionBox;
    public GameObject towerTop;
    public GameObject towerDropZone;
    public GameObject blockPlacingZone;
    public float TowerSetUpWaitTime = 0.5f;
    public List<GameObject> BlocksInTower = new List<GameObject>();
    public float numBlocksCollapsed { get; set; }

    public static event Action OnTowerReady;
    
    public void GenerateTower(TowerInitDetails initDetails, int nPallets)
    {
        //try
        //{
            //Set initdetails to match current tower
            initDetails.TowerCollisionBox = towerCollisionBoxObj;
            initDetails.towerArea = gameObject;
            initDetails.towerTop = towerTop;
            initDetails.nPallets= nPallets;
            initDetails.TowerDropZone= towerDropZone;
            towerCollisionBox = towerCollisionBoxObj.GetComponent<TowerCollisionBox>();
            initDetails.blockSettings.BlockMover.towerDropZone = initDetails.TowerDropZone;
            initDetails.blockSettings.BlockMover.cam = initDetails.cam;
            initDetails.SetTowerCollisionBoxAndDropZone(towerCollisionBoxObj);
            blockPlacingZone.transform.position = initDetails.dropZonePosition;
            BlocksInTower = TowerBuilder.createTower(initDetails);
            towerCollisionBox.CalculateTowerBoundsAndSet(BlocksInTower, initDetails.nPallets);

            var towerSize = towerCollisionBox.GetComponent<BoxCollider>().size;
            towerTop.transform.position = new Vector3(towerTop.transform.position.x, towerSize.y, towerTop.transform.position.z); 
            initDetails.MidwayBlockMovePoint.transform.localPosition = new Vector3(0, Camera.main.GetComponent<CameraController>().maxHeight / 3, 0);
            //towerDropZone.transform.position = new Vector3(0, (Camera.main.GetComponent<CameraController>().maxHeight - 1), 0);
            
            SetBlockPlacingZoneDimensions(initDetails);
            StartCoroutine(WaitForTowerToSettle());
            //ActivateBlockPlacingZone();
            //return true;

        //} catch (Exception e)
        //{
        //    Debug.LogError(e.Message);
        //    return false;
        //}
    }

    private IEnumerator WaitForTowerToSettle()
    {
        yield return new WaitForSeconds(1.5f);

        Debug.Log("Tower settled");
        ActivateBlockPlacingZone();
        OnTowerReady?.Invoke();
    }

    public void ActivateBlockPlacingZone()
    {
        blockPlacingZone.GetComponent<BlockPlacingZone>().BlockPlacingZoneIsReady = true;
        blockPlacingZone.GetComponent<BoxCollider>().enabled = true;
    }

    private void SetBlockPlacingZoneDimensions(TowerInitDetails initDetails) 
    {
        BoxCollider boxCollider = blockPlacingZone.GetComponent<BoxCollider>();
        var newWidth = initDetails.blockSettings.NBlocksPerPallet;
        //TODO-figure out more convinient calculation for y dim than magic number
        boxCollider.size = new Vector3(newWidth, 0.12f, newWidth);
    }

    public void UpdateBlockPlacingZonePosition(float newYPosition)
    {
        BoxCollider box = blockPlacingZone.GetComponent<BoxCollider>();
        box.transform.position = new Vector3(box.transform.position.x, newYPosition, box.transform.position.z);
    }

    public bool TowerIsCollapsing()
    {
        numBlocksCollapsed++;
        return (numBlocksCollapsed > 2);
    }
}