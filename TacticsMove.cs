using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{

    List<TileScript> selectableTiles = new List<TileScript>();
    GameObject[] tiles;


    Stack<TileScript> path = new Stack<TileScript>();
    TileScript currentTile;

   
    public int move = 5;
    public float jumpHeight = 2;
    public int moveSpeed = 3;
    public float jumpVelocity = 4.5f;

    bool fallingDown = false;
    bool jumpingUp = false;
    bool movingEdge = false;
    public bool turn = false;
    public bool moving = false;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();
    Vector3 jumpTarget = new Vector3();

    float halfHeight = 0;


    public TileScript actualTargetTile;


    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfHeight = GetComponent<Collider>().bounds.extents.y;

        TurnManger.AddUnit(this);
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    public TileScript GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        TileScript tile = null;
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<TileScript>();
        }
        return tile;
    }
    public void ComputeAdjacencyLists(float jumpHeight, TileScript target)
    {
        foreach (GameObject tile in tiles)
        {
            TileScript t = tile.GetComponent<TileScript>();
            t.FindNeighbors(jumpHeight, target);
        }

    }
    public void FindSelectableTiles()
    {
        ComputeAdjacencyLists(jumpHeight, null);
        GetCurrentTile();

        Queue<TileScript> process = new Queue<TileScript>();

        process.Enqueue(currentTile);
        currentTile.visted = true;

        while (process.Count > 0)
        {
            TileScript t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;
            if (t.distance < move)
            {
                foreach (TileScript tile in t.adjacencyList)
                {
                    if (!tile.visted)
                    {
                        tile.parent = t;
                        tile.visted = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }

        }
    }
    public void MoveToTile(TileScript tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;

        TileScript next = tile;
        while(next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void Move()
    {
        if (path.Count > 0)
        {
            TileScript t = path.Peek();
            Vector3 target = t.transform.position;

            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if(Vector3.Distance(transform.position, target) >= 0.05f)
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            RemoveSelectableTiles();
            moving = false;
            TurnManger.EndTurn();
        }
    }
    protected void RemoveSelectableTiles()
    {
        if(currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
       
        }
        foreach (TileScript tile in selectableTiles)
        {
            tile.Reset();
        
        }
        selectableTiles.Clear();

    }
    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();

    }

    void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;

    }

    void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallingDownward(target);
        }
        else if (jumpingUp)
        {
            JumpUpward(target);
        }
        else if (movingEdge)
        {
            MovetoEdge();
        }
        else
        {
            PrepareJump(target);
        }
    }
    void PrepareJump(Vector3 target)
    {
        float targetY = target.y;

        target.y = transform.position.y;

        CalculateHeading(target);
        if(transform.position.y > targetY)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;

            jumpTarget = transform.position + (target - transform.position) / 2.0f;
        }
        else
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;

            velocity = heading * moveSpeed / 3.0f;

            float difference = targetY - transform.position.y;

            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }
    }
    void FallingDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if(transform.position.y <= target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;

            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;

            velocity = new Vector3();
        }
    }
    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;
        if(transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;
        }
    }
    void MovetoEdge()
    {
        if(Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();

        }
        else
        {
            movingEdge = false;
            fallingDown = true;

            velocity /= 5.0f;
            velocity.y = 1.5f;

        }
    }

    protected TileScript FindLowestF(List<TileScript> list)
    {
        TileScript lowest = list[0];

        foreach (TileScript t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;

            }
        }

        list.Remove(lowest);
        return lowest;
    }
        
    protected TileScript FindEndTile(TileScript t)
    {
        Stack<TileScript> tempPath = new Stack<TileScript>();
        TileScript next = t.parent;

        while(next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        if(tempPath.Count <= move)
        {
            return t.parent;

        }

        TileScript endTile = null;

        for(int i = 0; i <= move; i++)
        {
            endTile = tempPath.Pop();

        }
        return endTile;


    }
    protected void FindPath(TileScript target)
    {
        ComputeAdjacencyLists(jumpHeight, target);
        GetCurrentTile();

        List<TileScript> openList = new List<TileScript>();
        List<TileScript> closedList = new List<TileScript>();

        openList.Add(currentTile);
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
        currentTile.f = currentTile.h;

        while(openList.Count > 0)
        {
            TileScript t = FindLowestF(openList);

            closedList.Add(t);

            if(t == target)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            foreach (TileScript tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {
                    //nothing
                } 
                else if (openList.Contains(tile))
                {
                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (tempG < tile.g)
                    {
                        tile.parent = t;

                        tile.g = tempG;
                        tile.f = tile.g + tile.h;
                         
                    }
                }
                else
                {
                    tile.parent = t;
                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.g + tile.h;

                    openList.Add(tile);

                       
                }

            }
        }

        //fix this. Find new path.
        Debug.Log("Path Not Found");
    }


    public void BeginTurn()
    {
        turn = true;
    }
    public void EndTurn()
    {
        turn = false;
    }
}
