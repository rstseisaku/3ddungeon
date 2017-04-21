using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1 : MonoBehaviour {
    
    //マップのサイズ(X,y)
    int MapX;
    int MapY;
    
    //マップチップの名前
    string[] MapChip;
    //オブジェクトの通行可否、床オブジェクトか壁オブジェクトか
    int[] ObjectType;
    //マップの座標
    int[,] MapData;

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
		
	}

    //マップを作成する関数
    public void MakeMap(string FileName)
    {
        //ファイルパス(固定)
        string FilePath = "Assets\\Resources\\MapData\\";
        //受け取ったマップ名と合成
        FilePath += FileName;

        //一時的に文字列を格納するファイル
        string[] buffer;
        string[] linebuffer;

        //bufferにファイルから読み込んだものを格納
        buffer = System.IO.File.ReadAllLines(FilePath);
        //カンマで区切られている情報ごとに分割
        linebuffer = buffer[0].Split(',');
        //マップのサイズを格納
        MapX = int.Parse(linebuffer[0]);
        MapY = int.Parse(linebuffer[1]);
        MapData = new int[MapX, MapY];
        //マップ生成に使うプレハブの取得
        MapChip = buffer[1].Split(',');
        ObjectType = new int[MapChip.Length];

        //オブジェクトの種類の取得
        linebuffer = buffer[2].Split(',');
        for (int i = 0; i < ObjectType.Length; i++) // マップチップの要素数(種類)
        {
            //何もない所は-1として置く
            if(linebuffer[i] == "")
            {
                ObjectType[i] = -1;
            }
            else
            {
                ObjectType[i] = int.Parse(linebuffer[i]);
            }
        }

        //マップデータを起こす
        for (int i = 0; i < MapY; i++)
        {
            linebuffer = buffer[2 + MapY - i].Split(',');
            
            for (int j = 0; j < MapX; j++)
            {
                /*
                 MapData[x, y]に直した
                 */
                MapData[j, i] = int.Parse(linebuffer[j]);
            }
        }

        //マップデータ通りにプレハブを配置
        for (int i = 0; i < MapY; i++)
        {

            for (int j = 0; j < MapX; j++)
            {
                float posY = 0.0f;
                //床オブジェクト
                if (ObjectType[MapData[j, i]] == 0 || ObjectType[MapData[j, i]] == 1)
                {
                    posY = 0.0f;
                }
                //壁オブジェクト
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
