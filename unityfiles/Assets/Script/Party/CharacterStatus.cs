using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CharacterStatus
{
    /* キャラクターのステータス情報 */
    // (エクセルから取得．レベルによる補正をかける)
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
}

