using UnityEngine;

public class block_text_debug : MonoBehaviour
{
    string myInfo = "5";
    public bool isBlockTouchingGround { get; set; }
    public bool blocksTouching { get; set; }

    public bool isBeingNudged;

    public bool blockIsInTowerZone = true;

    public bool userCanDrag = true;

    public bool isBeingPlacedOnTop;

    public bool blockIsBeingDragged;
    // Start is called before the first frame update
    void Awake()
    {
        isBlockTouchingGround = GetComponentInParent<Block>().isBlockTouchingGround;
        blocksTouching = GetComponentInParent<Block>().blocksTouching;
        isBeingNudged = GetComponentInParent<Block>().isBeingNudged;
        blockIsInTowerZone = GetComponentInParent<Block>().blockIsInTowerZone;
        userCanDrag = GetComponentInParent<Block>().userCanDrag;
        isBeingPlacedOnTop = GetComponentInParent<Block>().isBeingPlacedOnTop;
        blockIsBeingDragged = GetComponentInParent<Block>().isBeingDragged;
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponentInParent<Block>().hasBlockBeenMovedByPlayerRecently)
        {
            blockIsBeingDragged = GetComponentInParent<Block>().isBeingDragged;
            isBlockTouchingGround = GetComponentInParent<Block>().isBlockTouchingGround;
            blocksTouching = GetComponentInParent<Block>().blocksTouching;
            isBeingNudged = GetComponentInParent<Block>().isBeingNudged;
            blockIsInTowerZone = GetComponentInParent<Block>().blockIsInTowerZone;
            userCanDrag = GetComponentInParent<Block>().userCanDrag;
            isBeingPlacedOnTop = GetComponentInParent<Block>().isBeingPlacedOnTop;

            string debugText = (isBlockTouchingGround) ? "Touching Ground\n" : "";
            debugText += (blocksTouching) ? "" : " Not Touching Blocks\n";
            debugText += (isBeingNudged) ? " Nudging\n" : "";
            debugText += (blockIsInTowerZone) ? "" : " Outside Tower\n";
            debugText += (isBeingNudged) ? " Placing On Top\n" : "";
            transform.GetComponent<TextMesh>().text = "" + debugText;
        } else
        {
            transform.GetComponent<TextMesh>().text = "";
        }
    }
}
