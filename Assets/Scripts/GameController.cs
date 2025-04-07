using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Canvas canvas;
    public MapData mapData;
    public GraphClass graph;
    public PathFinder pathFinder;
    public DFS DFSAlgorithm;
    public Greedy GreedyAlgorithm;
    public AStar AstarAlgorithm;
    public int startX = 0;
    public int startY = 0;
    public int goalX = 3;
    public int goalY = 1;
    public float timeStep = 0.1f;
    private bool buttonFlag = false;
    public TMP_Dropdown dropdown;
    public enum Algorithms
    {
        BFS,
        DFS,
        Greedy,
        AStarManhattan,
        AStarEuclidean
    }

    public Algorithms algorithms;

    void Start()
    {
        if (mapData != null && graph != null)
        {
            int[,] mapInstance = mapData.MakeMap(); // 2D array of 1's and 0's
            graph.Init(mapInstance); // Convert the above to array of nodes
            GraphView graphView = graph.gameObject.GetComponent<GraphView>();
            if (graphView != null)
            {
                graphView.Init(graph);
            }
            else
            {
                Debug.LogWarning("No graph is found.");
            }
            if (graph.IsWithinBounds(startX, startY) && graph.IsWithinBounds(goalX, goalY) && pathFinder != null)
            {
                Debug.LogWarning("pathfinder init successfully runs");
                Node startNode = graph.nodes[startX, startY];
                Node goalNode = graph.nodes[goalX, goalY];

                Time.timeScale = 0;
                canvas.transform.SetAsLastSibling();

                Debug.LogWarning("Algorithms enum = " + algorithms.ToString());
                StartCoroutine(Algorithmselect(graphView, startNode, goalNode));
            }
        }
    }

    private IEnumerator Algorithmselect(GraphView graphView, Node startNode, Node goalNode)
    {
        while (!buttonFlag)
        {
            yield return null;
        }

        if (algorithms == Algorithms.BFS)
        {
            // BFS
            pathFinder.Init(graph, graphView, startNode, goalNode);
            StartCoroutine(pathFinder.SearchRoutine(timeStep));
        }
        else if (algorithms == Algorithms.DFS)
        {
            // DFS
            DFSAlgorithm.Init(pathFinder, graph, graphView, startNode, goalNode);
            StartCoroutine(DFSAlgorithm.DFSAlgorithm(timeStep));
        }
        else if (algorithms == Algorithms.Greedy)
        {
            // GreedyAlgorithm
            GreedyAlgorithm.Init(pathFinder, graph, graphView, startNode, goalNode);
            StartCoroutine(GreedyAlgorithm.GreedyAlgorithm(timeStep));
        }
        else if (algorithms == Algorithms.AStarManhattan)
        {
            // A* Manhattan
            AstarAlgorithm.Init(pathFinder, graph, graphView, startNode, goalNode, true);
            StartCoroutine(AstarAlgorithm.AStarAlgorithm(timeStep));
        }
        else if (algorithms == Algorithms.AStarEuclidean)
        {
            // A* Heuristic
            AstarAlgorithm.Init(pathFinder, graph, graphView, startNode, goalNode, false);
            StartCoroutine(AstarAlgorithm.AStarAlgorithm(timeStep));
        }
        else
        {
            yield return null;
        }
    }

    public void OnDropDownChange(TMP_Dropdown dropdown)
    {
        algorithms = (Algorithms)dropdown.value;
        Debug.Log("Selected Enum: " + algorithms);
    }

    public void StartScript()
    {
        Time.timeScale = 1;
        buttonFlag = true;
    }

    public void ResetScript()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
