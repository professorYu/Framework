
using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

public class UICodeGenerator
{
    private static string UIMainTemplatePath = Application.dataPath + "/Framework/UI/ConfigData/UIMainTemplate.txt";
    private static string UIReferenceTemplatePath = Application.dataPath + "/Framework/UI/ConfigData/UIReferenceTemplate.txt";
    private static string UIScriptSaveFolider = Application.dataPath + "/";

    [MenuItem("Assets/生成UI代码")]
    public static void CreateUICode()
    {
        var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets | SelectionMode.TopLevel);
        EditorUtility.DisplayProgressBar("", "生成UI代码中...", 0);
        for (var i = 0; i < objs.Length; i++)
        {
            CreateCode(objs[i] as GameObject, AssetDatabase.GetAssetPath(objs[i]));
            EditorUtility.DisplayProgressBar("", "生成UI代码中...", (float)(i + 1) / objs.Length);
        }
        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
    }

    //写引用文件
    private static void WriteReferenceCode(GameObject clone, string componentName)
    {
        UIMark[] marks = clone.transform.GetComponentsInChildren<UIMark>();
        string referenceStr = "";

        for (int i = 0; i < marks.Length; i++)
        {
            referenceStr += "public GameObject " + marks[i].name + ";\n\t";
        }

        string template = File.ReadAllText(UIReferenceTemplatePath);
        template = template.Replace("[ClassName]", componentName);
        template = template.Replace("[Reference]", referenceStr);

        File.WriteAllText(UIScriptSaveFolider + componentName + ".Reference.cs", template, Encoding.UTF8);
    }

    //写主文件
    private static void WriteMainCode(string componentName)
    {
        string mainCodePath = UIScriptSaveFolider + componentName + ".cs";
        if (!File.Exists(mainCodePath))
        {
            string template = File.ReadAllText(UIMainTemplatePath);
            template = template.Replace("[ClassName]", componentName);
            File.WriteAllText(UIScriptSaveFolider + componentName + ".cs", template, Encoding.UTF8);
        }
    }

    

    private static void CreateCode(GameObject obj, string uiPrefabPath)
    {
        if (obj == null) return;

        PlayerPrefs.SetString("_LOCAL_TEST", uiPrefabPath);
        string componentName = obj.name;
        //var clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;

        WriteReferenceCode(obj, componentName);
        WriteMainCode(componentName);

        //Object.DestroyImmediate(clone);
    }

    [DidReloadScripts]
    static void AddComponent2GameObject()
    {
        return;
        Debug.LogError("编译完成");

        string uiPrefabPath = PlayerPrefs.GetString("_LOCAL_TEST");
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(uiPrefabPath);
        var clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;

        string componentName = obj.name;
        Component component = clone.GetComponent(componentName);

        if (component == null)
        {
            Assembly assembly = AppDomain.CurrentDomain.Load("Assembly-CSharp");
            Type type = assembly.GetType(componentName);
            if (type == null)
            {
                string template = File.ReadAllText(UIMainTemplatePath);
                template = template.Replace("[ClassName]", componentName);
                File.WriteAllText(UIScriptSaveFolider + componentName + ".cs", template, Encoding.UTF8);
                AssetDatabase.Refresh();
                type = assembly.GetType(componentName);
            }

            component = clone.AddComponent(type);
        }

        FieldInfo[] fileFieldInfos = component.GetType().GetFields();
        UIMark[] marks = clone.transform.GetComponentsInChildren<UIMark>();
        foreach (FieldInfo fileFieldInfo in fileFieldInfos)
        {
            for (int i = 0; i < marks.Length; i++)
            {
                if (marks[i].name == fileFieldInfo.Name)
                {
                    fileFieldInfo.SetValue(component, marks[i].gameObject);
                    break;
                }
            }
        }
        PrefabUtility.SaveAsPrefabAssetAndConnect(clone, uiPrefabPath, InteractionMode.AutomatedAction);
        Object.DestroyImmediate(clone);
        //return component;

    }
}