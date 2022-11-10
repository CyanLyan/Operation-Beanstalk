using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockBuilder;

public class Tower : MonoBehaviour
{   
    public GameObject towerCollisionBoxObj;
    public TowerCollisionBox towerCollisionBox;
    public GameObject towerTop;
    public GameObject towerDropZone;
    public float TowerSetUpWaitTime = 0.5f;
    public List<GameObject> BlocksInTower= new List<GameObject>();
    public bool GenerateTower(TowerInitDetails initDetails, int nPallets)
    {
        try
        {
            initDetails.TowerCollisionBox = this.towerCollisionBoxObj;
            this.towerCollisionBox = this.towerCollisionBoxObj.GetComponent<TowerCollisionBox>();
            initDetails.towerArea = gameObject;
            initDetails.towerTop = this.towerTop;
            initDetails.nPallets= nPallets;
            initDetails.SetTowerCollisionBoxAndDropZone(towerCollisionBoxObj);
            this.towerDropZone.transform.position = initDetails.dropZonePosition;
            this.BlocksInTower = TowerBuilder.createTower(initDetails);
            this.towerCollisionBox.CalculateTowerBoundsAndSet(this.BlocksInTower, initDetails.nPallets);
            SetDropZoneDimensions(initDetails);

            StartCoroutine(Wait(20));
            return true;

        } catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public void ActivateTowerDropZone()
    {
        towerDropZone.GetComponent<TowerDropZone>().TowerDropZoneIsReady = true;
    }

    private IEnumerator Wait(int seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
    }

    private void SetDropZoneDimensions(TowerInitDetails initDetails) 
    {
        BoxCollider boxCollider = this.towerDropZone.GetComponent<BoxCollider>();
        var newWidth = initDetails.blockSettings.NBlocksPerPallet;
        //TODO-figure out more convinient calculation for y dim than magic number
        boxCollider.size = new Vector3(newWidth, 0.12f, newWidth);
    }

    public void UpdateDropZonePosition(float newYPosition)
    {
        BoxCollider box = this.towerDropZone.GetComponent<BoxCollider>();
        box.center = new Vector3(box.center.x, newYPosition, box.center.z);
    }
}