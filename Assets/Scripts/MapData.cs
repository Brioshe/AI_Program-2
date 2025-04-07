using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public int height = 5;
    public int width = 5;

    public TextAsset text;

    public List<string> getTextFromFile(TextAsset text)
    {
        List<string> line = new List<string>();
        if (text != null)
        {
            string textData = text.text;
            string[] delimeters = { "\r\n", "\n" };
            line = textData.Split(delimeters, System.StringSplitOptions.None).ToList();
        }
        return line;
    }

    public List<string> getTextFromFile()
    {
        return getTextFromFile(text);
    }

    public void setDimensions(List<string> textLines)
    {
        height = textLines.Count();
        foreach(string line in textLines)
        {
            width = line.Length;
        }
    }

    // syntax for making a 2D array
    public int[,] MakeMap()
    {
        List<string> lines = getTextFromFile();
        setDimensions(lines);
        int[,] map = new int[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = (int)char.GetNumericValue(lines[y][x]);
            }
        }
        return map;
    }
}