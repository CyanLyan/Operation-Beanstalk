using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BlockBuilder;

public class GameController : MonoBehaviour
{

    public List<Player> PlayerList = new List<Player>();

    public Tower tower;

    public List<KeyValuePair<string, GameSettings>> gameSettings = new List<KeyValuePair<string, GameSettings>>();

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

    // Start is called before the first frame update
    void Start()
    {
        this.turnIndex = 0;
        this.test2PlayerGame();

        GameObject newGO = new GameObject("myTextGO");
        newGO.transform.SetParent(this.textUI.transform);

        
        this.turnIndex = 0;
        GameObject textObj = new GameObject("p1 text");
        //var localPlayer = this.addPlayer(this.PlayerList[0], textObj);

        //CurrentPlayer = this.PlayerList.GetComponent<Player>();

        //for (int i = 0; i < this.PlayerList.Count; i++)
        //{
        //    Player currentPlayer = this.PlayerList[i];
        //}
        this.cursorInstance = this.PlayerList[0].cursorController;
        CurrentTurnState = TurnState.GetBlock;
        var cameraController = camerControllerObj.GetComponent<CameraControl>();

        //cursorInstance = cursorControllerObj.GetComponent<CursorController>();

        TowerInitDetails details = new TowerInitDetails(gameSettingsObj.GetComponent<GameSettings>().BlockSettings, 
                                                        cameraController, 
                                                        this, 
                                                        cursorInstance);

        var gameReady = this.tower.GenerateTower(details);
    }

    GameObject addPlayer(Player p, GameObject t)
    {
        Text myText = t.AddComponent<Text>();
        myText.text += p.playerName + ": " + p.score + "\n";
        var newPlayer = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        return newPlayer;
    }
    
    // Update is called once per frame
    void Update()
    {
        
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
}


public enum TurnState {
    Mgmt,
    GetBlock,
    PlaceBlock
}