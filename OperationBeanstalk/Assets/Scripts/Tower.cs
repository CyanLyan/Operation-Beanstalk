using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{   
    public GameObject center;

    public float TowerSetUpWaitTime = 0.5f;


    private void Awake()
    {        
    }

    public bool GenerateTower(BlockSettings blockSettings, int nPallets)
    {
        TowerBuilder.createTower(blockSettings, this.center, nPallets, center.transform);
        return true;
    }

    
}