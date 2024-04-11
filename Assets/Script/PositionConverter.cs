using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using Unity.VisualScripting;

[Serializable]
public class Gridpos
{
    [SerializeField]
    public int posx, posz;

    public Gridpos(int x, int z) {
        posx = x; posz = z;
    }
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
        return new Vector3(gridpos.posx,1,gridpos.posz);
    }

    public static Vector3 GridPosToWorld(int x, int z)
    {
        return new Vector3(x, 1, z);
    }
}
