//Command Trigger
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CT : Singleton<CT>
{
    private List<(Func<bool>, Action)> conditionCmds = new();
    private List<List<Action>> cmdQueue = new();
    public static void DelayCmd(Action cmd, int delayIdx = 0)
    {
        while (Ins.cmdQueue.Count <= delayIdx)
            Ins.cmdQueue.Add(new());
        Ins.cmdQueue[delayIdx].Add(cmd);
    }
    public static void SuspendCmd(Action cmd, Func<bool> condition)
    {
        Ins.conditionCmds.Add((condition, cmd));
    }
    // public static void SuspendCmd(Action cmd,string evtKey){
    //     Debug.Log("suspend");
    //     int hash = evtKey.GetHashCode();
    //     Debug.Log(hash);
    //     System.Object empty = new System.Object();
    //     // onceCmdsAgent.Add(hash,empty);
    //     bool flag = true;
    //     empty.On(evtKey, () =>
    //     {
    //         if(!flag)
    //             return;
    //         flag = false;
    //         cmd.Invoke();
    //         DelayCmd(()=>empty.Off(evtKey)) ;
    //         // onceCmdsAgent.Remove(hash);
    //     });
    // }
    public static void OnUpdate()
    {
        List<(Func<bool>, Action)> removeCmds = new List<(Func<bool>, Action)>();
        Ins.conditionCmds.ForEach(e =>
        {
            if (e.Item1.Invoke())
            {
                e.Item2.Invoke();
                removeCmds.Add(e);
            }
        });
        if (Ins.cmdQueue.Count > 0)
        {
            var func = Ins.cmdQueue[0];
            func.ForEach(e => e.Invoke());
            Ins.cmdQueue.RemoveAt(0);
        }
        removeCmds.ForEach(e => Ins.conditionCmds.Remove(e));
    }
}
