using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "new Parameters", fileName = "Params prefab")]
public class PlayerMoveArgs : ScriptableObject
{
    #region ========================================BGForce=========================================
    public V_F OnGround_config_x;
    public V_F InAir_config_x;
    public V_F InAir_config_y;
    public float gravityScale;
    #endregion

    #region ======================================Power=============================================
    public Vector2 OnGround_pw_x = new Vector2(-105, 105);
    public Vector2 InAir_pw_x = new Vector2(-105, 105);
    public AnimationCurve jumpCurve = AnimationCurve.Linear(0, 1, 1, 0);
    public float jumpTime = 0;
    public float jumpV = 20;

    #endregion
}
