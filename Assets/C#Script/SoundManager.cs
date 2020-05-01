using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

//
// 参考サイト：https://qiita.com/waken/items/a0288c9b160a20022635
//

[System.Serializable]
public class SoundVolume
{
    public float bgm = 1.0f;
    public float se = 1.0f;

    public bool mute = false;

    public void Reset()
    {
        bgm = 1.0f;
        se = 1.0f;
        mute = false;
    }
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    public SoundVolume volume = new SoundVolume();

    private AudioClip[] seClips;
    private AudioClip[] bgmClips;

    private Dictionary<string, int> seIndexes = new Dictionary<string, int>();
    private Dictionary<string, int> bgmIndexes = new Dictionary<string, int>();

    const int cNumChannel = 16;
    private AudioSource bgmSource;
    private AudioSource[] seSources = new AudioSource[cNumChannel];

    Queue<int> seRequestQueue = new Queue<int>();

    //------------------------------------------------------------------------------
    void Awake()
    {
        Debug.Log("Audio読み込み開始");
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;

        for (int i = 0; i < seSources.Length; i++)
        {
            seSources[i] = gameObject.AddComponent<AudioSource>();
        }

        seClips = Resources.LoadAll<AudioClip>("Audio/SE");
        bgmClips = Resources.LoadAll<AudioClip>("Audio/BGM");

        for (int i = 0; i < seClips.Length; ++i)
        {
            seIndexes[seClips[i].name] = i;
        }

        for (int i = 0; i < bgmClips.Length; ++i)
        {
            bgmIndexes[bgmClips[i].name] = i;
        }
        Debug.Log("Audio読み込み完了");

        /* Debug.Log("se ========================"); */
        /* foreach(var ac in seClips ) { Debug.Log( ac.name ); } */
        /* Debug.Log("bgm ========================"); */
        /* foreach(var ac in bgmClips ) { Debug.Log( ac.name ); } */
    }

    //------------------------------------------------------------------------------
    void Update()
    {
        bgmSource.mute = volume.mute;
        foreach (var source in seSources)
        {
            source.mute = volume.mute;
        }

        bgmSource.volume = volume.bgm;
        foreach (var source in seSources)
        {
            source.volume = volume.se;
        }

        int count = seRequestQueue.Count;
        if (count != 0)
        {
            int sound_index = seRequestQueue.Dequeue();
            playSeImpl(sound_index);
        }
    }

    //------------------------------------------------------------------------------
    private void playSeImpl(int index)
    {
        if (0 > index || seClips.Length <= index)
        {
            return;
        }

        foreach (AudioSource source in seSources)
        {
            if (false == source.isPlaying)
            {
                source.clip = seClips[index];
                source.Play();
                return;
            }
        }
    }

    //------------------------------------------------------------------------------
    public int GetSeIndex(string name)
    {
        return seIndexes[name];
    }

    //------------------------------------------------------------------------------
    public int GetBgmIndex(string name)
    {
        return bgmIndexes[name];
    }

    //------------------------------------------------------------------------------
    public void PlayBgm(string name)
    {
        Debug.Log(bgmClips[0].name);
        int index = bgmIndexes[name];
        PlayBgm(index);
    }

    //------------------------------------------------------------------------------
    public void PlayBgm(int index)
    {
        // 例外排除
        if (0 > index || bgmClips.Length <= index)
        {
            return;
        }

        // 既に流れているならそのまま
        if (bgmSource.clip == bgmClips[index])
        {
            return;
        }

        bgmSource.Stop();
        bgmSource.clip = bgmClips[index];
        bgmSource.Play();
        Debug.Log("BGM「" + bgmClips[index] + "」再生");
    }

    //------------------------------------------------------------------------------
    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    //------------------------------------------------------------------------------
    public void PlaySe(string name)
    {
        PlaySe(GetSeIndex(name));
    }

    //一旦queueに溜め込んで重複を回避しているので
    //再生が1frame遅れる時がある
    //------------------------------------------------------------------------------
    public void PlaySe(int index)
    {
        if (!seRequestQueue.Contains(index))
        {
            seRequestQueue.Enqueue(index);
        }
    }

    //------------------------------------------------------------------------------
    public void StopSe()
    {
        ClearAllSeRequest();
        foreach (AudioSource source in seSources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    //------------------------------------------------------------------------------
    public void ClearAllSeRequest()
    {
        seRequestQueue.Clear();
    }

}