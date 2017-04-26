using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Variables;

public class miniMap : MonoBehaviour {

    //ミニマップを構成する画像を格納する配列
    GameObject[,] minimap;
    //現在のモード
    int currentmode = 2;
    //プレイヤーの位置を表示するオブジェクト
    public GameObject playerpos;


    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPlayer()
    {
        //プレイヤーの位置を表示
        playerpos = Instantiate(Resources.Load("Prefabs/Map/playerpos"),
                            new Vector3(0, 0, 0),
                            Quaternion.identity) as GameObject;
        playerpos.transform.SetParent(GameObject.Find("MiniMap").transform);
        playerpos.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        playerpos.transform.localPosition = -Map.OFFSET;
        playerpos.transform.localEulerAngles = Map.direction;
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
            linebuffer = buffer[2 + Map.mapY - i].Split(',');

            for (int j = 0; j < Map.mapX; j++)
            {
                mapdata[j, i] = int.Parse(linebuffer[j]);
                //最初に作る位置はどこでもいい
                minimap[j, i] = Instantiate(Resources.Load(mapchip[mapdata[j, i]]),
                           new Vector3(0, 0, 0),
                           Quaternion.identity) as GameObject;
                //Canvasの子オブジェクトにする
                minimap[j, i].transform.SetParent(Map.minimapcanvas.transform);
                //自分の周囲だけ表示
                if ((i <= Map.playerpos.y + (Map.range * currentmode) && i >= Map.playerpos.y - (Map.range * currentmode)) &&
                         (j <= Map.playerpos.x + (Map.range * currentmode) && j >= Map.playerpos.x - (Map.range * currentmode)))
                {
                    minimap[j, i].transform.gameObject.SetActive(true);
                }
                else
                {
                    minimap[j, i].transform.gameObject.SetActive(false);
                }
                //位置調整
                minimap[j, i].transform.localScale = new Vector3(0.5f, 0.5f, 1);
                minimap[j, i].transform.localPosition = new Vector3(j * 10, i * 10, 0) 
                                                                    - Map.OFFSET
                                                                     - new Vector3(Map.playerpos.x * 10, Map.playerpos.y * 10);

            }
        }   
    }

    //表示モード、通常、拡大、非表示
    public void displaymode(int mode)
    {
        currentmode = mode;
        //非表示
        if (currentmode == 0)
        {
            int count = 0;
            foreach (Transform child in Map.minimap.transform)
            {
                child.gameObject.SetActive(false);
                count++;
            }
        }
        //拡大モード
        else if (currentmode == 1)
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
        else if (currentmode == 2)
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
    }

    //位置などの更新
    public void updateminimap()
    {
        if (currentmode == 0)
        {

        }
        else if (currentmode == 1 || currentmode == 2)
        {
            playerpos.GetComponent<Transform>().gameObject.SetActive(true);
            for (int i = 0;i< Map.mapY; i++)
            {
                for (int j = 0; j < Map.mapX; j++)
                {
                    //自分の周囲だけ表示
                    if ((i <= Map.playerpos.y + (Map.range * currentmode) && i >= Map.playerpos.y - (Map.range * currentmode)) &&
                         (j <= Map.playerpos.x + (Map.range * currentmode) && j >= Map.playerpos.x - (Map.range * currentmode)))
                    {
                        minimap[j,i].transform.gameObject.SetActive(true);
                    }
                    else
                    {
                        minimap[j, i].transform.gameObject.SetActive(false);
                    }
                    minimap[j, i].transform.localPosition = new Vector3(j * 10, i * 10, 0) * (3 - currentmode)
                                                                        - Map.OFFSET
                                                                        - new Vector3(Map.playerpos.x * 10, Map.playerpos.y * 10, 0) * (3 - currentmode);
                }
            }
        }


    }

}
