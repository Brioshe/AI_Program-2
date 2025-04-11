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
    [Range(1f,0)]
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


    public TMP_Dropdown dropdown;
    public enum MapPresets {
        empty,
        blinkers,
        gliders,
        gun
    }

    public MapPresets mapPreset;

    public TextAsset empty;
    public TextAsset gliders;
    public TextAsset blinkers;
    public TextAsset gun;

    // Flags
    private bool mapSetFlag = false;
    private bool isDragging = false;
    private bool switchStateLock;

    private Node lastNodeRaycast = null;

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
            tickInterval = 1 - sliderVal;
            sliderText.text = (Math.Round(sliderVal * 100f)*2).ToString() + "%";         
        }

        // Get mouse info and change state 
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            GetSwitchState(); // Set lock on switch
            StateSwitchHandler();
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            StateSwitchHandler();
        }
        if(Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }


        cellMechanics.UpdateAliveNodes();
    }

    IEnumerator GOLCoroutine()
    {
        while (true) {

            while (IsGamePaused)
            {
                yield return new WaitForSeconds(tickInterval);
            }

            while (!IsGameplayRunning)
            {
                ChooseMapPreset(); // preset switcher
                yield return new WaitForSeconds(tickInterval);
            }

            while (IsGameplayRunning && !IsGamePaused) {
                cellMechanics.UpdateCellStates();
                yield return new WaitForSeconds(tickInterval);
            }
        }
    }

    public void ChooseMapPreset()
    {
        if (!mapSetFlag) {
            if (mapPreset == MapPresets.empty)
            {
                SetState(empty);
            }
            else if (mapPreset == MapPresets.gliders)
            {
                SetState(gliders);
            }
            else if (mapPreset == MapPresets.blinkers)
            {
                SetState(blinkers);
            }
            else if (mapPreset == MapPresets.gun)
            {
                SetState(gun);
            }
            mapSetFlag = true;
        }
    }

    public void OnDropDownChange(TMP_Dropdown dropdown)
    {
        mapPreset = (MapPresets)dropdown.value;
        Debug.LogWarning("Map Preset: " + mapPreset);
        mapSetFlag = false;
    }

    public void PauseUnpausePress()
    {
        ColorBlock cb = Pausebutton.colors;
        
        if (IsGamePaused)
        {
            cb.normalColor = defaultNormal; // Switch to default colors
            cb.highlightedColor = defaultHighlighted;
            cb.pressedColor = defaultPressed;
            Pausebutton.colors = cb;
        }
        if (!IsGamePaused)
        {
            cb.normalColor = blueNormal;    // Switch to blue colors
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

            mapSetFlag = false;
            ChooseMapPreset();  // Allow map change when game stops.

            IsGamePaused = false;
            Pausebutton.interactable = false;

            ColorBlock pcb = Pausebutton.colors;    // Switch to default colors
            pcb.normalColor = defaultNormal;
            pcb.highlightedColor = defaultHighlighted;
            pcb.pressedColor = defaultPressed;
            Pausebutton.colors = pcb;

            ColorBlock cb = Startbutton.colors;     // Switch to green colors
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

            ColorBlock cb = Startbutton.colors;     // Switch to red colors
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
    }

    public void SetState(TextAsset text)
    {
        List<string> lines = mapData.getTextFromFile(text);
        mapData.setDimensions(lines); 

        for (int y = 0; y < graph.m_height; y++)
        {
            for (int x = 0; x < graph.m_width; x++)
            {
                graph.nodes[x, y].cellState = (CellState)char.GetNumericValue(lines[y][x]); // Set node values to equal map values
            }
        }

        cellMechanics.UpdateAliveNodes();
    }

    public void GetSwitchState()    // Define ray, cast it from camera, detect clicked node's cellstate, and lock switching to only that type.
    {
        Ray switchRay = Camera.main.ScreenPointToRay(Input.mousePosition); // Define ray

        if (Physics.Raycast(switchRay, out RaycastHit hit))
        {
            GameObject clickedObj = hit.collider.gameObject;    
            NodeView nodeView = clickedObj.GetComponentInParent<NodeView>();
            
            Debug.Log("SwitchState probe ray hit @ (" + nodeView.node.xIndex + ", " + nodeView.node.yIndex + ")");

            if (nodeView.node.cellState == CellState.alive)
            {
                switchStateLock = true;
            }
            else if (nodeView.node.cellState == CellState.dead)
            {
                switchStateLock = false;
            }
        }
    }

    public void StateSwitchHandler()    // Define ray, cast ray from camera, switch node that's hit.
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedObj = hit.collider.gameObject;
            NodeView nodeView = clickedObj.GetComponentInParent<NodeView>();
            
            Debug.Log("Raycast hit @ (" + nodeView.node.xIndex + ", " + nodeView.node.yIndex + ")");

            if (nodeView != null && nodeView.node != null && lastNodeRaycast != nodeView.node)
            {
                lastNodeRaycast = nodeView.node;

                if (nodeView.node.cellState == CellState.dead && switchStateLock == false)
                {
                    nodeView.node.cellState = CellState.alive;
                }
                else if (nodeView.node.cellState == CellState.alive && switchStateLock == true)
                {
                    nodeView.node.cellState = CellState.dead;
                }
            }
        }
    }
}
