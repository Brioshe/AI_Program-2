using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellState {
    dead = 0,
    alive = 1
}

public class Node
{
    public CellState cellState = CellState.dead;
    // To track position in 2D Array
    public int xIndex = -1;
    public int yIndex = -1;
    public Vector3 position;
    public List<Node> neighbors = new List<Node>();
    public Node previous = null;
    // Constructor with three parameters
    public Node(int xIndex, int yIndex, CellState cellState)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.cellState = cellState;
    }

    public void Reset()
    {
        previous = null;
    }
}
