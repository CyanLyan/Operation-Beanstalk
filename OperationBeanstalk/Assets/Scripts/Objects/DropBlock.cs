using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBlock : Object
{
    public bool isValidDropPosition { get; private set; }

    public void Update()
    {
        if(!isValidDropPosition)
        {

        }
    }

    public bool IsCurrentDropPositionValid()
    {
        /**
         * -Check whether block is being placed in correct rotation
         * -If we are placing the 1st block:
         *      ->Check whether we are leaving room for 2 other blocks to be placed
         * -......................2nd block:
         *      ->..................................... 1 other block
         * 
         * */



        //
        //

        return false;
    }
}
