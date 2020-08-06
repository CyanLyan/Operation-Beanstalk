using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool hasBlockBeenMoved {get; set;} = false;
    public bool isBlockTouchingGround {get; set;} = false;
    public static int nBlocksOnGround {get; set;} = 0;

    void Update()
    {
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag != "GroundPlane") return; //for now, drop this function for all collisions but that with the ground
        isBlockTouchingGround = true;
        nBlocksOnGround++;
    }

    void OnCollisionExit(Collision other)
    {
        if(other.gameObject.tag != "GroundPlane") return; //for now, drop this function for all collisions but that with the ground
        isBlockTouchingGround = false;
        if(nBlocksOnGround-- < 3);
    }
}
