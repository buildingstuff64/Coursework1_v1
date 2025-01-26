using DelaunatorSharp;
using DelaunatorSharp.Unity.Extensions;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Create_Map_V3 : MonoBehaviour
{
    public GameObject islandFab;
    public GameObject cube;

    [Header("MapGeneration")]
    public float islandAreaRadius;
    public int islandIterationCount;
    [Space(10)]


    [Header("IslandGeneration")]
    public float islandRadiusMin;
    public float islandRadiusMax;
    public int islandVertexCount;
    public float randomness;
    public int pathBlockingIterations;

    // Start is called before the first frame update
    void Start()
    {

        Island start = spawnIsland(Vector2.down * islandAreaRadius);
        Island end = spawnIsland(Vector2.up * islandAreaRadius);
        List<Island> islands = spawnIslands(start, end, islandIterationCount);
        List<Vector2> points = new List<Vector2>();
        foreach (Island island in islands)
        {
            points.Add(island.point);
        }

        Delaunator delaunator = new Delaunator(points.ToPoints());
        List<DelaunatorSharp.Edge> edges = new List<DelaunatorSharp.Edge>();
        foreach (var edge in delaunator.GetEdges())
        {
            Vector3 p = edge.P.ToVector3() * 2;
            p.z = p.y;
            p.y = 0;
            Island ip = getIslandFromRay(p);
            


            Vector3 q = edge.Q.ToVector3() * 2;
            q.z = q.y;
            q.y = 0;
            Island iq = getIslandFromRay(q);

            Debug.DrawLine(ip.center, iq.center, Color.magenta, 1000f);    

            ip.connections.Add(iq);
            iq.connections.Add(ip);

            if (Vector2.Distance(p, q) < islandAreaRadius/4)
            {
                //Debug.DrawLine(p, q, Color.red, 1000f, false);
                
            }
        }

        //List<Island> finalIslands = new List<Island>();
        Pathfinding pathfinding = new Pathfinding(start, end);
        //List<Island> path = pathfinding.findPath(start, end);
        //Debug.Log(path);
        //foreach (Island i in path)
        //{
        //    i.GetComponent<MeshRenderer>().material.color = Color.blue;
        //}

        //path[Random.Range(0, path.Count)].isWalkable = false;

        //List<Island> pathnew = pathfinding.findPath(start, end);
        //foreach (Island i in pathnew)
        //{
        //    i.GetComponent<MeshRenderer>().material.color = Color.blue;
        //}

        List< List<Island>> paths = createRandomPaths(pathfinding, pathBlockingIterations);
        foreach (List<Island> path in paths)
        {
            foreach (Island p in path)
            {
                p.connections.Clear();
                p.isWalkable = true;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                //path[i].GetComponent<MeshRenderer>().material.color = Color.blue;
                Debug.DrawLine(path[i].center, path[i + 1].center, Color.red, 1000f);

                path[i].next.Add(path[i + 1]);
                path[i + 1].previous.Add(path[i]);

            }
        }

        List<Island> finalIslands = new List<Island>();
        foreach (List<Island> path in paths) { finalIslands.AddRange(path); }
        finalIslands = finalIslands.Distinct().ToList();

        foreach (Island i in islands)
        {
            if (!finalIslands.Contains(i))
            {
                DestroyImmediate(i.gameObject);
            }
        }

        foreach (Island i in finalIslands)
        {

            foreach (Island next in i.next)
            {
                createBridge(i, next);
            }
        }

    }

    void createBridge(Island from, Island to)
    {
        Vector3 midPoint = Vector3.Lerp(from.center, to.center, 0.5f);
        GameObject g = Instantiate(cube, midPoint, Quaternion.identity, transform);
        g.transform.LookAt(to.center);
        g.transform.localScale = new Vector3(10, 1, Vector3.Distance(from.center, to.center));
        g.transform.position += Vector3.down * 0.51f;
    }

    List<List<Island>> createRandomPaths(Pathfinding pathfinding, int iterations)
    {
        List<Island> final = new List<Island>();
        List<List<Island>> paths = new List<List<Island>>();
        final.AddRange(pathfinding.findPath());
        for (int i = 0; i < iterations; i++)
        {
            Island remove = final[Random.Range(0, final.Count)];
            remove.isWalkable = false;

            List<Island> path = pathfinding.findPath();
            if (path == null || path.Count<1)
            {
                remove.isWalkable = true;
            }
            else
            {
                final.AddRange(path);
                paths.Add(path);
            }

            final = final.Distinct().ToList();

        }
        return paths;
    }

    Island getIslandFromRay(Vector3 pos)
    {
        if (Physics.Raycast(pos + Vector3.up + Vector3.back*0.001f, Vector3.down, out RaycastHit hit))
        {
            //Debug.DrawLine(pos + (Vector3.up * 100), pos, Color.blue, 1000f);
            GameObject go = hit.collider.gameObject;
            go.GetComponent<Island>().center = pos;
            Island i = go.GetComponent<Island>();
            if (i == null) Debug.Log(pos);
            return i;
        }
        return null;
    }


    Island spawnIsland(Vector2 pos)
    {
        GameObject go = Instantiate(islandFab, new Vector3(pos.x, 0, pos.y), Quaternion.identity, transform);
        Island island = go.GetComponent<Island>();
        float radius = Random.Range(islandRadiusMin, islandRadiusMax);
        island.createIslandParams(pos, islandVertexCount, radius, randomness, new float[] { 2f, radius, 2.4f });
        island.createIslandMesh();
        go.name = island.point.ToString();
        return island;
    }

    List<Island> spawnIslands(Island start, Island end, int count)
    {
        List<Island> islands = new List<Island>{ start, end };
        islands[0].isStart = true;
        islands[1].isEnd = true;
        for (int i = 0; i < count; i++)
        {
            Island island = spawnIsland(Random.insideUnitCircle * islandAreaRadius);
            //Debug.DrawLine(island.transform.position * 2, island.transform.position * 2 + Vector3.up * 2, Color.green, 1000f);
            bool x = false;
            for (int j = 0; j < islands.Count; j++)
            {
                if (Vector3.Distance(islands[j].transform.position, island.transform.position) < island.farEdgeMax + islands[j].farEdgeMax)
                {
                    DestroyImmediate(island.gameObject);
                    x = true;
                    break;
                }
            }
            if (x) continue;  
            islands.Add(island);
        }
        return islands;
    }

}
