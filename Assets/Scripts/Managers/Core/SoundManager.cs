using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];

    /// <summary>
    /// string: 경로
    /// </summary>
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject rootObject = GameObject.Find("@Sound");
        if (rootObject == null)
        {
            rootObject = new GameObject { name = "@Sound" };
            // 씬을 넘어가도 메모리 해제가 되지 않는다
            Object.DontDestroyOnLoad(rootObject);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject soundObject = new GameObject { name = soundNames[i] };
                _audioSources[i] = soundObject.AddComponent<AudioSource>();
                soundObject.transform.SetParent(rootObject.transform);
            }

            _audioSources[(int)Define.Sound.BGM].loop = true;
        }
    }

    string ValidatePath(string path)
    {
        if (path.Contains("Sounds/") == false)
        {
            path = $"Sounds/{path}";
        }

        return path;
    }

    AudioClip GetOrAddAudioClip(string path)
    {
        AudioClip audioClip;
        path = ValidatePath(path);

        _audioClips.TryGetValue(path, out audioClip);
        if (audioClip != null)
            return audioClip;

        // 이 무거운 작업을 중복하지 않기 위해 캐싱을 한다 _audioClips에 캐싱을 한다
        audioClip = Managers.Resource.Load<AudioClip>(path);
        if (audioClip == null)
        {
            Debug.Log($"AudioClip is missing : {path}");
            return null;
        }

        _audioClips.Add(path, audioClip);

        return audioClip;
    }

    // when a scene changes
    public void Clear()
    {
        foreach(AudioSource source in _audioSources)
        {
            source.clip = null;
            source.Stop();
        }

        _audioClips.Clear();
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path);
        Play(audioClip, type, pitch);
    }

     public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
     {
        if (audioClip == null)
            return;

        if (type == Define.Sound.BGM)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.BGM];
            if (audioSource.isPlaying == true)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.PlayOneShot(audioClip, pitch);
        }
    }
}
