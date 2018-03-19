using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentCollider : MonoBehaviour
{

   

    void Start()
    {

     
       Vector3 down = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position, down, 1))
        {
            GameObject notWalkable = GameObject.Find("Tile");
            TileScript playerScript = notWalkable.GetComponent<TileScript>();
            playerScript.walkable = false;
          
        }

    }
}
