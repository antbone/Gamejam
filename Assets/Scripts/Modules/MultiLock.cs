using System;
using System.Collections.Generic;

public class MutiLock
{
    Action lockFunc = () => { };
    Action unlockFunc = () => { };
    Action timeoutFunc = () => { };
    private HashSet<string> lockKeys = new HashSet<string>();
    private bool _isLock = false;
    private string timerKey = null;
    public bool isLock => _isLock;
    public float timeoutTime = 10;
    public MutiLock(Action lockFunc = null, Action unlockFunc = null, Action timeoutFunc = null)
    {
        this.lockFunc = lockFunc;
        this.unlockFunc = unlockFunc;
        this.timeoutFunc = timeoutFunc;
    }
    public void LockFrom(string key)
    {
        lockKeys.Add(key);
        Refresh();
        RestartTimeout();
    }
    public void UnlockFrom(string key)
    {
        lockKeys.Remove(key);
        Refresh();
        RestartTimeout();
    }
    private void BeTimeout()
    {
        TM.SetEnd(timerKey, false);
        timeoutFunc();
        lockKeys.Clear();
        unlockFunc();
        _isLock = false;
    }
    private void RestartTimeout()
    {
        TM.SetEnd(timerKey, false);
        if (lockKeys.Count > 0)
        {
            TM.SetTimer(timerKey, timeoutTime, null, s => BeTimeout());
        }
    }
    private void Refresh()
    {
        if (lockKeys.Count > 0)
        {
            if (!_isLock)
            {
                lockFunc();
            }
            _isLock = true;
        }
        else
        {
            if (_isLock)
            {
                unlockFunc();
            }
            _isLock = false;
        }
    }
}