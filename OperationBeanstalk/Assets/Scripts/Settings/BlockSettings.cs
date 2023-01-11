using UnityEngine;

public class BlockSettings : MonoBehaviour
{
	public float _mass;
	public float _drag;
    public float _blockHeight;
    public float _angularDrag;
	public float _blockSpacing;
	public float _nBlocks;
	public float _randomnessIndex;
    
    public float DistanceNeededForMouseDrag;
    public float TimeOnMouseDownNeededForDrag;

    public GameObject _blockPrefab;
    public GameObject BlockMoverObj;
    public BlockMover BlockMover;

    public BlockSettings(GameObject blockPrefab,
                         GameObject blockMoverObj,
                         float mass,
                         float drag,
                         float blockHeight,
                         float angularDrag,
                         float blockSpacing,
                         float nBlocks,
                         float randomnessIndex)
    {
        Mass = mass;
        Drag = drag;
        BlockHeight = blockHeight;
        AngularDrag = angularDrag;
        BlockSpacing = blockSpacing;
        NBlocksPerPallet = nBlocks;
        RandomnessIndex = randomnessIndex;
        BlockPrefab = blockPrefab;
        BlockMoverObj = blockMoverObj;
    }


    public float Mass {  get { return _mass; }
        set
        {
            _mass = value;
        } }
	public float Drag { 
		get { return _drag; } 
		set { _drag = value; } }

    public float BlockHeight
    {
        get { return _blockHeight; }
        set { _blockHeight = value; }
    }
    public float AngularDrag { 
		get { return _angularDrag; } 
		set { _angularDrag = value; } }
	public float BlockSpacing { 
		get { return _blockSpacing; } 
		set { _blockSpacing = value; } }
	public float NBlocksPerPallet { 
		get { return _nBlocks; }
        set
        {
            _nBlocks = value;
        } }
	public float RandomnessIndex { 
		get { return _randomnessIndex; } 
		set { _randomnessIndex = value; } }

	public GameObject BlockPrefab
    {
		get { return _blockPrefab; }
		set { _blockPrefab = value; }
    }

    //public float DistanceNeededForMouseNudge { get; set; }
}