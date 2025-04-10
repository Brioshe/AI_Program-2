using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool cellAlive = false;

    // To track position in 2D Array
    public int xIndex = -1;
    public int yIndex = -1;
    public Vector3 position;
    public List<Node> neighbors = new List<Node>();
    public Node previous = null;
    // Constructor with three parameters
    public Node(int xIndex, int yIndex, bool cellAlive)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.cellAlive = cellAlive;
    }

    public void Reset()
    {
        previous = null;
    }
}
