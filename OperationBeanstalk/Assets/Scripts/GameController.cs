using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject cursorInstance;

    // Start is called before the first frame update
    void Start()
    {
        this.turnIndex = 0;
        this.test2PlayerGame();

        GameObject newGO = new GameObject("myTextGO");
        newGO.transform.SetParent(this.textUI.transform);

        
        this.turnIndex = 0;
        CurrentPlayer = this.PlayerList[0];
        CurrentTurnState = TurnState.GetBlock;

        var gameReady = this.tower.GenerateTower(gameSettingsObj.GetComponent<GameSettings>().BlockSettings, 15);
        if(gameReady)
        {
            //for (int i = 0; i < this.PlayerList.Count; i++)
            //{
            //    Player currentPlayer = this.PlayerList[i];
            //    GameObject textObj = new GameObject("p" + i.ToString() + "text");
            //}

            Player currentPlayer = this.PlayerList[0];
            GameObject textObj = new GameObject("p0" + "text");
            this.addPlayer(this.PlayerList[0], textObj);
        }
    }

    void addPlayer(Player p, GameObject t)
    {
        Text myText = t.AddComponent<Text>();
        myText.text += p.playerName + ": " + p.score + "\n";
        var newPlayer = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    void test2PlayerGame()
    {
        this.PlayerList.Add(new Player(Color.red, "james", 0));
        this.PlayerList.Add(new Player(Color.cyan, "cyan", 0));
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