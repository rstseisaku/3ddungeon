using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miniMap : MonoBehaviour {

    int MiniMapX;
    int MiniMapY;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetMinimap(string Filename) {
        
        //絶対もっといい書き方ある
        string FilePath = "Assets\\Resources\\MapData\\";
        FilePath += Filename;

        string[] buffer;
        string[] linebuffer;
        buffer = System.IO.File.ReadAllLines(FilePath);

        linebuffer = buffer[0].Split(',');
        MiniMapX = int.Parse(linebuffer[0]);
        MiniMapY = int.Parse(linebuffer[1]);
        int[,] MapData = new int[MiniMapX, MiniMapY];

        string[] MapChip = buffer[1].Split(',');

        for (int i = 0; i < 5; i++)
        {
            MapChip[i] += "mini";
        }

        GameObject temp;

        for (int i = 0; i < MiniMapY; i++)
        {
            linebuffer = buffer[2 + MiniMapY - i].Split(',');

            for (int j = 0; j < MiniMapX; j++)
            {
                MapData[j, i] = int.Parse(linebuffer[j]);
                temp = Instantiate(Resources.Load(MapChip[MapData[j, i]]),
                           new Vector3(0, 0, 0), // Plane を 0.1 倍にすると 1x1 になる
                           Quaternion.identity) as GameObject;
                temp.transform.SetParent(GameObject.Find("MiniMap").GetComponent<Transform>());
                temp.transform.localScale = new Vector3(1, 1, 1);
                temp.transform.localPosition = new Vector3(i * 20 - 100, j * 20 - 100, 0);
            }
        }   
    }

    //表示モード、通常、拡大、非表示
    void displaymode(int mode)
    {

    }
}
