// view model
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class VMAdapter<T, K>
{
    private Func<T, K> core = null;
    public VMAdapter(Func<T, K> func)
    {
        core = func;
    }
    public K Convert(T data)
    {
        return core(data);
    }
}
public class EventGroup<T>
{
    public event Action OnChange;
    public event Action<T> OnSet;
    public event Action<T, T> OnAnalyse;
    public void Invoke(T old, T now, T cur)
    {
        OnChange.Invoke();
        OnSet.Invoke(cur);
        OnAnalyse.Invoke(old, now);
    }
    public EventGroup()
    {
        OnChange = () => { };
        OnSet = _ => { };
        OnAnalyse = (_, _) => { };
    }
}
public class VMBase<T>
{
    public K AdaptFrom<K>(VMAdapter<T, K> adapter)
    {
        return adapter.Convert(_data);
    }
    protected T _data = default;
    protected EventGroup<T> eventGrp = new();
    public void On(Action listener)
    {
        eventGrp.OnChange += listener;
    }
    public void On(Action<T> listener)
    {
        eventGrp.OnSet += listener;
    }
    public void InitRender(Action<T> render)
    {
        render(_data);
    }
    public void On(Action<T, T> listener)
    {
        eventGrp.OnAnalyse += listener;
    }

    public void Off(Action listener)
    {
        eventGrp.OnChange -= listener;
    }
    public void Off(Action<T> listener)
    {
        eventGrp.OnSet -= listener;
    }
    public void Off(Action<T, T> listener)
    {
        eventGrp.OnAnalyse -= listener;
    }

}
/// <summary>
/// 当数据发生变化时，触发event.Invoke(旧值，新值)
/// </summary>
/// <typeparam name="T"></typeparam>
public class VM<T> : VMBase<T>
{
    public T D
    {
        get
        {
            return _data;
        }
        set
        {
            T _old = _data;
            T _new = value;
            _data = value;
            if (!_old.Equals(_new))
                eventGrp.Invoke(_old, _new, _data);
        }
    }
    public static implicit operator VM<T>(T d)
    {
        VM<T> vm = new VM<T>();
        vm._data = d;
        return vm;
    }

}
/// <summary>
/// 当列表内容变化时，触发event.Invoke(舍弃列表，新增列表)
/// </summary>
/// <typeparam name="T"></typeparam>
public class VMList<T> : VMBase<List<T>>
{
    public VMList() : base()
    {
        this._data = new List<T>();
    }
    public List<T> list => new List<T>(_data);
    public int Count => _data.Count;
    public T Find(Predicate<T> cond)
    {
        return _data.Find(cond);
    }
    public int FindIndex(Predicate<T> cond)
    {
        return _data.FindIndex(cond);
    }
    public void Set(List<T> data)
    {
        (List<T> enter, List<T> exit) = MUtils.Analyse(_data, data);
        _data = data;
        eventGrp.Invoke(enter, exit, _data);
    }
    public void Set(T data, int idx)
    {
        if (idx < 0 || idx >= _data.Count)
            return;
        T oldV = _data[idx];
        T newV = data;
        _data[idx] = data;
        if (!oldV.Equals(newV))
            eventGrp.Invoke(new List<T>() { oldV }, new List<T>() { newV }, _data);
    }
    public void Add(params T[] data)
    {
        _data.AddRange(data);
        eventGrp.Invoke(new List<T>(data), new List<T>(), _data);
    }
    public void Add(List<T> data)
    {
        _data.AddRange(data);
        eventGrp.Invoke(data, new List<T>(), _data);
    }
    public void Remove(params T[] data)
    {
        this.Remove(data.ToList());
    }
    public void RemoveAt(int idx)
    {
        _data.RemoveAt(idx);
    }
    public void Remove(List<T> data)
    {
        List<T> removes = new List<T>();
        for (int i = 0; i < data.Count; i++)
        {
            if (_data.Contains(data[i]))
            {
                removes.Add(data[i]);
                _data.Remove(data[i]);
                i--;
            }
        }
        if (removes.Count > 0)
            eventGrp.Invoke(new List<T>(), removes, _data);
    }
    public void Clear()
    {
        if (_data.Count > 0)
        {
            List<T> clears = new List<T>(_data);
            _data.Clear();
            eventGrp.Invoke(new List<T>(), clears, _data);
        }
    }
    public static implicit operator VMList<T>(List<T> d)
    {
        VMList<T> vm = new VMList<T>();
        vm._data = d;
        return vm;
    }
    public T this[int idx] => _data[idx];
}
/// <summary>
/// 当字典内容变化时，触发event.Invoke(旧键值对，新键值对)
/// 若仅舍弃，则新键值对为null，若仅新增，则旧键值对为null
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="V"></typeparam>
public class VMMap<K, V> : VMBase<Dictionary<K, V>>
{
    public VMMap() : base()
    {
        this._data = new Dictionary<K, V>();
    }
    public List<K> keys => new List<K>(_data.Keys);
    public List<V> values => new List<V>(_data.Values);
    public List<KeyValuePair<K, V>> pairs => _data.ToList();
    public Dictionary<K, List<Action<V>>> events = new();
    public bool ContainsKey(K key)
    {
        return _data.ContainsKey(key);
    }
    public void On(K key, Action<V> func)
    {
        if (!events.ContainsKey(key))
        {
            events.Add(key, new List<Action<V>>());
        }
        events[key].Add(func);
    }
    public void Off(K key, Action<V> func)
    {
        if (events.ContainsKey(key))
        {
            events[key].Remove(func);
            if (events[key].Count == 0)
                events.Remove(key);
        }
    }
    public bool ContainsValue(V value)
    {
        return _data.ContainsValue(value);
    }
    public void Set(K key, V value)
    {
        Dictionary<K, V> _old = new Dictionary<K, V>(_data);
        if (_data.ContainsKey(key))
        {
            V oldValue = _data[key];
            _data[key] = value;
            if (!oldValue.Equals(value))
            {
                eventGrp.Invoke(_old, _data, _data);
                if (events.ContainsKey(key))
                    events[key].ForEach(e => e.Invoke(value));
            }
        }
        else
        {
            KeyValuePair<K, V> _new = new KeyValuePair<K, V>(key, value);
            _data.Add(key, value);
            eventGrp.Invoke(_old, _data, _data);
            if (events.ContainsKey(key))
                events[key].ForEach(e => e.Invoke(value));
        }
    }
    public void Remove(K key)
    {
        if (_data.ContainsKey(key))
        {
            Dictionary<K, V> _old = new(_data);
            _data.Remove(key);
            eventGrp.Invoke(_old, _data, _data);
        }
    }
    public V Get(K key)
    {

        if (_data.ContainsKey(key))
        {
            return _data[key];
        }
        else
        {
            return default;
        }
    }
    public static implicit operator VMMap<K, V>(Dictionary<K, V> dic)
    {
        VMMap<K, V> vm = new VMMap<K, V>();
        vm._data = dic;
        return vm;
    }
    public V this[K key] => _data[key];

}
