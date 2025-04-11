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
    CellState[,] nextStates;

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
        nextStates = new CellState[Graph.m_width, Graph.m_height];

        UpdateAliveNodes();
    }

    public void UpdateAliveNodes()
    {
        foreach (Node n in Graph.nodes)
        {
            if (n.cellState == CellState.alive && !AliveNodes.Contains(n))
            {
                AliveNodes.Add(n);
            }
            if (n.cellState == CellState.dead && AliveNodes.Contains(n))
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
            CellState nextAlive = n.cellState; 
            // Game Rules
            // Death
            if (n.cellState == CellState.alive && (aliveNeighborCount < 2 || aliveNeighborCount > 3))
            {
                nextAlive = CellState.dead;
            }
            // Birth
            if (n.cellState == CellState.dead && aliveNeighborCount == 3)
            {
                nextAlive = CellState.alive;
            }

            nextStates[n.xIndex, n.yIndex] = nextAlive;
        }

        foreach (Node n in Graph.nodes)
        {
            n.cellState = nextStates[n.xIndex, n.yIndex];
        }

        UpdateAliveNodes();
    }

    private int CountAliveNeighbors(Node node)
    {
        int aliveCount = 0;
        foreach (Node n in node.neighbors)
        {
            if (n.cellState == CellState.alive)
            {
                aliveCount++;
            }
        }
        // if (aliveCount > 0) {
        //     Debug.Log("Neighbor Count for cell (" + node.xIndex + "," + node.yIndex + "): " + aliveCount );
        // }
        return aliveCount;
    }
}