using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource coinAudio, deathAudio, backgroundAudio, clickAudio;

    void Awake() { Instance = this; }
}
