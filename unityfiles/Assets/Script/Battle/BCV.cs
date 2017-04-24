using System;
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
    public const int FRAME_PER_CTB = 5; // CTB1数値あたりのウェイト

    // ステータス表示関連
    public const int STATUS_PLAYER_Y = 310;
    public const int STATUS_ENEMY_Y = -310;
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

