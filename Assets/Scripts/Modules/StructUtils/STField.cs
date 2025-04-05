using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 单例字段
/// </summary>
/// <typeparam name="T typeof this"></typeparam>
/// <typeparam name="K typeof field"></typeparam>
public class STField<T, K> where K : class
{
    Func<T, K> _get;
    K ins = null;
    public STField(Func<T, K> get)
    {
        this._get = get;
    }
    public virtual K Get(T key)
    {
        ins ??= _get(key);
        return ins;
    }
}
public class STComp<K> : STField<Component, K> where K : Component
{
    public STComp() : base((g) => g.GetComponent<K>())
    { }
}
public class STInfo<T> : STField<T, T> where T : class
{

    public STInfo(Func<T> get) : base(_ => get())
    {
    }
    public static implicit operator T(STInfo<T> info)
    {
        return info.Get(null);
    }
    public T As()
    {
        return Get(null);
    }
}
public class STMap<K, V> : STField<K, V> where V : class
{
    private Dictionary<K, STInfo<V>> mapDic = new();
    private Func<K, V> mapFunc;
    public STMap(Func<K, V> mapFunc) : base(null)
    {
        this.mapFunc = mapFunc;
        mapDic = new();
    }
    public override V Get(K key)
    {
        if (!mapDic.ContainsKey(key))
        {
            mapDic.Add(key, new STInfo<V>(() => mapFunc(key)));
        }
        return mapDic[key].As();
    }
}