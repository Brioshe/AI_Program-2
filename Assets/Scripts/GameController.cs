using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Canvas canvas;
    public MapData mapData;
    public GraphClass graph;
    public CellMechanics cellMechanics;

    public bool IsGameplayRunning = false;
    public bool IsGamePaused = false;

    // Gametick Slider
    public UnityEngine.UI.Slider slider;
    [Range(0,1f)]
    public float tickInterval = 0.5f;
    public float sliderVal;
    public TextMeshProUGUI sliderText;

    // Buttons
    public UnityEngine.UI.Button Startbutton;
    public TextMeshProUGUI playText;
    public TextMeshProUGUI stopText;
    public UnityEngine.UI.Button Pausebutton;
    public UnityEngine.UI.Button Stepbutton;

    // Colors

    public Color redNormal = new Color32(236, 108, 108, 255);
    public Color redHighlighted = new Color32(171, 64, 64, 255);
    public Color redPressed = new Color32(128, 51, 51, 255);

    public Color greenNormal = new Color32(142, 241, 140, 255);
    public Color greenHighlighted = new Color32(111, 185, 109, 255);
    public Color greenPressed = new Color32(87, 137, 86, 255);

    public Color blueNormal = new Color32(170, 209, 227, 255);
    public Color blueHighlighted = new Color32(131, 183, 212, 255);
    public Color bluePressed = new Color32(89, 143, 173, 255);

    public Color defaultNormal = new Color32(255, 255, 255, 255);
    public Color defaultHighlighted = new Color32(226, 226, 226, 255);
    public Color defaultPressed = new Color32(185, 184, 184, 255);

    public enum MapPresets {

   }


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
                Debug.LogWarning("Graph Size: " + graph.m_width + " x " + graph.m_height);

                stopText.enabled = false;
                Pausebutton.interactable = false;

                cellMechanics.Init(graph, graphView);
                StartCoroutine(GOLCoroutine());
            }
        }
    }

    void Update()
    {
        // Slider Components
        sliderVal = slider.value;
        if (tickInterval != sliderVal)
        {
            tickInterval = sliderVal;
            sliderText.text = (Math.Round(sliderVal * 100f) / 100).ToString();         
        }
    }

    IEnumerator GOLCoroutine()
    {
        while (true) {
            while (!IsGameplayRunning || IsGamePaused)
            {
                yield return new WaitForSeconds(tickInterval);
            }

            while (IsGameplayRunning && !IsGamePaused) {
                cellMechanics.UpdateCellStates();
                Debug.Log("TICK");
                yield return new WaitForSeconds(tickInterval);
            }
        }
    }

    public void PauseUnpausePress()
    {
        ColorBlock cb = Pausebutton.colors;
        
        if (IsGamePaused)
        {
            cb.normalColor = defaultNormal;
            cb.highlightedColor = defaultHighlighted;
            cb.pressedColor = defaultPressed;
            Pausebutton.colors = cb;
        }
        if (!IsGamePaused)
        {
            cb.normalColor = blueNormal;
            cb.highlightedColor = blueHighlighted;
            cb.pressedColor = bluePressed;
            Pausebutton.colors = cb;
        }

        IsGamePaused = !IsGamePaused;
    }

    public void StartStopPress()
    {
        if (IsGameplayRunning)
        {
            playText.enabled = true;
            stopText.enabled = false;

            ResetState();

            IsGamePaused = false;
            Pausebutton.interactable = false;

            ColorBlock pcb = Pausebutton.colors;
            pcb.normalColor = defaultNormal;
            pcb.highlightedColor = defaultHighlighted;
            pcb.pressedColor = defaultPressed;
            Pausebutton.colors = pcb;

            ColorBlock cb = Startbutton.colors;
            cb.normalColor = greenNormal;
            cb.highlightedColor = greenHighlighted;
            cb.pressedColor = greenPressed;
            Startbutton.colors = cb;
        }

        if (!IsGameplayRunning)
        {
            playText.enabled = false;
            stopText.enabled = true;
            IsGamePaused = false;
            Pausebutton.interactable = true;

            ColorBlock cb = Startbutton.colors;
            cb.normalColor = redNormal;
            cb.highlightedColor = redHighlighted;
            cb.pressedColor = redPressed;
            Startbutton.colors = cb;
        }

        IsGameplayRunning = !IsGameplayRunning;
    }

    public void GameStepOnce()
    {
        cellMechanics.UpdateCellStates();
        Debug.Log("TICK");
    }

    public void ResetState()
    {
        List<string> lines = mapData.getTextFromFile();
        mapData.setDimensions(lines); 

        for (int y = 0; y < graph.m_height; y++)
        {
            for (int x = 0; x < graph.m_width; x++)
            {
                graph.nodes[x, y].cellState = (CellState)char.GetNumericValue(lines[y][x]);
            }
        }

        cellMechanics.UpdateAliveNodes();
    }

    public void SetState(TextAsset text)
    {

    }
}
