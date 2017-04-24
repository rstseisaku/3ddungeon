using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Map = Variables.Map;

public class miniMap : MonoBehaviour {

    GameObject[,] minimap;



    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetminiMap(string Filename) {

        //絶対もっといい書き方ある

  
        //マップ作成と基本的に同じ
        string[] buffer;
        string[] linebuffer;

        buffer = System.IO.File.ReadAllLines(Map.filepath + Filename);

        linebuffer = buffer[0].Split(',');

        minimap = new GameObject[Map.mapX, Map.mapY];

        int[,] mapdata = new int[Map.mapX, Map.mapY];

        string[] mapchip = buffer[1].Split(',');

        for (int i = 0; i < mapchip.Length; i++)
        {
            //ミニマップ用のプレハブはマップ用のプレハブ+miniというファイルで作る
            mapchip[i] += "mini";
        }
        
        for (int i = 0; i < Map.mapY; i++)
        {
            linebuffer = buffer[2 + Map.mapX - i].Split(',');

            for (int j = 0; j < Map.mapX; j++)
            {
                mapdata[j, i] = int.Parse(linebuffer[j]);
                //最初に作る位置はどこでもいい
                minimap[j, i] = Instantiate(Resources.Load(mapchip[mapdata[j, i]]),
                           new Vector3(0, 0, 0),
                           Quaternion.identity) as GameObject;
                //Canvasの子オブジェクトにする
                minimap[j, i].transform.SetParent(Map.minimapobject.transform);
                //最初は縮小モード
                minimap[j, i].transform.localScale = new Vector3(0.5f, 0.5f, 1);
                minimap[j, i].transform.localPosition = new Vector3(j * 10, i * 10, 0) - Map.OFFSET;
                
            }
        }   
    }

    //表示モード、通常、拡大、非表示
    public void displaymode(int mode)
    {
        //非表示
        if(mode == 0)
        {
            int count = 0;
            foreach (Transform child in Map.minimap.transform)
            {
                //child.gameObject.SetActive(false);
                count++;
            }
        }
        //拡大モード
        else if (mode == 1)
        {
            int count = 0;
            foreach (Transform child in Map.minimap.transform)
            {
                //child.gameObject.SetActive(true);
                child.transform.localScale = new Vector3(1, 1, 1);
                child.transform.localPosition += Map.OFFSET;
                child.transform.localPosition *= 2.0f;
                child.transform.localPosition -= Map.OFFSET;
                count++;
            }
        }
        //縮小モード
        else if (mode == 2)
        {
            int count = 0;
            foreach (Transform child in Map.minimap.transform)
            {
                child.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                child.transform.localPosition += Map.OFFSET;
                child.transform.localPosition *= 0.5f;
                child.transform.localPosition -= Map.OFFSET;
                count++;
            }
        }
        //念のためにそれ以外になった時
        else
        {
            Debug.Log("ここが出ちゃ駄目");
        }
        Debug.Log("called");
    }
}
