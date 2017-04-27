using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantValue : MonoBehaviour {
    // 定数設定
    public const int BATTLE_TIME_PER_ONECTB = 6; // 1マス当たりの移動にかかるフレーム   k
    public const int BATTLE_FACE_SIZE = 72; // 顔グラのサイズ設定   k
    public const int BATTLE_FACE_VY = 45; // 顔グラフィックの縦方向のずらし
    public const int BATTLE_ENEMYFACE_OFFSETY = -50; // 敵の CTB 顔グラのY座標をどの程度ずらすか  k
    public const int BATTLE_PLAYERFACE_OFFSETY = 50; // 味方の CTB 顔グラのY座標をどの程度ずらすか  k
    public const int BATTLE_FACE_OFFSETX = -504; // CTB顔グラのX座標補正値 k

    public const int BATTLE_STATUS_OFFSETX = -530; // ステータス表示のX座標補正値 k
    public const int BATTLE_STATUS_VX = 210; // ステータス表示の1キャラあたりの幅 k
    public const int BATTLE_STATUS_PLAYERY = 310; // プレイヤーステータスのＹ座標 k
    public const int BATTLE_STATUS_ENEMYY = -310; // 敵ステータスのＹ座標 k

    public const int BATTLE_ATTACKFACE_SIZE = 432; // 攻撃演出用(仮置)
    

    // グローバル変数
    public static int enemyNum;
    public static int playerNum;
}
