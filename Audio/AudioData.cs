
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 播放类型  2D or 3D声音
/// </summary>
public enum VoiceType
{
    Is2D,
    Is3D
}

public class AudioData
{
    //绑定AudioSource的GameObject 对象
    public GameObject Go
    {
        get { return Source?.gameObject; }
    }

    //音源  (Source可能会不存在  对象移除情况下）
    public AudioSource Source;

    //声音文件类型 
    public AudioType Type = AudioType.Sfx;

    //正在加载音频文件中
    public bool IsClipLoading = false;

    //播放类型
    private VoiceType _playType;
    public VoiceType PlayType
    {
        get { return _playType; }
        set
        {
            _playType = value;
            SetVoiceSetting(_playType);
        }
    }

    //声音资源路径
    public string FileName;

    //唯一ID
    public readonly int Id;

    private static int _accumulateId = 0;

    public AudioData(AudioSource source)
    {
        Source = source;
        Id = _accumulateId++;
    }

    /// <summary>
    /// 修改声音类型
    /// </summary>
    private void SetVoiceSetting(VoiceType type)
    {
        switch (type)
        {
            case VoiceType.Is2D:
                Set2DVoiceSetting();
                break;
            case VoiceType.Is3D:
                Set3DVoiceSetting();
                break;
            default:
#if UNITY_EDITOR
                Debug.LogError("未实现声音类型");
#endif
                break;
        }
    }

    /// <summary>
    /// 设置为2d音效
    /// </summary>
    private void Set2DVoiceSetting()
    {
        if (Source)
        {
            Source.spatialBlend = 0;
        }
    }

    /// <summary>
    /// 设置为3d音效
    /// </summary>
    private void Set3DVoiceSetting()
    {
        if (Source)
        {
            Source.maxDistance = 20;
            Source.spatialBlend = 1;
            Source.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    /// <summary>
    /// 刷新静音状态
    /// </summary>
    public void RefreshMuteStatus()
    {
        if (Source)
        {
            bool isMute = AudioManager.Instance.GetMuteStatus(Type);
            Source.mute = isMute;
        }
    }

    public void SetVolume(float volume, bool isFade = false)
    {
        if (Source)
        {
            if (isFade)
            {
                //_volumeTween = Source.DOFade(volume, 0.5f);
            }
            else
            {
                //_volumeTween?.Kill();

                Source.volume = volume;
            }
        }
    }


    public void Play()
    {
        if (Source)
        {
            RefreshMuteStatus();
            Source.Play();
        }
    }

    public void Stop()
    {
        IsClipLoading = false;
        if (Source)
        {
            Source.clip = null;
            FileName = "";
            Source.Stop();
        }
    }

    public void Pause()
    {
        if (Source)
        {
            Source.Pause();
        }
    }

    public void UnPause()
    {
        if (Source)
        {
            Source.UnPause();
        }
    }

}