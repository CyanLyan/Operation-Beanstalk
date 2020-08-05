using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    /**
           +--------+
          /        /|
         /        / |
        +--------+  |  
        |        |  |
        |        |  +
 height |        | /
        |        |/ depth
        +--------+
           width
    **/

// Please change coord stuff as needed to make it usable
    public float width { get; set; }
    public float height { get; set; }
    public float depth { get; set; }


    

    public float xPos { get; set; }
    public float yPos {get; set; }
    public float zPos {get; set; }

    public bool hasBlockBeenMoved {get; set;} = false;
    public bool isBlockTouchingGround {get; set;} = false;


    public Block(float w, float h, float d, float x, float y, float z) {
        width = w;
        height = h;
        depth = d;

        xPos = x;
        yPos = y;
        zPos = z;
    }

    // Either use some kind of global variable for ground and see if block is touching it (coordinates overlap),
    // or get args for ground from calling parent and just see if collision has occured with those
    public bool checkIfBlockIsTouchingGround(/*ground coord args?*/) {
        if(hasBlockBeenMoved) {
            // Check for collision
        }
    }
        
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
