using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    // We reference this block for dimensions of all jenga blocks in a tower
    Block defaultBlock = new Block(8, 2, 4, 0, 0 , 0);

    /**
       x 012
       y 
       0 ...
       1 ...
       2 ...

       default block positions [{(0,0) - (2,0)}, {(1,0) - (1,2)}, {(2,0) - (2,2)}]
       
       'sideways' block positions [{(0,0) - (0,2)}, {(0,1) - (2,1)}, {(0,2) - (2,2)}]
       UNSURE HOW TO SPAWN SIDEWAYS - CAN'T DO BRAIN STUFF, FACING SIDEWAYS CURRENTLY DOES NOTHING
       
    **/

    /**
        -Create row of 3 blocks - i.e. a pallet
        -start spawning it at x,y,z
        -pallet needs to be at least the width of 3 blocks, so we use defaultBlock's width (blockWidth)
        -we MIGHT also need space between blocks because of Unity's collision rules
        -Each row of blocks faces the opposite direction, so we'll assume that there's a default orientation (facingSideways=false),
        and then a manually set orientation every 2 rows by the calling function.
        -Maths will likely be off - pls check ;3
        **/
    public List<Block> CreatePallet(float x, float y, float z, Posfloat spaceBetweenBlocks = 0, bool facingSideways = false)
    {
        if (blockWidth != 0)
        {
            List<Block> blockArr = new List<Block>();
            float localXPos = 0;
            float localYPos = 0;
            float localZPos = 0;
            for (int i = 0; i < 3; i++)
            {
                localXPos = (defaultBlock.width * i) + spaceBetweenBlocks;
                Block b = new Block(defaultBlock.width, defaultBlock.height, defaultBlock.depth, localXPos + x, localYPos + y, localZPos + z);
                blockArr.Add(b);
            }
        }
        else
        {
            // Throw error, this should not happen!
            return;
        }
    }



    public List<List<Block>> createTower(float x = 0, float y = 0, float z = 0, int numberOfLayers = 18, float spaceBetweenPallets = 0)
    {
        // We'll change x, y, z eventually with input args
        float localX = x;
        float localY = y;
        float localZ = z;

        float palletHeight = defaultBlock.height;
        
        bool sidewaysPalette = false;
        for (int i = 0; i < 18; i++)
        {
            sidewaysPalette = ((i % 2) == 0);
            localY = localY + (palletHeight * i) + spaceBetweenPallets;
            List<Block> pallet = CreatePallet(x, y, z, 0, sidewaysPalette);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
