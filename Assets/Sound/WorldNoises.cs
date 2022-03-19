/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using Movement = Controller.Movement;
using Direction = Controller.Direction;
using Airborne = Controller.Airborne;
using ActionState = Controller.ActionState;

/// <summary>
///
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class WorldNoises : MonoBehaviour {

    public static WorldNoises Instance;

    public AudioSource audioSource;

    public static AudioClip ChangePages;
    public AudioClip changePages;

    public static AudioClip ChangePagesAlt;
    public AudioClip changePagesAlt;

    public static AudioClip Collect;
    public AudioClip collect;

    public static AudioClip Slot;
    public AudioClip slot;

    public static AudioClip TriggerDeActive;
    public AudioClip triggerDeActive;

    public static AudioClip TriggerActive;
    public AudioClip triggerActive;

    public static AudioClip TriggerFire;
    public AudioClip triggerFire;

    public static AudioClip Break;
    public AudioClip breakSound;

    public static AudioClip MoveBoulder;
    public AudioClip moveBoulder;


    // Start is called before the first frame update
    void Start() {
        Init();
    }

    private void Init() {
        Instance = this;
        ChangePages = changePages;
        Collect = collect;
        Slot = slot;
        ChangePagesAlt = changePagesAlt;
        TriggerActive = triggerActive;
        TriggerDeActive = triggerDeActive;
        TriggerFire = triggerFire;
        Break = breakSound;
        MoveBoulder = moveBoulder;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public static void PlaySound(AudioClip audioClip) {
        Instance._PlaySound(audioClip);
    }

    public void _PlaySound(AudioClip audioClip) {
        if (audioSource.clip == audioClip && audioSource.isPlaying) {
            return;
        }
        audioSource.clip = audioClip;
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
        audioSource.Play();
    }

}
