using System.Collections;
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

    // Start is called before the first frame update
    void Start()
    {

        this.currentGameSettings = gameSettingsObj.GetComponent<GameSettings>();
        this.turnIndex = 0;
        this.test2PlayerGame();

        GameObject newGO = new GameObject("myTextGO");
        newGO.transform.SetParent(this.textUI.transform);

        
        this.turnIndex = 0;
        GameObject textObj = new GameObject("p1 text");
        this.cursorInstance = this.PlayerList[0].cursorController;
        CurrentTurnState = TurnState.GetBlock;
        var cameraController = camerControllerObj.GetComponent<CameraController>();
        TowerInitDetails details = new TowerInitDetails(currentGameSettings.BlockSettings, 
                                                        cameraController, 
                                                        this, 
                                                        cursorInstance);
        var gameReady = this.tower.GenerateTower(details, currentGameSettings.NPalletsHigh);
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
        this.PlayerList.Add(new Player(Color.red, "james", 0, this.cursorControllerObj));
        this.PlayerList.Add(new Player(Color.cyan, "cyan", 0, this.cursorControllerObj));
    }

    public void GoToTurnState(TurnState state)
    {
        this.CurrentTurnState = state;
    }

    public void DropState()
    {
        this.tower.ActivateBlockPlacingZone();
    }

    public void FinishTurn()
    {
        turnIndex++;
        if(turnIndex%3 == 0)
        {
            this.currentGameSettings.NPalletsHigh++;
            this.tower.towerCollisionBox.UpdateTowerBoxBounds(this.currentGameSettings.NPalletsHigh);
            var newYPosition = this.currentGameSettings.NPalletsHigh * this.currentGameSettings.BlockSettings.BlockHeight;
            this.tower.UpdateBlockPlacingZonePosition(newYPosition);
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