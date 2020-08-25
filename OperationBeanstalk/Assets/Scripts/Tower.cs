using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int nPallets;
    List<Pallet> palletStack = new List<Pallet>();
    public GameObject blockPrefab;
    public GameObject center;

    private void Awake()
    {
        createTower();
    }

    public void createTower()
    {
        float x;
        float y;
        float z;

        Quaternion spawnRotation;
        //Vector3 objectSize = Vector3.Scale(transform.localScale, GetComponent())
        float blockHeight = blockPrefab.transform.localScale.y;

        for (float i = 0; i < nPallets; i++)
        {
            y = (i == 0) ? 0.5f : (i * (blockHeight)) + 0.2f;
            // spawnRotation = (((i + 1) % 2) == 0) ? Quaternion.Euler(0, 90, 0) : Quaternion.identity;
            spawnRotation = (((i + 1) % 2) == 0) ? Quaternion.Euler(90, 90, 90) : Quaternion.Euler(90, 0, 90);
            Pallet pallet = new Pallet(blockPrefab, center, spawnRotation , 0, y, 0, 3f, 0.05f);
            palletStack.Add(pallet);
        }
    }
    //Function to instantiate pallets vertically, to be written
}
