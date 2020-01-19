using System;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    //所有
    All = 0,
    //背景音乐
    Music = 1,
    //音效
    Sfx = 2,
    //UI音效
    UISfx = 3,
}

public class AudioSetting
{
    public float Volume = 0.5f;
    public float Pitch = 1f;
    public bool IsMute = false;
}

/// <summary>
/// 声音管理类
/// </summary>
[MonoSingletonPath("[Framework]/AudioManager")]
public class AudioManager : MonoSingleton<AudioManager>
{
    /// <summary>
    /// 声音设置  Dictionary
    /// </summary>
    private readonly Dictionary<AudioType, AudioSetting> _audioSettingDic = new Dictionary<AudioType, AudioSetting>();

    /// <summary>
    /// 所有声音数据列表
    /// </summary>
    public readonly List<AudioData> AllAudioDataList = new List<AudioData>();

    /// <summary>
    /// 背景音乐
    /// </summary>
    private AudioData _bgmData;

    private void Awake()
    {
        InitBgm();

        foreach (AudioType type in Enum.GetValues(typeof(AudioType)))
        {
            if (type != AudioType.All)
            {
                AudioSetting setting = new AudioSetting();
                _audioSettingDic.Add(type, setting);
            }
        }

    }
    private void InitBgm()
    {
        _bgmData = GetIdleAudioData();
        _bgmData.Type = AudioType.Music;
        _bgmData.Source.loop = true;
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="fileName"></param>
    public AudioData PlayBgm(string fileName)
    {
        if (_bgmData.Source.isPlaying && _bgmData.FileName == fileName)
        {
            return null;
        }

        _bgmData.FileName = fileName;
        return Play(_bgmData);
    }

    /// <summary>
    /// 重载播放处理
    /// </summary>
    /// <param name="fileName">【必传】音频文件名 </param>
    /// <param name="type">【必传】文件类型</param>
    /// <param name="go">【可选参数】是否挂载到对象（挂载对象为3d音效 通常挂载赛车上）</param>
    /// <param name="isLoop">【可选参数】是否循环</param>
    /// <returns></returns>
    public AudioData Play(string fileName, AudioType type, GameObject go = null, bool isLoop = false)
    {
        AudioData data = GetIdleAudioData(go);
        data.FileName = fileName;
        data.Source.loop = isLoop;
        data.Type = type;
        Play(data);
        return data;
    }

    /// <summary>
    /// 最终播放函数
    /// </summary>
    /// <param name="data"></param>
    public AudioData Play(AudioData data)
    {
        AudioSource source = data.Source;

        if (source == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(data.FileName) && source.clip == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(data.FileName))
        {
            data.IsClipLoading = true;

            ResManager.Instance.Load<AudioClip>(data.FileName, clip =>
            {
                if (source && data.IsClipLoading)
                {
                    source.clip = clip;

                    AudioType type = data.Type;
                    data.SetVolume(GetVolumeByType(type));
                    data.SetPitch(GetPitch(type));
                    data.SetMuteStatus(GetMuteStatus(type));
                    data.Play();
                }
                data.IsClipLoading = false;
            });
        }

        return data;
    }


    /// <summary>
    /// 传入绑定声音的GameObject对象
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public AudioData GetIdleAudioData(GameObject go = null)
    {
        if (go == null)
        {
            //没有传入对象  绑定在Manager上
            go = gameObject;
        }

        AudioData idledata = null;

        for (int i = 0; i < AllAudioDataList.Count; i++)
        {
            AudioData audioData = AllAudioDataList[i];
            if(audioData.Type == AudioType.Music)continue;

            if (audioData.Source == null)
            {
                idledata = audioData;
                idledata.Source = go.AddComponent<AudioSource>();
                break;
            }
            else
            {
                //[没有在播放]    并且  [没有在加载音频]   并且   [对象和传入对象一致]
                if (!audioData.Source.isPlaying && !audioData.IsClipLoading && audioData.Go == go)
                {
                    idledata = audioData;
                    break;
                }
            }
        }

        if (idledata == null)
        {
            AudioSource source = go.AddComponent<AudioSource>();
            idledata = new AudioData(source);
            AllAudioDataList.Add(idledata);
        }

        idledata.PlayType = go == gameObject ? VoiceType.Is2D : VoiceType.Is3D;
        idledata.FileName = "";
        idledata.Source.clip = null;
        return idledata;
    }


    public void PauseBgm()
    {
        if (_bgmData != null && _bgmData.Source != null)
        {
            _bgmData.Source.Pause();
        }
    }

    public void UnPauseBgm()
    {
        if (_bgmData != null && _bgmData.Source != null)
        {
            _bgmData.Source.UnPause();
        }
    }

    public void StopAll(AudioType type = AudioType.All)
    {
        for (int i = 0; i < AllAudioDataList.Count; i++)
        {
            AudioData data = AllAudioDataList[i];
            if (type == AudioType.All || type == data.Type)
            {
                data.Stop();
            }
        }
    }

    public void StopByFileName(string fileName)
    {
        for (int i = 0; i < AllAudioDataList.Count; i++)
        {
            AudioData data = AllAudioDataList[i];
            if (data.FileName == fileName)
            {
                data.Stop();
            }
        }
    }

    /// <summary>
    /// 暂停所有声音
    /// </summary>
    public void PauseAll()
    {
        for (int i = 0; i < AllAudioDataList.Count; i++)
        {
            AudioData data = AllAudioDataList[i];
            data.Pause();
        }
    }

    /// <summary>
    /// 恢复所有暂停声音
    /// </summary>
    public void UnPauseAll()
    {
        for (int i = 0; i < AllAudioDataList.Count; i++)
        {
            AudioData data = AllAudioDataList[i];
            data.UnPause();
        }
    }

    /// <summary>
    /// 设置静音
    /// </summary>
    public void SetMuteStatus(bool isMute, AudioType type = AudioType.All)
    {
        //Logger.LogError("设置静音状态 isMute : " + isMute + " + type : " + type);
        foreach (var audioSetting in _audioSettingDic)
        {
            if (type == AudioType.All || type == audioSetting.Key)
            {
                audioSetting.Value.IsMute = isMute;
            }
        }

        for (int i = 0; i < AllAudioDataList.Count; i++)
        {
            AudioData data = AllAudioDataList[i];
            if (data.Type == type || type == AudioType.All)
            {
                data.SetMuteStatus(GetMuteStatus(data.Type));
            }
        }
    }

    public bool GetMuteStatus(AudioType type = AudioType.All)
    {
        return _audioSettingDic[type].IsMute;
    }

    /// <summary>
    /// 根据类型停止声音
    /// </summary>
    public void StopByType(AudioType type)
    {
        for (int i = 0; i < AllAudioDataList.Count; i++)
        {
            AudioData data = AllAudioDataList[i];
            if (data.Type == type)
            {
                data.Stop();
            }
        }
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="volume">音量大小(0~1)</param>
    /// <param name="type">声音文件类型</param>
    /// <param name="isFade">是否渐变</param>
    public void SetVolume(float volume, AudioType type = AudioType.All, bool isFade = false)
    {
        foreach (var audioSetting in _audioSettingDic)
        {
            if (type == AudioType.All || type == audioSetting.Key)
            {
                audioSetting.Value.Volume = volume;
            }
        }

        for (int i = 0; i < AllAudioDataList.Count; i++)
        {
            AudioData data = AllAudioDataList[i];
            if (data.Type == type || type == AudioType.All)
            {
                data.SetVolume(GetVolumeByType(data.Type), isFade);
            }
        }
    }

    /// <summary>
    /// 根据类型获取声音大小
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetVolumeByType(AudioType type)
    {
        AudioSetting setting = _audioSettingDic[type];
        return setting.Volume;
    }

    public void SetPitch(float pitch, AudioType type = AudioType.All)
    {
        foreach (var audioSetting in _audioSettingDic)
        {
            if (type == AudioType.All || type == audioSetting.Key)
            {
                audioSetting.Value.Pitch = pitch;
            }
        }

        for (int i = 0; i < AllAudioDataList.Count; i++)
        {
            AudioData data = AllAudioDataList[i];
            if (data.Type == type || type == AudioType.All)
            {
                data.SetPitch(GetPitch(data.Type));
            }
        }
    }

    public float GetPitch(AudioType type = AudioType.All)
    {
        AudioSetting setting = _audioSettingDic[type];
        return setting.Pitch;
    }

    public Dictionary<AudioType, AudioSetting> GetAudioSetting()
    {
        return _audioSettingDic;
    }
}
