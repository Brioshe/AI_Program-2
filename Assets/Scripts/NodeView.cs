using UnityEngine;

public class NodeView : MonoBehaviour
{
    public GameObject tile;
    [Range(0,0.5f)]
    public float borderSize = 0.15f;
    public void Init(Node node)
    {
        if (tile != null)
        {
            tile.name = "Node (" + node.position.x + ", " + node.position.z + ")";
            tile.transform.position = node.position;
            Debug.Log(gameObject.transform.position.x + ", " + gameObject.transform.position.z);
            tile.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);
        }
        else
        {
            Debug.LogWarning("Tile does not exist!");
        }
    }

    private void ColorNode(Color color, GameObject gameObject)
    {
        if (gameObject != null)
        {
            Renderer gameObjectRenderer = gameObject.GetComponent<Renderer>();
            gameObjectRenderer.material.color = color;
        }
    }

    public void ColorNode(Color color)
    {
        ColorNode(color, tile);
    }
}