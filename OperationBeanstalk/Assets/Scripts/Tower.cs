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
        //float blockHeight = blockPrefab.transform.lossyScale.y;

        float height = nPallets * blockHeight;
        center.transform.position = new Vector3(gameObject.transform.position.x, height / 2f, gameObject.transform.position.z);

        for (float i = 0; i < nPallets; i++)
        {
            y = (i == 0) ? 0.5f : (i * blockHeight * 1.1f);
            // spawnRotation = (((i + 1) % 2) == 0) ? Quaternion.Euler(0, 90, 0) : Quaternion.identity;
            spawnRotation = (((i + 1) % 2) == 0) ? Quaternion.Euler(90, 90, 90) : Quaternion.Euler(90, 0, 90);
            Pallet pallet = new Pallet(blockPrefab, center, spawnRotation, 0, y, 0, 3f, 0.05f);
            palletStack.Add(pallet);
        }

        GameObject.FindGameObjectWithTag("TowerTop").transform.position = new Vector3(gameObject.transform.position.x, height);
        Camera.main.GetComponent<CameraControl>().maxHeight = height * 1.4f;

        GameObject.FindGameObjectWithTag("TowerArea").transform.position = new Vector3(0, height/2);
        GameObject.FindGameObjectWithTag("TowerArea").transform.localScale = new Vector3(3f - 0.1f, height, 3f - 0.1f);
    }
    //Function to instantiate pallets vertically, to be written
}