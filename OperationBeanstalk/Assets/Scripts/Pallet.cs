using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pallet : MonoBehaviour
{
    public GameObject blockPrefab;
    public List<Block> members = new List<Block>();

    void Awake()
    {
        createPallet(); //Only in awake for debug
    }

    public void createPallet(float nBlocks = 3f, float blockSpacing = 0.3f)
    {
        float start = (nBlocks/2f)-0.5f; //Distance from midpoint of furthest block to center, in blocks
        GameObject block;
        float positionOffset;
        for(float i = -start; i <= start; i += 1f)
        {
            block = GameObject.Instantiate(blockPrefab, transform);
            positionOffset = (block.transform.localScale.z + blockSpacing) * i; //Distance from midpoints, in units
            block.transform.Translate(  new Vector3( 0f, 0f, positionOffset) );
            members.Add(block.GetComponent<Block>());
        }
    }
}
