using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int nPallets;
    List<Pallet> palletStack = new List<Pallet>();
    public GameObject blockPrefab;
    public GameObject center;
    public float blockHeight;

    public float blockIndex = 0;

    public float TowerSetUpWaitTime = 0.5f;
    public bool TowerIsReady = false;

    public float blockRandomnessIndex = 0f;

    private void Awake()
    {
        createTower();
        TowerIsReady = true;
    }

    public void createTower()
    {
        float x;
        float y;
        float z;

        Quaternion spawnRotation;

        float height = nPallets * blockHeight;
        center.transform.position = new Vector3(gameObject.transform.position.x, height / 2f, gameObject.transform.position.z);
        float i;
        for (i = 0; i < nPallets; i++)
        {
            y = (i == 0) ? 0.5f : (i * blockHeight * 1.1f);
            // spawnRotation = (((i + 1) % 2) == 0) ? Quaternion.Euler(0, 90, 0) : Quaternion.identity;
            spawnRotation = this.getSpawnRotation(i);
            Pallet pallet = new Pallet(blockPrefab, center, spawnRotation, 0, y, 0, 3f, 0.005f, this.blockRandomnessIndex);
            palletStack.Add(pallet);
        }

        blockIndex = i;

        GameObject.FindGameObjectWithTag("TowerTop").transform.position = new Vector3(gameObject.transform.position.x, height+ 1f);
        Camera.main.GetComponent<CameraControl>().maxHeight = height * 1.4f;

        GameObject.FindGameObjectWithTag("TowerArea").transform.position = new Vector3(0, height/2);
        GameObject.FindGameObjectWithTag("TowerArea").transform.localScale = new Vector3(3f - 0.1f, height, 3f - 0.1f);
    }
    //Function to instantiate pallets vertically, to be written

    public Quaternion getSpawnRotation(float i)
    {
        var spawnRotation = (((i + 1) % 2) == 0) ? Quaternion.Euler(90, 90, 90) : Quaternion.Euler(90, 0, 90);
        return spawnRotation;
    }

    public Vector3 getSpawnPosition(float i, Block block)
    {
        float positionOffset = (block.transform.localScale.z + 0.005f) * i; //Distance from midpoints, in units
        return new Vector3(positionOffset, 0f, 0f);
    }
}