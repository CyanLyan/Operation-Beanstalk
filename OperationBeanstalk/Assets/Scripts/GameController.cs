using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public List<Player> PlayerList = new List<Player>();

    private float turnIndex;

    public Canvas textUI;

    // Start is called before the first frame update
    void Start()
    {

        this.turnIndex = 0;
        this.test2PlayerGame();
        for(int i = 0; i < this.PlayerList.Count; i++)
        {
            Player currentPlayer = this.PlayerList[i];
            this.addPlayer(currentPlayer);
        }
    }

    void addPlayer(Player p)
    {
        Transform t = textUI.transform.Find("P1Text");
        t.GetComponent<Text>().text += p.playerName + ": " + p.score + "\n";
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
}
