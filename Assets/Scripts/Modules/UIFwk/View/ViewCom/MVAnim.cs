using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class MVAnim : MView<string>
{
    private STComp<Animator> animator = new();
    public override void SetData(string animName)
    {
        animator.Get(this).Play(animName);
    }
}