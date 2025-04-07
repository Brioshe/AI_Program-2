using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class AStar : MonoBehaviour
{
    PathFinder pathFinder;
    Node Start;
    Node Goal;
    GraphClass Graph;
    GraphView GraphView;
    List<Node> FrontierNodes;
    List<Node> ExploredNodes;
    List<Node> PathNodes;
    bool manhattan;
    Dictionary<Node, int> startDistance;

    public bool isComplete;
    public int iterations = 0;
    public int maxStored = 0;

    public void Init(PathFinder pathFinder, GraphClass graph, GraphView graphView, Node start, Node goal, bool manhattan)
    {
        if (start == null || goal == null || graph == null || graphView == null)
        {
            Debug.LogWarning("A* Algorithm Init error: missing components.");
            return;
        }
        if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
        {
            Debug.LogWarning("A* Algorithm Init error: Start or goal node cannot be a blocked node!");
            return;
        }

        this.GraphView = graphView;
        this.pathFinder = pathFinder;
        this.Graph = graph;
        this.Goal = goal;
        this.Start = start;

        FrontierNodes = new List<Node>();
        FrontierNodes.Add(start);
        ExploredNodes = new List<Node>();
        PathNodes = new List<Node>();

        startDistance = new Dictionary<Node, int>();
        startDistance.Add(start, 0);

        for (int y = 0; y < Graph.m_height; y++)
        {
            for (int x = 0; x < Graph.m_width; x++)
            {
                this.Graph.nodes[x, y].Reset();
            }
        }

        isComplete = false;
        this.manhattan = manhattan;
    }

    public IEnumerator AStarAlgorithm(float timeStep)
    {
        yield return null;
        while (!isComplete)
        {
            if(FrontierNodes.Count > 0)
            {
                Node currentNode = FrontierNodes[0];
                foreach (Node n in FrontierNodes)
                {
                    if (AStarDist(n, Goal) < AStarDist(currentNode, Goal))
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

                if (FrontierNodes.Contains(Goal))
                {
                    PathNodes = pathFinder.GetPathNodes(Goal);
                    pathFinder.showColors(GraphView, Start, Goal, FrontierNodes, ExploredNodes, PathNodes);
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

            pathFinder.showColors(GraphView, Start, Goal, FrontierNodes, ExploredNodes, PathNodes);
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
                startDistance.Add(n, startDistance[node] + 1);
            }
        }
    }

    float AStarDist(Node start, Node goal)
    {
        return startDistance[start] + (manhattan ? Greedy.ManhattanDistCalc(start, goal) : EuclideanDistCalc(start, goal)); 
    }

    float EuclideanDistCalc(Node n1, Node n2)
    {
        return Mathf.Sqrt(Mathf.Pow(n1.xIndex-n2.xIndex, 2) + Mathf.Pow(n1.yIndex - n2.yIndex, 2));
    }
}