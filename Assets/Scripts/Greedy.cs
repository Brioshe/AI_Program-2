using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Greedy : MonoBehaviour
{
    PathFinder pathFinder;
    Node Start;
    Node Goal;
    GraphClass Graph;
    GraphView GraphView;
    List<Node> FrontierNodes;
    List<Node> ExploredNodes;
    List<Node> PathNodes;

    public bool isComplete;
    public int iterations = 0;
    public int maxStored = 0;

    public void Init(PathFinder pathFinder, GraphClass graph, GraphView graphView, Node start, Node goal)
    {
        if (start == null || goal == null || graph == null || graphView == null)
        {
            Debug.LogWarning("Greedy Algorithm Init error: missing components.");
            return;
        }
        if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
        {
            Debug.LogWarning("Greedy Algorithm Init error: Start or goal node cannot be a blocked node!");
            return;
        }

        this.pathFinder = pathFinder;
        this.Graph = graph;
        this.GraphView = graphView;
        this.Start = start;
        this.Goal = goal;
        FrontierNodes = new List<Node>();
        FrontierNodes.Add(start);
        ExploredNodes = new List<Node>();
        PathNodes = new List<Node>();

        for (int y = 0; y < Graph.m_height; y++)
        {
            for (int x = 0; x < Graph.m_width; x++)
            {
                Graph.nodes[x, y].Reset();
            }
        }
    }
    public IEnumerator GreedyAlgorithm(float timeStep)
    {
        yield return null;
        while (!isComplete)
        {
            if (FrontierNodes.Count > 0)
            {
                Node currentNode = FrontierNodes[0];
                foreach (Node n in FrontierNodes)
                {
                    if (ManhattanDistCalc(currentNode, Goal) > ManhattanDistCalc(n, Goal))
                    {
                        currentNode = n;
                    }
                }
                FrontierNodes.Remove(currentNode);
                iterations++;

                if(!ExploredNodes.Contains(currentNode))
                {
                    ExploredNodes.Add(currentNode);
                }

                ExpandFrontier(currentNode);

                if(FrontierNodes.Contains(Goal))
                {
                    PathNodes = pathFinder.GetPathNodes(Goal);
                    pathFinder.showColors(GraphView, Start, Goal, FrontierNodes.ToList(), ExploredNodes, PathNodes);
                    isComplete = true;
                }
                yield return new WaitForSeconds(timeStep);
            }
            else
            {
                isComplete = true;
            }

            int totalExplored = ExploredNodes.Count + FrontierNodes.Count;

            Debug.Log("Iterations: " + iterations);
            Debug.Log("Explored Nodes: " + totalExplored);
            Debug.Log("Max Frontier: " + maxStored);

            pathFinder.showColors(GraphView, Start, Goal, FrontierNodes.ToList(), ExploredNodes, PathNodes);
        }
    }

    public void ExpandFrontier(Node node)
    {
        foreach (Node n in node.neighbors)
        {
            if (n.nodeType != NodeType.Blocked && !ExploredNodes.Contains(n) && !FrontierNodes.Contains(n))
            {
                FrontierNodes.Add(n);
                if (FrontierNodes.Count() > maxStored) 
                {
                    maxStored = FrontierNodes.Count();
                } 
                n.previous = node;
            }
        }
    }

    public static int ManhattanDistCalc(Node n1, Node n2)
    {
        return(Mathf.Abs((n2.xIndex - n1.xIndex) + (n2.yIndex - n1.yIndex)));
    }
}