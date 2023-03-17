using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBlock : Block
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;   
    }
}
