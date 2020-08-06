using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pallet : MonoBehaviour
{
    public GameObject blockPrefab;
    public List<Block> members = new List<Block>();

    void Awake()
    {
        createPallet();
    }

    public void createPallet(float nBlocks = 3f, float blockSpacing = 0.3f)
    {
        float start = (nBlocks/2f)-0.5f;
        print(start);
        GameObject block;
        float positionOffset;
        for(float i = -start; i <= start; i += 1f)
        {
            block = GameObject.Instantiate(blockPrefab, transform);
            positionOffset = (block.transform.localScale.z + blockSpacing) * i;
            print(i);
            block.transform.Translate(  new Vector3( 0f, 0f, positionOffset) );
            members.Add(block.GetComponent<Block>());
        }
    }
}
