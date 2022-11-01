using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Pallet : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject tower;
    private float RandomnessIndex { get; set; }
    public List<Block> members = new List<Block>();
    public Vector3 pos;

    public Pallet(GameObject block, GameObject tower, Quaternion spawnRotation, float x = 0f, float y = 0f, float z = 0f, float nBlocks = 3f, float blockSpacing = 0.3f, float randomnessIndex = 0)
    {
        this.pos = new Vector3(x, y, z);
        this.blockPrefab = block;
        this.tower = tower;
        this.RandomnessIndex = randomnessIndex;
        createPallet(blockPrefab, spawnRotation, nBlocks, blockSpacing);
    }

    public void createPallet(GameObject blockPrefab, Quaternion rotation, float nBlocks = 3f, float blockSpacing = 0.3f)
    {
        float start = (nBlocks/2f)-0.5f; //Distance from midpoint of furthest block to center, in blocks
        GameObject block;
        float positionOffset;
        for(float i = -start; i <= start; i += 1f)
        {
            
            block = Instantiate(blockPrefab, pos, rotation);
            positionOffset = (block.transform.localScale.z + blockSpacing) * i; //Distance from midpoints, in units
            block.transform.Translate(  new Vector3(positionOffset, 0f, 0f));
            block = randomizeBlockDimensions(block); 
            block.transform.parent = tower.transform;
            members.Add(block.GetComponent<Block>());
        }
    }
    public GameObject randomizeBlockDimensions(GameObject block)
    {
        var randomWeightModifierMaxVariation = this.RandomnessIndex / 10;
        var randomScaleModifierMaxVariation = this.RandomnessIndex / 100;
        float randomWeightModifier = Random.Range(0, randomWeightModifierMaxVariation);
        float randomScaleModifier = Random.Range(0, randomScaleModifierMaxVariation);


        block.GetComponent<Rigidbody>().mass = GenerateRandomDeviation(block.GetComponent<Rigidbody>().mass, randomWeightModifier);
        Vector3 randomScaleDimensions = new Vector3(GenerateRandomDeviation(block.transform.localScale.x, randomScaleModifier),
            GenerateRandomDeviation(block.transform.localScale.y, randomScaleModifier), GenerateRandomDeviation(block.transform.localScale.z, randomScaleModifier));
        block.transform.localScale = randomScaleDimensions;

        return block;
    }

    private float GenerateRandomDeviation(float dimensionToDeviate, float randomWeightModifierMaxVariation)
    {
        float randomModifier = Random.Range(0, randomWeightModifierMaxVariation);
        
    //Do coin flip
        if (Mathf.Round(Random.Range(0, 2)) == 1)
        {
            dimensionToDeviate = dimensionToDeviate + randomModifier;
        }
        else
        {
            dimensionToDeviate = dimensionToDeviate - randomModifier; 
        }
        return dimensionToDeviate;
    }
}
