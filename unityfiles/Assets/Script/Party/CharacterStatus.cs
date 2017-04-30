using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* =======================================================
 * キャラクターのステータス情報を管理するクラス
 *  ( 戦闘中には変化しない )
 * =======================================================*/
public class CharacterStatus : MonoBehaviour
{
    /* キャラクターのベーシックなステータス */
    public string charaName; // キャラの名前
    public int maxHp; // 最大体力( 雑魚戦用 )
    public int atk; // 攻撃力
    public int mag; // 魔力値
    public int knockback; // 吹き飛ばし力
    public int resistKnockback; // 吹き飛ばし耐性
    public int magWaitBase; // 詠唱待機時間のベース値
    public int waitActionBase; // 待機時間のベース値
    public int element; // キャラクターの属性
    public string faceGraphicPath; // 顔グラのファイルパス

    public void LoadCharacterData(string FilePath, int characterId)
    {
        if( characterId == 0)
        {
            Debug.LogError(" ※ characterID(0) が指定されました");
            return;
        }

        // 設定ファイルを読込
        string[] buffer;
        buffer = System.IO.File.ReadAllLines(FilePath);

        // linebuffer にキャラクターの情報( characterId 番目の )を格納
        string[] linebuffer;
        linebuffer = buffer[characterId].Split(',');

        // ステータス読込
        charaName = linebuffer[0]; // 名前
        faceGraphicPath = linebuffer[1]; // 画像パス
        maxHp = int.Parse(linebuffer[2]); // 体力
        atk = int.Parse(linebuffer[3]); // 攻撃力
        mag = int.Parse(linebuffer[4]); // 魔力値
        knockback = int.Parse(linebuffer[5]); ; // 吹き飛ばし力
        resistKnockback = int.Parse(linebuffer[6]); // 吹き飛び耐性
        waitActionBase = int.Parse(linebuffer[7]); ; // 行動後の待機時間
        magWaitBase = int.Parse(linebuffer[8]); ; // 詠唱後の待機時間
        element = int.Parse(linebuffer[9]); ; // 属性
    }


    // 値をコピー(Unityでは、コピーコンストラクタは使えない)
    public void CopyCharacterStatus( CharacterStatus cs)
    {
        charaName = cs.charaName;
        faceGraphicPath = cs.faceGraphicPath;
        maxHp = cs.maxHp;
        atk = cs.atk;
        mag = cs.mag;
        knockback = cs.knockback;
        resistKnockback = cs.resistKnockback;
        waitActionBase = cs.waitActionBase;
        magWaitBase = cs.magWaitBase;
        element = cs.element;
    }
}

