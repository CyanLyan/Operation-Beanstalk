using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public List<Player> PlayerList = new List<Player>();

    private float turnIndex;

    public Player CurrentPlayer;

    public TurnState CurrentTurnState;

    public Canvas textUI;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        this.turnIndex = 0;
        this.test2PlayerGame();

        GameObject newGO = new GameObject("myTextGO");
        newGO.transform.SetParent(this.textUI.transform);

        for (int i = 0; i < this.PlayerList.Count; i++)
        {
            Player currentPlayer = this.PlayerList[i];
            GameObject textObj = new GameObject("p" + i.ToString() + "text");
            this.addPlayer(currentPlayer, textObj);
        }
        this.turnIndex = 0;
        CurrentPlayer = this.PlayerList[0];
        CurrentTurnState = TurnState.GetBlock;
    }

    void addPlayer(Player p, GameObject t)
    {
        Text myText = t.AddComponent<Text>();
        myText.text += p.playerName + ": " + p.score + "\n";
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