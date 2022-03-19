using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip explosionAudio;

    public SpriteRenderer spriteRenderer;
    public Particle explosion;
    public Trigger trigger;

    public bool exploded;

    public float interval = 0.1f;
    float ticks;

    void Update() {
        if (trigger.active && !exploded) {
            Explode();
        }
    }

    private void Explode() {
        GameRules.MainPlayer.Kill(transform.position);
        explosion.Create(transform.position);
        Destroy(gameObject);
        Screen.CameraShake(0.15f, 0.35f);

        //audioSource.clip = explosionAudio;
        //audioSource.Play();

        exploded = true;
    }
}
