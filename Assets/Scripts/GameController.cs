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
    public CellMechanics cellMechanics;

    [Range(0,1f)]
    public float tickInterval = 0.5f;

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

            if (cellMechanics != null)
            {
                Debug.LogWarning("CellMechanics loaded successfully");
                cellMechanics.Init(graph, graphView);
                StartCoroutine(GOLCoroutine());
            }
        }
    }

    IEnumerator GOLCoroutine()
    {
        while (true)
        {
            cellMechanics.UpdateCellStates();
            Debug.Log("TICK");
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
