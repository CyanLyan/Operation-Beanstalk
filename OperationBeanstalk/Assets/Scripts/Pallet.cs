using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pallet : MonoBehaviour
{
    public GameObject blockPrefab;
    public List<Block> members = new List<Block>();
    public float x;
    public float y;
    public float z;

    public Pallet(GameObject block, float xPos = 0, float yPos = 0, float zPos = 0, float nBlocks = 3f, float blockSpacing = 0.3f)
    {
        this.x = xPos;
        this.y = yPos;
        this.z = zPos;
        createPallet(block);
    }

    void Awake()
    {
        //0createPallet(block); //Only in awake for debug
    }

    public void createPallet(GameObject block, float nBlocks = 3f, float blockSpacing = 0.3f)
    {
        float start = (nBlocks/2f)-0.5f; //Distance from midpoint of furthest block to center, in blocks
        //GameObject block;
        float positionOffset;
        for(float i = -start; i <= start; i += 1f)
        {
            block = Instantiate(block, new Vector3(x,y,z), Quaternion.identity);
            positionOffset = (block.transform.localScale.z + blockSpacing) * i; //Distance from midpoints, in units
            block.transform.Translate(  new Vector3( 0f, 0f, positionOffset) );
            members.Add(block.GetComponent<Block>());
        }
    }
}
