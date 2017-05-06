using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum _ST
{
    _charaName = 0,
    _faceGraphicPath = 1,
    _standGraphicPath = 2,
    _maxHp = 3,
    _atk = 4,
    _mag = 5,
    _knockback = 6,
    _resistKnockback = 7,
    _waitActionBase = 8,
    _magWaitBase = 9,
    _element = 10,
    _description = 11,
};

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
    public string standGraphicPath; // 立ち絵へのファイルパス
    public string description; // 説明文

    public void LoadCharacterData(string FilePath, int characterId)
    {
        if( characterId == 0)
        {
            Debug.LogError(" ※ characterID(0) が指定されました");
            return;
        }

        // 設定ファイルを読込
        string[] buffer;

        buffer = System.IO.File.ReadAllLines(FilePath, System.Text.Encoding.GetEncoding("shift_jis"));


        // linebuffer にキャラクターの情報( characterId 番目の )を格納
        string[] linebuffer;
        linebuffer = buffer[characterId].Split(',');

        // ステータス読込
        charaName = linebuffer[(int)_ST._charaName]; // 名前
        faceGraphicPath = linebuffer[(int)_ST._faceGraphicPath]; // 顔グラ画像パス
        standGraphicPath = linebuffer[(int)_ST._standGraphicPath]; // 立ち絵画像パス
        maxHp = int.Parse(linebuffer[(int)_ST._maxHp]); // 体力
        atk = int.Parse(linebuffer[(int)_ST._atk]); // 攻撃力
        mag = int.Parse(linebuffer[(int)_ST._mag]); // 魔力値
        knockback = int.Parse(linebuffer[(int)_ST._knockback]); ; // 吹き飛ばし力
        resistKnockback = int.Parse(linebuffer[(int)_ST._resistKnockback]); // 吹き飛び耐性
        waitActionBase = int.Parse(linebuffer[(int)_ST._waitActionBase]); ; // 行動後の待機時間
        magWaitBase = int.Parse(linebuffer[(int)_ST._magWaitBase]); ; // 詠唱後の待機時間
        element = int.Parse(linebuffer[(int)_ST._element]); ; // 属性
        description = linebuffer[(int)_ST._description]; // 説明文
    }

    // 指定されたパラメータの取得    
    public static string LoadPlayerCharacterStringStatus(int charaId, _ST st)
    {
        Debug.LogWarning("LoadPlayerCharacterStringStatus()はデバッグ関数で非推奨です(I/Oが絡むため極めて重たいです)");


        string FilePath = Variables.Unit.PlayerDataFilePath;
        // 設定ファイルを読込
        string[] buffer;
        buffer = System.IO.File.ReadAllLines(FilePath);//, System.Text.Encoding.GetEncoding("shift_jis"));
        

        // linebuffer にキャラクターの情報( characterId 番目の )を格納
        string[] linebuffer;
        linebuffer = buffer[charaId].Split(',');
        return linebuffer[(int)st];
    }

    // 値をコピー(Unityでは、コピーコンストラクタは使えない)
    public void CopyCharacterStatus( CharacterStatus cs)
    {
        charaName = cs.charaName;
        faceGraphicPath = cs.faceGraphicPath;
        standGraphicPath = cs.standGraphicPath;
        maxHp = cs.maxHp;
        atk = cs.atk;
        mag = cs.mag;
        knockback = cs.knockback;
        resistKnockback = cs.resistKnockback;
        waitActionBase = cs.waitActionBase;
        magWaitBase = cs.magWaitBase;
        element = cs.element;
        description = cs.description;
    }
}

