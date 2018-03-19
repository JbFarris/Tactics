using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if  (UNITY_EDITOR)
using UnityEditor;
public class MenuScript
{ 
    [MenuItem("Tools/Assign Tile Material")]
    public static void AssignTileMaterial()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        Material material = Resources.Load<Material>("TileGrass");
        
        foreach (GameObject t in tiles)
        {
            t.GetComponent<Renderer>().material = material;
        }
    }
    [MenuItem("Tools/Assign Title Script")]
    public static void AssignTileScript()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
    
        foreach (GameObject t in tiles)
        {
            t.AddComponent<TileScript>();
        }

    }
}

#endif