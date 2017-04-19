using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1 : MonoBehaviour {
    
    int MapX;
    int MapY;
    
    string[] MapChip;
    int[] ObjectType;
    int[,] MapData;

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void MakeMap(string FileName)
    {
        string FilePath = "Assets\\Resources\\MapData\\";
        FilePath += FileName;

        string[] buffer;
        string[] linebuffer;
        buffer = System.IO.File.ReadAllLines(FilePath);

        linebuffer = buffer[0].Split(',');
        MapX = int.Parse(linebuffer[0]);
        MapY = int.Parse(linebuffer[1]);
        MapData = new int[MapX, MapY];

        MapChip = buffer[1].Split(',');
        ObjectType = new int[MapChip.Length];

        linebuffer = buffer[2].Split(',');
        for (int i = 0; i < ObjectType.Length; i++) // マップチップの要素数(種類)
        {
            if(linebuffer[i] == "")
            {
                ObjectType[i] = -1;
            }
            else
            {
                ObjectType[i] = int.Parse(linebuffer[i]);
            }
        }

        for (int i = 0; i < MapY; i++)
        {
            linebuffer = buffer[2 + MapY - i].Split(',');
            
            for (int j = 0; j < MapX; j++)
            {
                /*
                 MapData[y, x]
                 */
                MapData[j, i] = int.Parse(linebuffer[j]);
            }
        }

        for (int i = 0; i < MapY; i++)
        {

            for (int j = 0; j < MapX; j++)
            {
                float posY = 0.0f;
                if (ObjectType[MapData[j, i]] == 0 || ObjectType[MapData[j, i]] == 1)
                {
                    posY = 0.0f;
                }
                else if(ObjectType[MapData[j, i]] == 2 || ObjectType[MapData[j, i]] == 3)
                {
                    posY = 0.5f;
                }

                Instantiate(Resources.Load(MapChip[MapData[j, i]]),
                            new Vector3(j, posY, i), // Plane を 0.1 倍にすると 1x1 になる
                            Quaternion.identity);
            }
        }
    } //---MakeMap


    // input: 移動先の座標
    public bool isMoveable(int x,int y) 
    {
        Debug.Log(x + ", " + y);
        int tmp = ObjectType[MapData[x, y]];
        if (tmp == 0 || tmp == 2) return true;
        else return false;
    }
}
