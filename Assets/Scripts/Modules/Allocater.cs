
using System.Collections.Generic;

public class Allocater
{
    private int maxCnt = 0;
    private Queue<int> idPool = new Queue<int>();
    public int GetID()
    {
        if (idPool.Count == 0)
            return maxCnt++;
        else
            return idPool.Dequeue();
    }
    public void PushID(int id)
    {
        idPool.Enqueue(id);
    }
}
