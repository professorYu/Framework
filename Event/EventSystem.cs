
using System;
using System.Collections.Generic;

#region 事件接口

public delegate void EventDelegate(params object[] param);

#endregion

public class EventSystem : Singleton<EventSystem>
{
    private readonly Dictionary<int, EventDispatcher> _allListenerMap = new Dictionary<int, EventDispatcher>(50);

    #region 内部结构

    private class EventDispatcher
    {
        private LinkedList<EventDelegate> _eventList;

        public bool Send(params object[] param)
        {
            if (_eventList == null)
            {
                return false;
            }

            var next = _eventList.First;

            while (next != null)
            {
                next.Value(param);
                next = next.Next;
            }

            return true;
        }

        public bool Add(EventDelegate listener)
        {
            if (_eventList == null)
            {
                _eventList = new LinkedList<EventDelegate>();
            }

            if (_eventList.Contains(listener))
            {
                return false;
            }

            _eventList.AddLast(listener);
            return true;
        }

        public void Remove(EventDelegate listener)
        {
            _eventList?.Remove(listener);
        }

        public void RemoveAll()
        {
            _eventList?.Clear();
        }
    }

    #endregion

    #region 功能函数

    public bool Register<T>(T key, EventDelegate fun) where T : IConvertible
    {
        int keyValue = key.ToInt32(null);

        EventDispatcher dispatcher;
        if (!_allListenerMap.TryGetValue(keyValue, out dispatcher))
        {
            dispatcher = new EventDispatcher();
            _allListenerMap.Add(keyValue, dispatcher);
        }

        if (dispatcher.Add(fun))
        {
            return true;
        }

        return false;
    }

    public void UnRegister<T>(T key, EventDelegate fun) where T : IConvertible
    {
        int keyValue = key.ToInt32(null);
        if (_allListenerMap.TryGetValue(keyValue, out var dispatcher))
        {
            dispatcher.Remove(fun);
        }
    }

    public void UnRegister<T>(T key) where T : IConvertible
    {
        int keyValue = key.ToInt32(null);
        if (_allListenerMap.TryGetValue(keyValue, out var dispatcher))
        {
            dispatcher.RemoveAll();
            _allListenerMap.Remove(keyValue);
        }
    }

    public bool Send<T>(T key, params object[] param) where T : IConvertible
    {
        int keyValue = key.ToInt32(null);
        if (_allListenerMap.TryGetValue(keyValue, out var dispatcher))
        {
            return dispatcher.Send(param);
        }
        return false;
    }

    public void ClearAll()
    {
        _allListenerMap.Clear();
    }
    #endregion


}
