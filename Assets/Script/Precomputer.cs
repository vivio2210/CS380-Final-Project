using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Precomputer : MonoBehaviour
{
    [SerializeField]
    private int[,] clusterStep = new int[20,20];

    [SerializeField]
    public FloorColorController floorColorController;

    public float threshold = 0.7f;

    private void Start()
    {
        AStarPather.initialize();

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                clusterStep[i, j] = 0;
            }
        }

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                for (int k = 0; k < 20; k++)
                {
                    for (int l = 0; l < 20; l++)
                    {
                        List<Gridpos> path = AStarPather.compute_path(new Gridpos(i,j), new Gridpos(k, l));
                        for (int m = 0; m < path.Count; m++)
                        {
                            clusterStep[path[m].posx, path[m].posz] += 1;
                        }
                    }
                }
            }
        }
        float max = -1;
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (clusterStep[i, j] > max)
                {
                    max = clusterStep[i, j];
                }
            }
        }

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (((float)clusterStep[i, j] / max) >= threshold)
                {
                    floorColorController.ChangeFloorColor(i, j, new Color(1.0f, 1.0f - ((float)clusterStep[i, j] / max),
                        1.0f - ((float)clusterStep[i, j] / max), 1.0f));
                }
            }
        }

    }


}
