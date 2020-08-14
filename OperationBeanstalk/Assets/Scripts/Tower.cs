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

        //Vector3 objectSize = Vector3.Scale(transform.localScale, GetComponent())
        float blockHeight = blockPrefab.transform.localScale.y + 0.1f;

        for (float i = 0; i < nPallets; i++)
        {
            y = (i == 0) ? 0.8f : (i * (blockHeight) + 0.2f );
            Pallet pallet = new Pallet(blockPrefab, 0, y, 0);
            palletStack.Add(pallet);
        }
    }
    //Function to instantiate pallets vertically, to be written
}
