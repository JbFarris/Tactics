using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    public bool walkable = true;

    public List<TileScript> adjacencyList = new List<TileScript>();

    // BFS

    public bool visted = false;
    public TileScript parent = null;
    public int distance = 0;

    // A*
    public float f = 0;
    public float g = 0;
    public float h = 0;

    void Start()
    {

    }
    void Update()
    {
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.green;

        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.cyan;

        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void Reset()
    {
        adjacencyList.Clear();
        current = false;
        target = false;
        selectable = false;
        walkable = true;


        visted = false;
        parent = null;
        distance = 0;

        f = g = h = 0;
    }
    public void FindNeighbors(float jumpHeight, TileScript target)
    {
        Reset();
        CheckTile(Vector3.forward, jumpHeight, target);
        CheckTile(-Vector3.forward, jumpHeight, target);
        CheckTile(Vector3.right, jumpHeight, target);
        CheckTile(-Vector3.right, jumpHeight, target);


    }
    public void CheckTile(Vector3 direction, float jumpHeight, TileScript target)
    {
        Vector3 halfExtents = new Vector3(.25f, (1 + jumpHeight) / 2.0f, .25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            TileScript tile = item.GetComponent<TileScript>();
            if (tile != null && tile.walkable)
            {
                RaycastHit hit;

                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target))
                {
                    adjacencyList.Add(tile);
                }

            }
        }

    }
}
