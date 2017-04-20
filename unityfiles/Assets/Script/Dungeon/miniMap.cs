using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miniMap : MonoBehaviour {

    int MiniMapX;
    int MiniMapY;

    public GameObject minimap;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetMinimap(string Filename) {

        //絶対もっといい書き方ある

        minimap = GameObject.Find("MiniMap");
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
                temp.transform.SetParent(minimap.transform);
                temp.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                temp.transform.localPosition = new Vector3(i * 10 - 100, j * 10 - 100, 0);
            }
        }   
    }

    //表示モード、通常、拡大、非表示
    public void displaymode(int mode)
    {
        if(mode == 0)
        {
            int count = 0;
            foreach (Transform child in minimap.transform)
            {
                child.gameObject.SetActive(false);
                count++;
            }
        }
        else if (mode == 1)
        {
            int count = 0;
            foreach (Transform child in minimap.transform)
            {

                child.gameObject.SetActive(true);
                child.transform.localScale = new Vector3(1, 1, 1);
                child.transform.localPosition += new Vector3(100, 100, 0);
                child.transform.localPosition *= 2.0f;
                child.transform.localPosition -= new Vector3(100, 100, 0);
                count++;
            }
        }
        else if (mode == 2)
        {
            int count = 0;
            foreach (Transform child in minimap.transform)
            {
                child.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                child.transform.localPosition += new Vector3(100, 100, 0);
                child.transform.localPosition *= 0.5f;
                child.transform.localPosition -= new Vector3(100, 100, 0);
                count++;
            }
        }
        else
        {
            Debug.Log("ここが出ちゃ駄目");
        }
        Debug.Log("called");
    }
}
