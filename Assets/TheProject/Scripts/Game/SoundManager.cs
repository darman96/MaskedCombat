/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// SoundManager plays one shot sounds or named sounds, pooled as well
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : Singleton<SoundManager>
{
    public GameObject SoundPlayer;
    public GameObject SpeechPlayer;

    Dictionary<SoundType, AudioClip> SoundCache = new Dictionary<SoundType, AudioClip>();
    ObjectPool SoundAudioSourcePool = new ObjectPool();
    ObjectPool SpeechAudioSourcePool = new ObjectPool();
    Dictionary<GameObject, SoundType> PlayingSounds = new Dictionary<GameObject, SoundType>();
    Dictionary<string, GameObject> InstancedSounds = new Dictionary<string, GameObject>();

    public void Initialize()
    {
    }

    void Start()
    {
        StartCoroutine(RemoveOldPlayers());
    }

    void Awake()
    {
        base.Awake(this);

        AudioClip obj = null;

        foreach (SoundType st in ((SoundType[])Enum.GetValues(typeof(SoundType))))
        {
            obj = (AudioClip)Resources.Load("Sounds/" + st.ToString());
            SoundCache.Add(st, obj);
        }

        SoundAudioSourcePool.SetObjectPool(SoundPlayer, transform, 30);
        SpeechAudioSourcePool.SetObjectPool(SpeechPlayer, transform, 1);
    }

    private IEnumerator RemoveOldPlayers ()
    {
        List<GameObject> DelList = new List<GameObject>();

        while (true)
        {
            DelList.Clear();

            foreach (KeyValuePair<GameObject, SoundType> obj in PlayingSounds)
            {
                if (!obj.Key.GetComponent<AudioSource>().isPlaying)
                {
                    if (obj.Key.tag == "sound")
                        SoundAudioSourcePool.ReturnObject(obj.Key);
                    else
                        SpeechAudioSourcePool.ReturnObject(obj.Key);

                    DelList.Add(obj.Key);
                }
            }

            foreach (GameObject o in DelList)
                PlayingSounds.Remove(o);

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Play sound that gets auto deleted.
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="Rotation"></param>
    /// <param name="Type"></param>
    public void Play(Vector3 Position, Quaternion Rotation, SoundType Type, bool IsSpeech = false)
    {
        GameObject player = null;

        if (!IsSpeech)
        {
            player = SoundAudioSourcePool.GetObject();
        }
        else
        {
            player = SpeechAudioSourcePool.GetObject();
        }

        if (player != null)
        {
            AudioSource source = player.GetComponent<AudioSource>();
            source.clip = SoundCache[Type];
            source.loop = false;

            if (!IsSpeech)
                source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

            source.Play();

            PlayingSounds.Add(player, Type);
        }
    }

    /// <summary>
    /// Stop all sounds from that type.
    /// </summary>
    /// <param name="Type"></param>
    public void Stop(SoundType Type)
    {
        List<GameObject> DelList = new List<GameObject>();

        foreach (KeyValuePair<GameObject, SoundType> obj in PlayingSounds)
        {
            if (obj.Value == Type)
            {
                obj.Key.GetComponent<AudioSource>().Stop();

                if (obj.Key.tag == "sound")
                    SoundAudioSourcePool.ReturnObject(obj.Key);
                else
                    SpeechAudioSourcePool.ReturnObject(obj.Key);

                DelList.Add(obj.Key);
            }
        }

        foreach (GameObject o in DelList)
            PlayingSounds.Remove(o);
    }

    /// <summary>
    ///  Play sound in ID channel.
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="Position"></param>
    /// <param name="Rotation"></param>
    /// <param name="Type"></param>
    public void Play(string ID, Vector3 Position, Quaternion Rotation, SoundType Type, bool IsSpeech = false)
    {
        if (InstancedSounds.ContainsKey(ID))
        {
            if (!InstancedSounds[ID].GetComponent<AudioSource>().isPlaying)
            {
                RemoveSound(InstancedSounds[ID]);
            }
            else
                return;
        }

        GameObject player = null;

        if (!IsSpeech)
        {
            player = SoundAudioSourcePool.GetObject();
        }
        else
        {
            player = SpeechAudioSourcePool.GetObject();
        }

        if (player != null)
        {
            AudioSource source = player.GetComponent<AudioSource>();
            source.clip = SoundCache[Type];
            source.loop = false;

            if (!IsSpeech)
                source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

            source.Play();

            InstancedSounds[ID] = player;
        }
    }

    /// <summary>
    /// Loop specific sound with certain ID. Use that ID to stop the sound later.
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="Position"></param>
    /// <param name="Rotation"></param>
    /// <param name="Type"></param>
    public void Loop(string ID, Vector3 Position, Quaternion Rotation, SoundType Type, bool IsSpeech = false)
    {
        if (InstancedSounds.ContainsKey(ID))
        {
            if (!InstancedSounds[ID].GetComponent<AudioSource>().isPlaying)
            {
                RemoveSound(InstancedSounds[ID]);
            }
            else
                return;
        }

        GameObject player = null;

        if (!IsSpeech)
        {
            player = SoundAudioSourcePool.GetObject();
        }
        else
        {
            player = SpeechAudioSourcePool.GetObject();
        }

        if (player != null)
        {
            AudioSource source = player.GetComponent<AudioSource>();
            source.clip = SoundCache[Type];
            source.loop = true;
            source.Play();

            InstancedSounds[ID] = player;
        }
    }

    /// <summary>
    /// Stop specific sound by ID.
    /// </summary>
    /// <param name="ID"></param>
    public void Stop(string ID)
    {
        if (InstancedSounds.ContainsKey(ID))
        {
            GameObject del = InstancedSounds[ID];

            RemoveSound(del);

            InstancedSounds.Remove(ID);
        }
    }

    /// <summary>
    /// Stop specific sound by obj.
    /// </summary>
    /// <param name="del"></param>
    private void RemoveSound (GameObject del)
    {
        del.GetComponent<AudioSource>().Stop();

        if (del.tag == "sound")
            SoundAudioSourcePool.ReturnObject(del);
        else
            SpeechAudioSourcePool.ReturnObject(del);
    }
}
