using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_PlayAudio : MonoBehaviour, IInteractFunction
{
    [Header("Audio track")]
    [SerializeField] AudioClip audioTrack;
    [SerializeField] bool loop = false;

    private GameDataFile GDFile;
    private EventManager EM;

    public void Interact()
    { PlayAudio(); }
    public string InteractType()
    { return "Audio"; }
    public string InteractText()
    { return "to play " + audioTrack.name; }
    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        EM = GDFile.GetComponent<EventManager>();
    }
    private void PlayAudio()
    {
        GDFile.MasterAudio.Stop();
        string trackName = this.name.Remove(0, 19);
        if (GDFile.currentTrack.Contains(trackName))
        {
            GDFile.MasterAudio.Stop();
            GDFile.currentTrack = "";
            return;
        }
        GDFile.MasterAudio.clip = audioTrack;
        GDFile.MasterAudio.loop = loop;
        GDFile.currentTrack = audioTrack.name;
        GDFile.MasterAudio.Play();
    }
}
