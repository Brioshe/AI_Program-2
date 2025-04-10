using UnityEngine;
using Unity.VisualScripting;
using System;
using System.Collections;
using System.Collections.Generic;

public class CellMechanics : MonoBehaviour
{

    GraphClass Graph;
    GraphView GraphView;

    List<Node> AliveNodes;
    List<Node> AliveNeighbors;

    bool[,] nextStates = new bool[16, 9];

    public void Init(GraphClass Graph, GraphView GraphView)
    {
        if (Graph == null || GraphView == null)
        {
            Debug.LogWarning("CellMechanics Init error: missing components.");
            return;
        }

        this.Graph = Graph;
        this.GraphView = GraphView;
        AliveNodes = new List<Node>();
        Graph.nodes[2, 2].cellAlive = true;
        Graph.nodes[2, 3].cellAlive = true;
        Graph.nodes[2, 4].cellAlive = true;

        Graph.nodes[6, 8].cellAlive = true;
        Graph.nodes[6, 7].cellAlive = true;
        Graph.nodes[6, 6].cellAlive = true;
        Graph.nodes[5, 6].cellAlive = true;
        Graph.nodes[4, 7].cellAlive = true;

        AliveNeighbors = new List<Node>();
    }

    void UpdateAliveNodes()
    {
        foreach (Node n in Graph.nodes)
        {
            if (n.cellAlive == true && !AliveNodes.Contains(n))
            {
                AliveNodes.Add(n);
            }
            if (n.cellAlive == false && AliveNodes.Contains(n))
            {
                AliveNodes.Remove(n);
                NodeView deadNode = GraphView.nodeViews[n.xIndex, n.yIndex];
                deadNode.ColorNode(GraphView.deadColor);
            }
        }
        GraphView.ColorNodes(AliveNodes, GraphView.aliveColor);
    }

    public void UpdateCellStates()
    {
        int aliveNeighborCount = 0;
        foreach (Node n in Graph.nodes)
        {
            aliveNeighborCount = CountAliveNeighbors(n);
            bool nextAlive = n.cellAlive; 
            // Game Rules
            // Death
            if (n.cellAlive) {
                if (aliveNeighborCount < 2) // Underpopulation
                {
                    nextAlive = false;
                }
                if (aliveNeighborCount > 3) // Overpopulation
                {
                    nextAlive = false;
                }
            }
            // Birthing
            if (!n.cellAlive && aliveNeighborCount == 3)
            {
                nextAlive = true;
            }

            nextStates[n.xIndex, n.yIndex] = nextAlive;
        }

        foreach (Node n in Graph.nodes)
        {
            n.cellAlive = nextStates[n.xIndex, n.yIndex];
        }

        UpdateAliveNodes();
    }

    private int CountAliveNeighbors(Node node)
    {
        int aliveCount = 0;
        foreach (Node n in node.neighbors)
        {
            if (n.cellAlive == true)
            {
                aliveCount++;
            }
        }
        if (aliveCount > 0) {
            Debug.Log("Neighbor Count for cell (" + node.xIndex + "," + node.yIndex + "): " + aliveCount );
        }
        return aliveCount;
    }
}