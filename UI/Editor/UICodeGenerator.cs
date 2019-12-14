
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class UICodeGenerator
{
    [MenuItem("Assets/生成UI代码")]
    public static void CreateUICode()
    {
        //mScriptKitInfo = null;
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

    private static void CreateCode(GameObject obj, string uiPrefabPath)
    {
        if (obj != null)
        {
            //PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(obj);
            //if (status == PrefabInstanceStatus.NotAPrefab)
            //{
            //    return;
            //}
            string name = obj.name;
            var clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            if (null == clone)
            {
                return;
            }

            Debug.LogError(Application.dataPath);

            string writePath = Application.dataPath + "/" + name + ".Reference.cs";


            UIMark[] marks = clone.transform.GetComponentsInChildren<UIMark>();
            string referenceStr = "";

            for (int i = 0; i < marks.Length; i++)
            {
                referenceStr += "public GameObject " + marks[i].name + ";\n";
                
            }

            string template = File.ReadAllText(Application.dataPath + "/Framework/UI/ConfigData/" + "UICodeTemplate.txt");
            template = template.Replace("[ClassName]", name);
            template = template.Replace("[Reference]", referenceStr);

            File.WriteAllText(writePath, template, Encoding.UTF8);
            Object.DestroyImmediate(clone);
        }




    }
}