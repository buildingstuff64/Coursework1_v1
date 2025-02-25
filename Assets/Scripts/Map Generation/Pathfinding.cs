using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//A* pathfinding
public class Pathfinding
{
    public Island start;
    public Island end;

    public Pathfinding(Island start, Island end)
    {
        this.start = start;
        this.end = end;
    }

    public List<Island> findPath()
    {
        List<Island> open = new List<Island>();
        List<Island> closed = new List<Island>();
        open.Add(start);

        while (open.Count > 0)
        {
            Island node = open[0];
            //node.GetComponent<MeshRenderer>().material.color = Color.blue;
            for (int i = 0; i < open.Count; i++)
            {
                if (open[i].fCost < node.fCost || open[i].fCost == node.fCost)
                {
                    if (open[i].hCost < node.hCost) node = open[i];
                }
            }

            open.Remove(node);
            closed.Add(node);

            if (node == end)
            {
                return RetracePath(start, end);
            }

            foreach (Island neighbour in node.connections)
            {
                if (!neighbour.isWalkable || closed.Contains(neighbour)) continue;

                float newCostToNeighbour = node.gCost + Vector3.Distance(node.transform.position, neighbour.transform.position);
                if (newCostToNeighbour < neighbour.gCost || !open.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = Vector3.Distance(neighbour.transform.position, end.transform.position);
                    neighbour.prev = node;

                    if (!open.Contains(neighbour)) open.Add(neighbour);
                }
            }
        }
        return null;
    }

    List<Island> RetracePath(Island start, Island end)
    {
        List<Island> path = new List<Island>();
        Island currentNode = end;


        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.prev;
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
}

