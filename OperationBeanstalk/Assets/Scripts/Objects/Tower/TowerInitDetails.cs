using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject TowerDropZone { get; internal set; }

    public GameObject MidwayBlockMovePoint { get; set; }

    public TowerInitDetails(BlockSettings blockSettings,
                            CameraController cameraController,
                            GameController gameController,
                            GameObject midwayBlockMovePoint,
                            int nPallets = 0,
                            GameObject blockPrefab = null) : this()
    {
        this.blockSettings = blockSettings;
        initBlockPrefab = blockPrefab;
        cam = cameraController;
        this.gameController = gameController;
        this.nPallets = nPallets;
        MidwayBlockMovePoint = midwayBlockMovePoint;
    }

    public void SetTowerCollisionBoxAndDropZone(GameObject box)
    {
        TowerCollisionBox = box;
        var height = (nPallets * blockSettings.BlockHeight) + (blockSettings.BlockHeight*5);
        TowerCollisionBox.transform.position = new Vector3(TowerCollisionBox.transform.position.x, height / 2f, TowerCollisionBox.transform.position.z);
        dropZonePosition = new Vector3(TowerCollisionBox.transform.position.x, height);
        towerTop.transform.position = new Vector3(TowerCollisionBox.transform.position.x, height);
        dropZonePosition = new Vector3(TowerCollisionBox.transform.position.x, height);
        TowerDropZone.transform.position = dropZonePosition;
    }
}
