﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Battle Constant Value Class
 *  ┗ バトルで用いる定数を設定するクラス
 */
public class BCV
{
    // CTB 関連
    public const int VX_PER_CTBNUM = 48; // CTB1数値当たりずらすX座標
    public const int FRAME_PER_CTB = 1; // CTB1数値あたりのウェイト
    public const int FACE_SIZE = 72; // 顔グラのサイズ設定
    public const int CTB_LEFTEND_POS = -504; // CTB予測の左端
    public const int CTB_ENEMY_UPPER = -50; // 敵の CTB 顔グラのY座標をどの程度ずらすか
    public const int CTB_PLAYER_BOTTOM = 50; // 味方の CTB 顔グラのY座標をどの程度ずらすか
    public const int CTB_FACE_VY = 45; // CTB 顔グラの縦方向の変位値

    public const int PLAYER_MAX = 5; // (6以上にするのは現実的じゃない)

    public const string FACE_IMAGE_PREFAB = "Prefabs\\Battle\\ImageBase"; // 顔グラフィックのプレハブ
    public const string MAG_TEXT_PREFAB = "Prefabs\\Battle\\MagText"; // 魔力テキストのプレハブ
    public const string STATUSOBJ_PREFAB = "Prefabs\\Battle\\Status"; // ステータスオブジェクトのプレハブ
    public const string RESULT_PREFAB = "Prefabs\\Battle\\ResultObject"; // リザルトオブジェクト
    public const string RESULT_LOSE_PREFAB = "Prefabs\\Battle\\ResultLoseObject"; // リザルトオブジェクト(敗北時)

    // ステータス表示関連
    public const int STATUS_PLAYER_Y = 310;
    public const int STATUS_ENEMY_Y = -310;
    public const int STATUS_LEFT_END = -530;
    public const int STATUS_VX = 210;
}


/*
 * Battle Global Value Class
 *  ┗ バトルで用いる定数を設定するクラス
 */
public class BGV
{
    // TODO: ステータスオブジェクトの表示方法
    public static int statusObjPosY;
}

