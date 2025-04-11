using UnityEngine;

public class StateSwitchHandler : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                 NodeView nodeView = hit.collider.GetComponent<NodeView>();
                if (nodeView != null && nodeView.node != null)
                {
                    if (nodeView.node.cellState == CellState.dead)
                    {
                        nodeView.node.cellState = CellState.alive;
                    }
                    else if (nodeView.node.cellState == CellState.alive)
                    {
                        nodeView.node.cellState = CellState.dead;
                    }
                }
            }
        }
    }
}
