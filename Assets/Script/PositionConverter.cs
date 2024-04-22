using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.Burst.CompilerServices;
using UnityEngine.UIElements;

[Serializable]
public class Gridpos
{
    [SerializeField]
    public int posx, posz;

    public Gridpos(int x, int z) {
        posx = x; posz = z;
    }

    public static Gridpos operator +(Gridpos rhs1, Gridpos rhs2)
    {
        Gridpos result = new Gridpos(0,0);
        result.posx = rhs1.posx + rhs2.posx;
        result.posz = rhs1.posz + rhs2.posz;
        return result;
    }

    public static Gridpos operator -(Gridpos rhs1, Gridpos rhs2)
    {
        Gridpos result = new Gridpos(0, 0);
        result.posx = rhs1.posx - rhs2.posx;
        result.posz = rhs1.posz - rhs2.posz;
        return result;
    }

    //public static bool operator ==(Gridpos rhs1, Gridpos rhs2)
    //{
    //    return (rhs1.posx == rhs2.posz && rhs1.posz == rhs2.posz);
    //}
}

public class PositionConverter
{
    public static Gridpos WorldToGridPos(Vector3 worldposition)
    {
        Gridpos result = new Gridpos((int)worldposition.x, (int)worldposition.z);
        return result;
    }

    public static Vector3 GridPosToWorld(Gridpos gridpos)
    {
        return new Vector3(gridpos.posx + 0.5f,1,gridpos.posz+ 0.5f);
    }

    public static Vector3 GridPosToWorld(int x, int z)
    {
        return new Vector3(x + 0.5f, 1, z + 0.5f);
    }

    // To round up the decimal number to grid pos
    public static Vector3 WorldToWorld(Vector3 worldposition)
    {
        return new Vector3((int)worldposition.x + 0.5f, 1, (int)worldposition.z + 0.5f);
    }
}

public class MapChecker 
{
    public static bool IsValid(int posx, int posz)
    {
        return (posx >= 0 && posx < 20) && (posz >= 0 && posz < 20);
    }

    public static bool IsWall(Gridpos gridpos)
    {
        RaycastHit hit;
        Vector3 position = PositionConverter.GridPosToWorld(gridpos);
        position.y = 2;
        Vector3 direction = new Vector3(0, -1, 0);

        if (Physics.Raycast(position, direction, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.tag == "Wall")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsWall(int posx, int posz)
    {
        RaycastHit hit;
        Vector3 position = PositionConverter.GridPosToWorld(posx, posz);
        position.y = 2;
        Vector3 direction = new Vector3(0, -1, 0);

        if (Physics.Raycast(position, direction, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.tag == "Wall")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsWallOrEnemy(Gridpos gridpos)
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        Vector3 position = PositionConverter.GridPosToWorld(gridpos);
        Vector3[] offsetVector = new Vector3[5]{ new Vector3(0,0,0), new Vector3(-0.45f,0,0),
                                new Vector3(0.45f,0,0), new Vector3(0,0,-0.45f), new Vector3(0,0,0.45f) };
        position.y = 2;
        Vector3 direction = new Vector3(0, -1, 0);
        RaycastHit hit;

        for (int i = 0; i < 5; i++)
        {
            Vector3 temp = position + offsetVector[i];
            if (Physics.Raycast(temp, direction, out hit, Mathf.Infinity))
            {
                hits.Add(hit);
            }
        }

        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i].transform.gameObject.tag == "Enemy" || hits[i].transform.gameObject.tag == "Wall")
            {
                return true;
            }
        }

        return false;
    }
}
