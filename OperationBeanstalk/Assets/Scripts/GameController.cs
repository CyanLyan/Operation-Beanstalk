using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BlockBuilder;

public class GameController : MonoBehaviour
{

    public List<Player> PlayerList = new List<Player>();

    public TowerController tower;

    public List<KeyValuePair<string, GameSettings>> gameSettingsList = new List<KeyValuePair<string, GameSettings>>();

    public GameSettings currentGameSettings;

    public GameObject gameSettingsObj;

    public GameObject blockPrefab;

    private float turnIndex;

    public GameObject PlayerPrefab;

    public Player CurrentPlayer;

    public TurnState CurrentTurnState;

    public Canvas textUI;

    private CursorController cursorInstance;
    
    public GameObject cursorControllerObj;

    public GameObject camerControllerObj;
    
    //TODO - create tool/toggle for point drag vs. frozen rotation drag
    public GameObject dragBoxToolObj;

    public GameObject midwayBlockMovePoint;

    // Start is called before the first frame update
    void Start()
    {

        currentGameSettings = gameSettingsObj.GetComponent<GameSettings>();
        turnIndex = 0;
        test2PlayerGame();

        GameObject newGO = new GameObject("myTextGO");
        newGO.transform.SetParent(textUI.transform);

        
        turnIndex = 0;
        GameObject textObj = new GameObject("p1 text");
        cursorInstance = PlayerList[0].cursorController;
        CurrentTurnState = TurnState.GetBlock;
        var cameraController = camerControllerObj.GetComponent<CameraController>();
        cameraController.cameraViewPivotSpeed = currentGameSettings.cameraViewPivotSpeed;
        TowerInitDetails details = new TowerInitDetails(currentGameSettings.BlockSettings, 
                                                        cameraController, 
                                                        this, 
                                                        cursorInstance, 
                                                        midwayBlockMovePoint);
        details.blockSettings.BlockMover = details.blockSettings.BlockMoverObj.GetComponent<BlockMover>();
        details.blockSettings.BlockMover.gameController = this;
        var gameReady = tower.GenerateTower(details, currentGameSettings.NPalletsHigh);
    }

    GameObject addPlayer(Player p, GameObject t)
    {
        Text myText = t.AddComponent<Text>();
        myText.text += p.playerName + ": " + p.score + "\n";
        var newPlayer = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        return newPlayer;
    }
    
    void test2PlayerGame()
    {
        PlayerList.Add(new Player(Color.red, "james", 0, cursorControllerObj));
        PlayerList.Add(new Player(Color.cyan, "cyan", 0, cursorControllerObj));
    }

    public void GoToTurnState(TurnState state)
    {
        CurrentTurnState = state;
    }

    public void DropState()
    {
        tower.ActivateBlockPlacingZone();
    }

    public void FinishTurn()
    {
        turnIndex++;
        if(turnIndex%3 == 0)
        {
            currentGameSettings.NPalletsHigh++;
            tower.towerCollisionBox.UpdateTowerBoxBounds(currentGameSettings.NPalletsHigh);
            var newYPosition = currentGameSettings.NPalletsHigh * currentGameSettings.BlockSettings.BlockHeight;
            tower.UpdateBlockPlacingZonePosition(newYPosition);
        }
    }

    public void SaveTower()
    {
        //Freeze all rigidbodies on tower
    }
}

public enum TurnState {
    Mgmt,
    GetBlock,
    PlaceBlock
}