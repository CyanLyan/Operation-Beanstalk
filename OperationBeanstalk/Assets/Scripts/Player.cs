using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject cursorControllerObj;

    public CursorController cursorController;

    public Color color { get; set; }
    public string playerName { get; set; }
    public int score { get; set; }

    public Player(Color color, string name, int score, GameObject cursorControllerObj)
    {
        this.color = color;
        this.playerName = name;
        this.score = score;
        this.cursorControllerObj = Instantiate(cursorControllerObj);
        this.cursorController = cursorControllerObj.GetComponent<CursorController>();
    }

    void Start()
    {
        //this.cursorControllerObj = Instantiate(cursorControllerObj);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
