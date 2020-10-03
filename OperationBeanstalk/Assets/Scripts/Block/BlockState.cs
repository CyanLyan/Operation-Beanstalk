using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockState : MonoBehaviour
{
    public bool hasBlockBeenMoved { get; set; } = false;
    public bool isBlockTouchingGround { get; set; } = false;

    public bool blocksTouching { get; set; } = false;

    public bool isBeingNudged = false;

    public bool blockIsInTowerZone { get; set; } = false;

    public bool rotating = false;

    public bool userCanDrag = true;
    public bool isBeingPlacedOnTop = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
