using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockBuilder;

public class Tower : MonoBehaviour
{   
    public GameObject center;
    public GameObject towerTop;

    public float TowerSetUpWaitTime = 0.5f;


    private void Awake()
    {        
    }

    public bool GenerateTower(TowerInitDetails initDetails)
    {
        try
        {
            initDetails.towerCenter = this.center;
            initDetails.towerArea = gameObject;
            initDetails.towerTop = this.towerTop;
            initDetails.nPallets = 15;
            TowerBuilder.createTower(initDetails);
            return true;

        } catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    
}