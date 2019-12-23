using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[MonoSingletonPath("[Framework]/PoolManager")]
public class PoolManager : MonoSingleton<PoolManager>
{
    public Dictionary<int, ObjectPool> Pools
    {
        get { return _pools; }
    }

    private readonly Dictionary<int, ObjectPool> _pools = new Dictionary<int, ObjectPool>();

    public Object Get(IConvertible key)
    {
        int keyValue = key.ToInt32(null);
        if (_pools.TryGetValue(keyValue, out ObjectPool pool))
        {
            return pool.Get();
        }

        return default;
    }

    public void Put(IConvertible key, Object item)
    {
        int keyValue = key.ToInt32(null);
        if (_pools.TryGetValue(keyValue, out var pool))
        {
            pool.Put(item);

            //放入manager下  方便管理查看
            if (item is Component comp)
            {
                comp.gameObject.SetActive(false);
                comp.transform.SetParent(transform, false);
            }
            else if (item is GameObject go)
            {
                go.SetActive(false);
                go.transform.SetParent(transform, false);
            }
        }
    }

    public void RegisterPool(IConvertible key, Func<Object> createFunc, int initCount = 0)
    {
        int keyValue = key.ToInt32(null);
        _pools.Add(keyValue, new ObjectPool(createFunc, initCount));
    }

    public void UnRegisterPool(IConvertible key)
    {
        int keyValue = key.ToInt32(null);
        if (_pools.TryGetValue(keyValue, out var pool))
        {
            pool.Clear();
            _pools.Remove(keyValue);
        }

    }
}
