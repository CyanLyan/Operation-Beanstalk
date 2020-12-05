using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawLine
{
    // Start is called before the first frame 

    public static void Draw(GameObject myLine, Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        //GameObject myLine = new GameObject();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        myLine.transform.position = start;
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }

    public static void ResetLine(GameObject myLine)
    {
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.SetPosition(0, Vector3.zero);
        lr.SetPosition(1, Vector3.zero);
    }
}
