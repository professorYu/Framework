using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum UILayer
{
    Bg,   //背景层     一般是底层常驻UI  例如 : 主界面的常驻UI
    Common, //通用层    普通UI层
    Pop,  //弹出层     常规弹窗  比一般UI高
    Tips,  //提示层    提示飘字  一般覆盖普通弹窗
    Top,  //最顶层    一般是重要必须显示在最上面的  例如 : 退出游戏弹窗
}

public class PanelData
{
    public UIBasePanel panel;
    public string name;
    public UILayer layer;
    public bool IgnoreClose;
    public AsyncOperationHandle handle;
}

/// <summary>
/// UI管理类
/// </summary>
[MonoSingletonPath("[Framework]/UIManager")]
public class UIManager : MonoSingleton<UIManager>
{
    //缓存UI父节点  分层
    private Dictionary<UILayer, RectTransform> _allPanelParentDict = new Dictionary<UILayer, RectTransform>();

    //缓存打开的UI 
    private Dictionary<string, PanelData> _allPanelData = new Dictionary<string, PanelData>();

    private void Awake()
    {
        //加载UI
        string uiRootPath = "UIRoot";
        GameObject root = Instantiate(Resources.Load<GameObject>(uiRootPath));
        root.name = uiRootPath;
        DontDestroyOnLoad(root);

        //创建分层UI
        foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
        {
            string strName = layer.ToString();
            GameObject child = new GameObject(strName);
            RectTransform rect = child.AddComponent<RectTransform>();
            rect.anchorMax = Vector2.one;
            rect.anchorMin = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            child.transform.SetParent(root.transform, false);

            if (!_allPanelParentDict.ContainsKey(layer))
            {
                _allPanelParentDict.Add(layer, rect);
            }
        }
    }

    private List<string> _ignoreClosePanelList = new List<string>();

    //设置常驻的界面
    public void SetIgnoreClose(params string[] panels) //where T : UIBasePanel
    {
        foreach (var panelName in panels)
        {
            _ignoreClosePanelList.Add(panelName);
        }
    }

    public PanelData GetPanel<T>() where T : UIBasePanel
    {
        string panelName = typeof(T).ToString();
        _allPanelData.TryGetValue(panelName, out var panelData);
        return panelData;
    }

    public PanelData Open<T>(UILayer layer = UILayer.Common, Action<T> callback = null) where T : UIBasePanel
    {
        string panelName = typeof(T).ToString();

        if (_allPanelData.TryGetValue(panelName, out var data))
        {
            if (data.handle.IsDone)
            {
                data.panel.gameObject.SetActive(true);
                data.panel.transform.SetAsLastSibling();
                callback?.Invoke((T) data.panel);
            }
            return data;
        }

        PanelData panelData = new PanelData();
        _allPanelData.Add(panelName, panelData);

        panelData.layer = layer;
        panelData.name = panelName;

        foreach (var ignoreName in _ignoreClosePanelList)
        {
            if (panelName == ignoreName)
            {
                panelData.IgnoreClose = true;
                break;
            }
        }

        panelData.handle = ResManager.Instance.Instantiate(panelName, panelGo =>
        {
            panelGo.name = panelName;
            panelGo.transform.SetParent(_allPanelParentDict[layer], false);
            T uiPanel = panelGo.GetComponent<T>();
            panelData.panel = uiPanel;
            callback?.Invoke(uiPanel);
        });

        return panelData;
    }

    public void Close<T>() where T : UIBasePanel
    {
        string panelName = typeof(T).ToString();
        Close(panelName);
    }

    public void Close(string panelName)
    {
        if (_allPanelData.TryGetValue(panelName, out var panelData))
        {
            Addressables.ReleaseInstance(panelData.handle);
            _allPanelData.Remove(panelName);
        }
    }

    LinkedList<string> _closeList = new LinkedList<string>();

    public void CloseAll()
    {
        _closeList.Clear();

        foreach (var panelData in _allPanelData)
        {
            if (!panelData.Value.IgnoreClose)
            {
                _closeList.AddLast(panelData.Value.name);
            }
        }

        foreach (string panelName in _closeList)
        {
            Close(panelName);
        }

        _closeList.Clear();
    }

    public void CloseByUILevel(UILayer layer)
    {
        _closeList.Clear();

        foreach (var panelData in _allPanelData)
        {
            if (panelData.Value.layer == layer && !panelData.Value.IgnoreClose)
            {
                _closeList.AddLast(panelData.Value.name);
            }
        }

        foreach (string panelName in _closeList)
        {
            Close(panelName);
        }

        _closeList.Clear();
    }
}
