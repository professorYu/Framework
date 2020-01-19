using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectPool
{
    public Stack<Object> ObjStack
    {
        get { return _objStack; }
    }

    private readonly Stack<Object> _objStack = new Stack<Object>();
    private readonly Func<Object> _createFunc;

    public ObjectPool(Func<Object> createFunc)
    {
        _createFunc = createFunc;
    }

    public void Put(Object item)
    {
        _objStack.Push(item);
    }

    public Object Get()
    {
        if (_objStack.Count == 0)
        {
            return _createFunc();
        }
        else
        {
            return _objStack.Pop();
        }
    }

    public void Clear()
    {
        while (_objStack.Count > 0)
        {
            Object.Destroy(_objStack.Pop());
        }
    }
}
