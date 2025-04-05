using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class V_F
{
    public AnimationCurve c;//曲线
    //0~v.x为速度向左时的速度缩放，0~v.y为速度向右时的速度缩放
    //0~f.x为速度向左时的受力缩放，0~f.y为速度向右时的受力缩放
    public Vector2 v;
    public Vector2 f;
    public float Val(float velocity)
    {
        if (velocity < 0)
        {
            return c.Evaluate(velocity / Mathf.Abs(v.x)) * Mathf.Abs(f.x);
        }
        else
        {
            return c.Evaluate(velocity / Mathf.Abs(v.y)) * Mathf.Abs(f.y);
        }
    }
    public V_F()
    {
        c = AnimationCurve.EaseInOut(-1, 1, 1, -1);
        v = new Vector2(-5, 5);
        f = new Vector2(-10, 10);
    }
    public V_F(Vector2 scale_v, Vector2 scale_f)
    {
        c = AnimationCurve.EaseInOut(-1, 1, 1, -1);
        v = scale_v;
        f = scale_f;
    }
}
