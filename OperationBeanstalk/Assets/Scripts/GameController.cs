using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public List<Player> PlayerList;

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
        Player p1 = new Player();
        p1.playerName = "yasha";
        p1.color = Color.blue;
        p1.score = 0;

        Player p2 = new Player();
        p2.playerName = "cyan";
        p2.color = Color.cyan;
        p2.score = 0;

        this.PlayerList.Add(p1);
        this.PlayerList.Add(p2);
    }
}
