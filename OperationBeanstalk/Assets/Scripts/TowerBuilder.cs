using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder: MonoBehaviour {
    public static List<List<Block>> createTower(BlockSettings blockSettings, GameObject center, int nPallets, Transform towerTransform)
    {
        List<List<Block>> palletStack = new List<List<Block>>();

        float x;
        float y;
        float z;

        Quaternion spawnRotation;

        float height = nPallets * blockSettings.BlockHeight;
        center.transform.position = new Vector3(towerTransform.position.x, height / 2f, towerTransform.position.z);
        float i;

        //Set universal settings for all blocks so we don't need a repeat
        GameObject block = BlockBuilder.SetNonUniqueBlockSettings(blockSettings.BlockPrefab, blockSettings);
        for (i = 0; i < nPallets; i++)
        {
            y = (i == 0) ? 0.5f : (i * blockSettings.BlockHeight * 1.1f);
            // spawnRotation = (((i + 1) % 2) == 0) ? Quaternion.Euler(0, 90, 0) : Quaternion.identity;
            spawnRotation = getSpawnRotation(i);

            //nBLocks = 3f, spacing = 0.005f, randomnessIndex = this.blockRandomnessIndex
            List<Block> pallet = createPallet(blockSettings, block, new Vector3(0, y, 0), spawnRotation, towerTransform);
            palletStack.Add(pallet);
        }

        //blockIndex = i;

        GameObject.FindGameObjectWithTag("TowerTop").transform.position = new Vector3(towerTransform.transform.position.x, height + 1f);
        Camera.main.GetComponent<CameraControl>().maxHeight = height * 1.4f;

        GameObject.FindGameObjectWithTag("TowerArea").transform.position = new Vector3(0, height / 2);
        GameObject.FindGameObjectWithTag("TowerArea").transform.localScale = new Vector3(3f - 0.1f, height, 3f - 0.1f);
        return palletStack;
        
    }

    // Create a full row of blocks for one layer of a tower
    public static List<Block> createPallet(BlockSettings blockSettings, GameObject block, Vector3 pos, Quaternion rotation, Transform towerTransform)
    {
        List<Block> members = new List<Block>();
        float start = (blockSettings.NBlocks / 2f) - 0.5f; //Distance from midpoint of furthest block to center, in blocks
        float positionOffset;
        for (float i = -start; i <= start; i += 1f)
        {
            block = Instantiate(block, pos, rotation);
            positionOffset = (block.transform.localScale.z + blockSettings.BlockSpacing) * i; //Distance from midpoints, in units
            block.transform.Translate(new Vector3(positionOffset, 0f, 0f));
            block = BlockBuilder.randomizeBlockDimensions(block, blockSettings.RandomnessIndex);
            block.transform.parent = towerTransform;
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
