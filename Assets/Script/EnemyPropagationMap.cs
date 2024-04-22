using Unity.Mathematics;

//public class NodeLayer
//{
//    public float layerValue = 0.0f;
//    public Gridpos gridPos = new Gridpos(0, 0);
//    public bool[] neighbors = new bool[8];
//}

public class EnemyPropagationMap
{
    public static float[,] layer = new float[20,20];
    public static Gridpos[] surroundNeighbors = { new Gridpos(-1,1)  ,new Gridpos(0,1)    ,new Gridpos(1,1),
                                     new Gridpos(-1,0)                   ,new Gridpos(1,0) ,
                                     new Gridpos(-1,-1)  ,new Gridpos(0,-1)   ,new Gridpos(1,-1) };

    private static float ROOT2 = 1.414213f;

    public static float decay = 0.05f;

    public static float growth = 1.00f * 4;

    public static bool initialize()
    {
        int width = 20;
        int height = 20;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (MapChecker.IsWall(i, j))
                {
                    continue;
                }
                layer[i, j] = 1.0f;
            }
        }

        return true;
    }
    public static void Propagate(double deltaTime)
    { 
        int width = 20;
        int height = 20;

        float [,] templayer = new float[20, 20];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float highestValue = -1.0f;
                for (int k = 0; k < 8; k++)
                {
                    if (!MapChecker.IsValid(i + (int)surroundNeighbors[k].posx, j + (int)surroundNeighbors[k].posz))
                    {
                        continue;
                    }
                    if (MapChecker.IsWall(i + (int)surroundNeighbors[k].posx, j + (int)surroundNeighbors[k].posz))
                    {
                        continue;
                    }
                    float value = -1.0f;
                    if (k == 0 || k == 2 || k == 5 || k == 7) // diaganal
                    {
                        if (k == 0)
                        {
                            if (MapChecker.IsWall(i + (int)surroundNeighbors[1].posx, 
                                j + (int)surroundNeighbors[1].posz) ||
                                MapChecker.IsWall(i + (int)surroundNeighbors[3].posx, 
                                j + (int)surroundNeighbors[3].posz))
                            {
                                continue;
                            }
                        }
                        else if (k == 2)
                        {
                            if (MapChecker.IsWall(i + (int)surroundNeighbors[1].posx, j + (int)surroundNeighbors[1].posz) || 
                                MapChecker.IsWall(i + (int)surroundNeighbors[4].posx, j + (int)surroundNeighbors[4].posz))
                            {
                                continue;
                            }
                        }
                        else if (k == 5)
                        {
                            if (MapChecker.IsWall(i + (int)surroundNeighbors[3].posx, j + (int)surroundNeighbors[3].posz) || MapChecker.IsWall(i + (int)surroundNeighbors[6].posx, j + (int)surroundNeighbors[6].posz))
                            {
                                continue;
                            }
                        }
                        else if (k == 7)
                        {
                            if (MapChecker.IsWall(i + (int)surroundNeighbors[4].posx, j + (int)surroundNeighbors[4].posz) || MapChecker.IsWall(i + (int)surroundNeighbors[6].posx, j + (int)surroundNeighbors[6].posz))
                            {
                                continue;
                            }
                        }
                        value = layer[i + (int)surroundNeighbors[k].posx, j + (int)surroundNeighbors[k].posz] * (float)math.exp(-ROOT2 * decay);
                    }
                    else
                    {
                        value = layer[i + (int)surroundNeighbors[k].posx, j + (int)surroundNeighbors[k].posz] * (float)math.exp(-1 * decay);
                    }
                    if (value > highestValue)
                    {
                        highestValue = value;
                    }
                }
                templayer[i,j] = math.lerp(layer[i, j], highestValue, (float)deltaTime * growth);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (MapChecker.IsWall(i, j))
                {
                    continue;
                }
                layer[i, j] = templayer[i,j];
            }
        }
    }

    public static void NormalizePropagate(double deltaTime)
    {
        int width = 20;
        int height = 20;

        float hishestValue = -100.0f;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (layer[i, j] > hishestValue)
                {
                    hishestValue = layer[i, j];
                }
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float currentvalue = layer[i, j];
                if (currentvalue > 0.0f)
                {
                    layer[i, j] = currentvalue / hishestValue;
                }
            }
        }


    }
}
