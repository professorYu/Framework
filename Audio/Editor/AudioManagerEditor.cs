using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private bool _isShowData = true;
    private string _nullReferenceName = "空引用";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (_isShowData == false)
        {
            if (GUILayout.Button("显示可视化管理"))
            {
                _isShowData = true;
            }
        }
        
        if (_isShowData)
        {
            var target = (AudioManager)(serializedObject.targetObject);
            EditorGUILayout.LabelField("声音设置 : ");

            float musicVolume = target.GetVolumeByType(AudioType.Music);
            EditorGUILayout.LabelField("BGM音量 : " + (int)(musicVolume * 100) + "%");
            float sfxVolume = target.GetVolumeByType(AudioType.Sfx);
            EditorGUILayout.LabelField("音效音量 : " + (int)(sfxVolume * 100) + "%");
            //bool isMute = target.GetMuteStatus();
            EditorGUILayout.LabelField("[背景音乐] 是否静音 : " + target.GetMuteStatus(AudioType.Music));
            EditorGUILayout.LabelField("[音效] 是否静音 : " + target.GetMuteStatus(AudioType.Sfx));

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (target.AllAudioDataList.Count > 0)
            {
                EditorGUILayout.LabelField("声音列表 : ");
            }

            Dictionary<string, List<AudioData>> dic = new Dictionary<string, List<AudioData>>();
            for (int i = 0; i < target.AllAudioDataList.Count; i++)
            {
                AudioData data = target.AllAudioDataList[i];

                string nameKey = "";
                if (data.Source == null)
                {
                    nameKey = _nullReferenceName;
                }
                else
                {
                    nameKey = data.Go.name;
                }

                if (!dic.ContainsKey(nameKey))
                {
                    dic.Add(nameKey, new List<AudioData>() { data });
                }
                else
                {
                    dic[nameKey].Add(data);
                }
            }

            foreach (var kv in dic)
            {
                EditorGUILayout.LabelField(kv.Key);
                EditorGUILayout.LabelField("总数量 : " + kv.Value.Count);

                foreach (var data in kv.Value)
                {
                    if (_nullReferenceName == kv.Key)
                    {
                        EditorGUILayout.LabelField("空引用数据");
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("正在播放：" + (data.Source.isPlaying ? "√" : "×"), GUILayout.Width(70));
                        EditorGUILayout.LabelField(" | 是否循环 : " + (data.Source.loop ? "√" : "×"), GUILayout.Width(85));
                        EditorGUILayout.LabelField(" | 文件名 : " + (data.Source.clip?data.Source.clip.name: "×"), GUILayout.Width(140));

                        EditorGUILayout.LabelField(" | 声音类型 : ", GUILayout.Width(65));
                        EditorGUILayout.EnumPopup(data.PlayType, GUILayout.Width(70));

                        EditorGUILayout.LabelField(" | 文件类型 : ", GUILayout.Width(65));
                        EditorGUILayout.EnumPopup(data.Type, GUILayout.Width(70));
                        EditorGUILayout.ObjectField(data.Source, typeof(AudioSource), true);
                        EditorGUILayout.EndHorizontal();
                    }

                }

                EditorGUILayout.LabelField("----------------------------------");
            }
        }

    }

}
