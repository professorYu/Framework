using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UIBasePanel : MonoBehaviour
{
    private Dictionary<IConvertible, EventDelegate> _saveEvent = new Dictionary<IConvertible, EventDelegate>();
    protected void CloseSelf()
    {
        UIManager.Instance.Close(name);
    }

    //添加监听  不需要手动移除   
    protected void AddListener(IConvertible key, EventDelegate fun)
    {
        _saveEvent.Add(key, fun);
        EventManager.Instance.Register(key, fun);
    }

    protected void AutoRemoveListener()
    {
        foreach(var kv in _saveEvent)
        {
            EventManager.Instance.UnRegister(kv.Key, kv.Value);
        }

        _saveEvent.Clear();
    }

    protected void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        AutoRemoveListener();
    }


}
