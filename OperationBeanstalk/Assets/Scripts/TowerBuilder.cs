using System;
using System.Collections.Generic;
using UnityEngine;
using static BlockBuilder;

public class TowerBuilder: MonoBehaviour {
    public static List<List<Block>> createTower(TowerInitDetails initDetails)
    {
        List<List<Block>> palletStack = new List<List<Block>>();

        float x;
        float y;
        float z;

        Quaternion spawnRotation;
        Vector3 spawnPosition;
        float height = initDetails.nPallets * initDetails.blockSettings.BlockHeight;
        initDetails.towerCenter.transform.position = new Vector3(initDetails.towerCenter.transform.position.x, height / 2f, initDetails.towerCenter.transform.position.z);
        float i;

        //Set universal settings for all blocks so we don't need a repeat
        GameObject block = SetNonUniqueBlockSettings(initDetails.blockSettings.BlockPrefab, initDetails.blockSettings);
        for (i = 0; i < initDetails.nPallets; i++)
        {
            y = (i == 0) ? 0.5f : (i * initDetails.blockSettings.BlockHeight * 1.1f);
            spawnRotation = getSpawnRotation(i);
            spawnPosition = new Vector3(0, y, 0);

            List<Block> pallet = createPallet(initDetails, block, spawnPosition, spawnRotation);
            palletStack.Add(pallet);
        }

        initDetails.towerTop.transform.position = new Vector3(initDetails.towerCenter.transform.position.x, height + 1f);
        Camera.main.GetComponent<CameraControl>().maxHeight = height * 1.4f;

        initDetails.towerArea.transform.position = new Vector3(0, height / 2);
        initDetails.towerArea.transform.localScale = new Vector3(3f - 0.1f, height, 3f - 0.1f);
        return palletStack;
        
    }

    // Create a full row of blocks for one layer of a tower (like a loading pallet)
    public static List<Block> createPallet(TowerInitDetails initDetails, GameObject block, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        List<Block> members = new List<Block>();
        float start = (initDetails.blockSettings.NBlocks / 2f) - 0.5f; //Distance from midpoint of furthest block to center, in blocks
        for (float i = -start; i <= start; i += 1f)
        {
            block = Instantiate(block, spawnPosition, spawnRotation);
            InitializeBlockParamsAndSetTransform(initDetails, block, i);
            members.Add(block.GetComponent<Block>());
        }
        return members;
    }

    //Function to instantiate pallets vertically, to be written

    private static Quaternion getSpawnRotation(float i)
    {
        var spawnRotation = (((i + 1) % 2) == 0) ? Quaternion.Euler(90, 90, 90) : Quaternion.Euler(90, 0, 90);
        return spawnRotation;
    }

    private static Vector3 getSpawnPosition(float i, Block block)
    {
        float positionOffset = (block.transform.localScale.z + 0.005f) * i; //Distance from midpoints, in units
        return new Vector3(positionOffset, 0f, 0f);
    }
}
