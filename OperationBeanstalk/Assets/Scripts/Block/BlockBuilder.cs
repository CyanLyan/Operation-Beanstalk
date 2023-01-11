using UnityEngine;

public static class BlockBuilder
{
    public static GameObject randomizeBlockDimensions(GameObject block, float RandomnessIndex)
    {
        var randomWeightModifierMaxVariation = RandomnessIndex / 10;
        var randomScaleModifierMaxVariation = RandomnessIndex / 1000;
        float randomWeightModifier = Random.Range(0, randomWeightModifierMaxVariation);
        float randomScaleModifier = Random.Range(0, randomScaleModifierMaxVariation);


        block.GetComponent<Rigidbody>().mass = GenerateRandomDeviation(block.GetComponent<Rigidbody>().mass, randomWeightModifier);
        

        return block;
    }

    //When we randomize some transform part of the block, deviations should always result in blocks being smaller & looser
    private static float GenerateRandomDeviation(float dimensionToDeviate, float randomWeightModifierMaxVariation)
    {
        float randomModifier = Random.Range(0, randomWeightModifierMaxVariation);

        //Do coin flip
        if (Mathf.Round(Random.Range(0, 2)) == 1)
        {
            dimensionToDeviate = dimensionToDeviate - randomModifier;
        }
        return dimensionToDeviate;
    }


    //TODO - look into randomizing mass and other stuff
    private static GameObject SetRigidBodyDimensions(GameObject block, BlockSettings blockSettings)
    {
        var rigidBody = block.GetComponent<Rigidbody>();
        rigidBody.mass = blockSettings.Mass;
        rigidBody.drag = blockSettings.Drag;
        rigidBody.angularDrag = blockSettings.AngularDrag;
        rigidBody.useGravity = true;
        return block;
    }

    private static GameObject SetRandomScaleDimensions(GameObject block, BlockSettings blockSettings, float randomScaleModifier) {
        var localScale = block.transform.localScale;
        Vector3 randomScaleDimensions = new Vector3(GenerateRandomDeviation(localScale.x, randomScaleModifier), GenerateRandomDeviation(localScale.y, randomScaleModifier), GenerateRandomDeviation(localScale.z, randomScaleModifier));
        localScale = randomScaleDimensions;
        return block;
    }

    private static GameObject SetBlockClassProperties(GameObject block, BlockSettings blockSettings)
    {
        var blockClass = block.GetComponent<Block>();
        blockClass.blockIsInTowerZone = true;
        blockClass.userCanDrag = false;
        blockClass.userCanNudge = true;
        return block;
    }

    public static GameObject SetNonUniqueBlockSettings(GameObject block, BlockSettings blockSettings)
    {
        block = SetBlockClassProperties(block, blockSettings);
        block = SetRigidBodyDimensions(block, blockSettings);
        return block;
    }

    public static GameObject InitializeBlockParamsAndSetTransform(TowerInitDetails initDetails, GameObject block, float palletCreatorIndex)
    {
        
        var positionOffset = (block.transform.localScale.z + initDetails.blockSettings.BlockSpacing) * palletCreatorIndex; //Distance from midpoints, in units
        block.transform.Translate(new Vector3(positionOffset, 0f, 0f));
        block = randomizeBlockDimensions(block, initDetails.blockSettings.RandomnessIndex);
        block.transform.parent = initDetails.TowerCollisionBox.transform;
        block.GetComponent<Block>().Init(initDetails.gameController, 
                                         initDetails.cursorController, 
                                         initDetails.blockSettings.BlockMover, 
                                         initDetails.blockSettings.DistanceNeededForMouseDrag, 
                                         initDetails.blockSettings.TimeOnMouseDownNeededForDrag);
        return block;
    }


    //This started as a way to reduce arguments for tower & block creation functions but it's a bit bloated and basically a global variable holder
    //TODO: See if this can be cleaned up, some functions are basically unreadable due to this
    public struct TowerInitDetails
    {
        public BlockSettings blockSettings { get; set; }
        public GameObject initBlockPrefab { get; set; }
        public CameraController cam { get; set; }
        public GameController gameController { get; set; }
        public GameObject TowerCollisionBox { get; set; }
        public GameObject towerTop { get; set; }
        public GameObject towerArea { get; set; }
        public int nPallets { get; set; }

        public Vector3 dropZonePosition { get; set; }

        public CursorController cursorController { get; set; }
        public GameObject TowerDropZone { get; internal set; }

        public GameObject MidwayBlockMovePoint { get; set; }

        public TowerInitDetails(BlockSettings blockSettings, 
                                CameraController cameraController, 
                                GameController gameController, 
                                CursorController cursorController,
                                GameObject midwayBlockMovePoint,
                                int nPallets = 0, 
                                GameObject blockPrefab = null) : this()
        {
            this.blockSettings = blockSettings;
            initBlockPrefab = blockPrefab;
            cam = cameraController;
            this.gameController = gameController;
            this.nPallets = nPallets;
            this.cursorController = cursorController;
            MidwayBlockMovePoint= midwayBlockMovePoint;
        }

        public void SetTowerCollisionBoxAndDropZone(GameObject box)
        {
            TowerCollisionBox = box;
            var height = nPallets * blockSettings.BlockHeight;
            dropZonePosition = new Vector3(TowerCollisionBox.transform.position.x, height);
            towerTop.transform.position = new Vector3(TowerCollisionBox.transform.position.x, height + 1f);
            dropZonePosition = new Vector3(TowerCollisionBox.transform.position.x, height);
            TowerCollisionBox.transform.position = new Vector3(TowerCollisionBox.transform.position.x, height / 2f, TowerCollisionBox.transform.position.z);
        }
    }
}
