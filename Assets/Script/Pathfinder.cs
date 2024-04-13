using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.AI;

public enum Nodelist
{
    E_NONELIST,
    E_OPENLIST,
    E_CLOSELIST
};

public class GridNode
{
    private float ROOT2 = 1.414213f;
    public GridNode parent = null;
    public Gridpos gridPos = new Gridpos(0,0);
    public float finalCost = 0;
    public float givenCost = 0;
    public Nodelist onlist = Nodelist.E_NONELIST;
    public bool[] neighbors = new bool[8]; // false = wall , true = can walk

    // Neighbor Map
    //	0	1	2
    //	3	X	4
    //	5	6	7

    public GridNode(int x, int y)
    {
        gridPos.posx = x;
        gridPos.posz = y;
    }
    public void calurateFinalCost(Gridpos goalpos, float givenCost)
    {
        this.givenCost = givenCost;
        this.finalCost = this.givenCost + calurateHeuristic(goalpos);
    }
    public float calurateHeuristic(Gridpos goalpos)
    {
        int xDiff = math.abs(gridPos.posx - goalpos.posx);
        int yDiff = math.abs(gridPos.posz - goalpos.posz);
        return (float)((math.min(xDiff, yDiff) * ROOT2) + math.max(xDiff, yDiff) - math.min(xDiff, yDiff));
    }
}

public class AStarPather
{
    public static GridNode[,] gridNodes = new GridNode[20,20];
    public static Gridpos[] surroundNeighbors = { new Gridpos(-1,1)  , new Gridpos(0,1)    ,new Gridpos(1,1),
                                    new Gridpos(-1,0)                   ,new Gridpos(1,0) ,
                                    new Gridpos(-1,-1)  , new Gridpos(0,-1)   ,new Gridpos(1,-1) };

    private static List<GridNode> openlist = new List<GridNode>();
    private static float ROOT2 = 1.414213f;
    public static bool initialize()
    {
        InitGridNode();
        ComputeNeighbor();
        return true; 
    }
    private static int FindCheapestNodeIndex()
    {
        int index = -1;
        float currentcost = 5000;
        for (int i = 0; i < openlist.Count; i++)
        {
            if (openlist[i].finalCost < currentcost)
            {
                currentcost = openlist[i].finalCost;
                index = i;
            }
        }
        return index;
    }
    public static List<Gridpos> compute_path(Gridpos start, Gridpos goal)
    {
        List<Gridpos> pathResult = new List<Gridpos>();

        if (MapChecker.IsWall(goal))
        {
            Debug.Log("IMPOSSIBLE");
            return new List<Gridpos>();
        }
        ClearGridNode();
        gridNodes[start.posx, start.posz].onlist = Nodelist.E_OPENLIST;
        gridNodes[start.posx, start.posz].calurateFinalCost(goal, 0);
        gridNodes[start.posx, start.posz].parent = null;
        openlist.Add(gridNodes[start.posx,start.posz]);

        while (openlist.Count > 0)
        {
            int parentNodeIndex = FindCheapestNodeIndex();
            GridNode parentNode = openlist[parentNodeIndex];
            openlist.RemoveAt(parentNodeIndex);
            if (parentNode.gridPos.posx == goal.posx && parentNode.gridPos.posz == goal.posz)
            {
                GridNode currentNode = parentNode;
                while (currentNode.parent != null)
                {
                    pathResult.Insert(0, currentNode.gridPos);
                    currentNode = currentNode.parent;
                }
                pathResult.Insert(0, start);
                return pathResult;
            }
            parentNode.onlist = Nodelist.E_CLOSELIST;
            for (int i = 0; i < 8; i++)
            {
                if (parentNode.neighbors[i])
                {
                    Gridpos childpos = parentNode.gridPos + surroundNeighbors[i];
                    GridNode childNode = gridNodes[childpos.posx,childpos.posz];
                    if (childNode.onlist == Nodelist.E_NONELIST)
                    {
                        if (i == 0 || i == 2 || i == 5 || i == 7) // is diaganal
                        {
                            childNode.calurateFinalCost(goal, parentNode.givenCost + ROOT2);
                        }
                        else
                        {
                            childNode.calurateFinalCost(goal, parentNode.givenCost + 1.0f);
                        }

                        childNode.onlist = Nodelist.E_OPENLIST;
                        childNode.parent = parentNode;
                        openlist.Add(childNode);
                    }
                    else // in open or close list
                    {
                        float currentfinalcost = childNode.finalCost;
                        float newfinalcost = parentNode.givenCost + childNode.calurateHeuristic(goal);
                        float stepDistance = -1.0f;
                        if (i == 0 || i == 2 || i == 5 || i == 7) // is diaganal
                        {
                            newfinalcost += ROOT2;
                            stepDistance = ROOT2;
                        }
                        else
                        {
                            newfinalcost += 1.0f;
                            stepDistance = 1.0f;
                        }
                        if (newfinalcost < currentfinalcost)
                        {
                            childNode.calurateFinalCost(goal, parentNode.givenCost + stepDistance);
                            childNode.parent = parentNode;
                            if (childNode.onlist == Nodelist.E_CLOSELIST)
                            {
                                childNode.onlist = Nodelist.E_OPENLIST;
                                openlist.Add(childNode);
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("IMPOSSIBLE");
        return new List<Gridpos>();
    }
    public static void InitGridNode()
    {
        int width = 20;
        int height = 20;

        for (int j = 0; j < width; j++)
        {
            for (int i = 0; i < height; i++)
            {
                gridNodes[i,j] = new GridNode(i, j);
            }
        }
    }

    public static void ComputeNeighbor()
    {
        int width = 20;
        int height = 20;

        for (int j = 0; j < width; j++)
        {
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    Gridpos neighborPos = new Gridpos(0, 0);
                    neighborPos.posx = gridNodes[i,j].gridPos.posx + surroundNeighbors[k].posx;
                    neighborPos.posz = gridNodes[i,j].gridPos.posz + surroundNeighbors[k].posz;
                    if (neighborPos.posz >= 0 && neighborPos.posz < height && neighborPos.posx >= 0 && neighborPos.posx < width)
                    {
                        if (MapChecker.IsWall(neighborPos))
                        {
                            gridNodes[i, j].neighbors[k] =  false;
                        }
                        else
                        {
                            gridNodes[i, j].neighbors[k] = true;
                        }
                    }
                    else
                    {
                        gridNodes[i, j].neighbors[k] = false;
                    }
                }

                if (gridNodes[i,j].neighbors[1] == false)
                {
                    gridNodes[i, j].neighbors[0] = false;
                    gridNodes[i, j].neighbors[2] = false;
                }
                if (gridNodes[i, j].neighbors[3] == false)
                {
                    gridNodes[i, j].neighbors[0] = false;
                    gridNodes[i, j].neighbors[5] = false;
                }
                if (gridNodes[i, j].neighbors[4] == false)
                {
                    gridNodes[i, j].neighbors[2] = false;
                    gridNodes[i, j].neighbors[7] = false;
                }
                if (gridNodes[i, j].neighbors[6] == false)
                {
                    gridNodes[i, j].neighbors[5] = false;
                    gridNodes[i, j].neighbors[7] = false;
                }

            }
        }
    }

    private static void ClearGridNode()
    {
        int width = 20;
        int height = 20;

        openlist.Clear();

        for (int j = 0; j < width; j++)
        {
            for (int i = 0; i < height; i++)
            {
                gridNodes[i,j].onlist = Nodelist.E_NONELIST;
            }
        }
    }
};
