using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantValue : MonoBehaviour {
    // 定数設定
    public const int BATTLE_PLAYER_NUM = 4; // バトル人数(味方)
    public const int BATTLE_FACE_SIZE = 72; // 顔グラのサイズ設定
    public const int BATTLE_FACE_VY = 48; // 顔グラフィックの縦方向のずらし
    public const int BATTLE_TIME_PER_ONECTB = 6; // 1マス当たりの移動にかかるフレーム

    public const int BATTLE_ENEMYFACE_OFFSETY = -70; // 敵の CTB 顔グラのY座標をどの程度ずらすか
    public const int BATTLE_PLAYERFACE_OFFSETY = 20; // 味方の CTB 顔グラのY座標をどの程度ずらすか
    public const int BATTLE_FACE_OFFSETX = -504; // CTB顔グラのX座標補正値

    public const int BATTLE_ATTACKFACE_SIZE = 432; // 攻撃演出用(仮置)
    

    // グローバル変数
    public static int enemyNum;
    public static int playerNum;
}
