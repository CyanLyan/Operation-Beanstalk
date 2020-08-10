using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int nPallets;
    List<Pallet> palletStack = new List<Pallet>();
    public GameObject blockPrefab;

    private void Awake()
    {
        createTower();
    }

    public void createTower()
    {
        float x;
        float y;
        float z;

        for (float i = 0; i < nPallets; i++)
        {
            Pallet pallet = new Pallet(blockPrefab, 0, 0, 0);
            palletStack.Add(pallet);
        }
    }
    //Function to instantiate pallets vertically, to be written
}
