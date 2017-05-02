using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HaveChara : MonoBehaviour {
    public List<GameObject> CharaImage;
    public int touchId = -1;

    /* 利用するオブジェクトを生成する */
    public void GenerateObject( bool[] isObtainChara )
    {
        // キャンバスの取得
        GameObject parentObj = transform.FindChild("Characters").gameObject;
        int haveCharaNum = 0;

        for (int i = 1; i <= Variables.Unit.Num; i++)
        {
            // 獲得していないキャラクターであれば終了
            if (!isObtainChara[i]) continue;

            // 表示オブジェクトの内容をセット
            GameObject obj = Utility.MyInstantiate(
                "Prefabs\\Party\\HaveCharaButton",
                parentObj,
                CharacterStatus.LoadPlayerCharacterStringStatus(i, _ST._faceGraphicPath),
                new Vector2(216,216));          
            obj.GetComponent<RectTransform>().localPosition = Id2Pos(i);
            obj.name = "" + i;
            // ボタンの中身のセット(イベントハンドラの登録)
            Button b = obj.GetComponent<Button>();
            b.onClick.AddListener( () => this.onClick(obj) );

            // 追加
            CharaImage.Add(obj);

            haveCharaNum++;
        }
    }

    // Imageの場所
    private Vector2 Id2Pos(int i)
    {
        i--;
        int x = -472 + ( i % 4 ) * 236;
        int y = 128 + (i / 4) * 236;
        return new Vector2(x, y);
    }



    public void onClick( GameObject obj )
    {
        touchId = int.Parse( obj.name );
    }
}
