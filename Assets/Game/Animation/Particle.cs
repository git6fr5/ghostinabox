/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays a particle animation.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Particle : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Components.
    private SpriteRenderer spriteRenderer;

    // Sprites.
    [SerializeField] private Sprite[] sprites;

    // Settings.
    [SerializeField] private bool loop;
    [SerializeField] private Vector3 target;
    [SerializeField, ReadOnly] private float speed;

    // Animation.
    [SerializeField] private int frameRate = 12;
    [SerializeField, ReadOnly] private int frame;
    [SerializeField, ReadOnly] private float ticks;

    public bool isProjectile;

    #endregion

    /* --- Unity --- */
    #region Unity
    public bool forceInit;
    void Start() {
        if (forceInit) {
            Init();
        }
    }

    // Runs once every frame.
    private void Update() {
        float deltaTime = Time.deltaTime;
        Animate(deltaTime);
        Move(deltaTime);
        Rotate(deltaTime);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public Particle Create(Vector3 position, float lifeTime = -1f) {
        Particle newParticle = Instantiate(gameObject, position, Quaternion.identity, null).GetComponent<Particle>();
        newParticle.Init();
        if (lifeTime > 0f) {
            Destroy(newParticle.gameObject, lifeTime);
        }
        return newParticle;
    }

    public void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        target = transform.position;
        speed = 0f;
        gameObject.SetActive(true);
    }

    public void MoveTo(Vector2Int position, float duration) {
        target = (Vector3)(Vector2)position;
        speed = (target - transform.position).magnitude / duration;
    }

    private bool falling = false;
    private Vector3 origin;
    private float height = 8f;
    private float ratio = 0f;

    public void FallTo(Vector2Int position, float duration) {
        falling = true;
        origin = transform.position;
        ratio = 0f;
        MoveTo(position, duration);
    }

    #endregion

    /* --- Animation --- */
    #region Animation

    private void Animate(float deltaTime) {
        // Set the current frame.
        frame = (int)Mathf.Floor(ticks * frameRate);
        if (!loop && frame >= sprites.Length) {
            Destroy(gameObject);
        }
        frame = frame % sprites.Length;
        spriteRenderer.sprite = sprites[frame];

        ticks += deltaTime;

    }

    private void Move(float deltaTime) {
        if (!falling) {
            Vector3 displacement = target - transform.position;
            if (displacement.sqrMagnitude >= 0.05f * 0.05f) {
                transform.position += displacement.normalized * speed * deltaTime;
            }
        }
        else {
            Vector3 displacement = target - origin;
            ratio += (speed * deltaTime) / displacement.magnitude;
            float y = -((ratio - 0.5f) * (ratio - 0.5f) - (0.5f * 0.5f)) * height;
            transform.position = origin + displacement * ratio + Vector3.up * y;
        }
    }

    private void Fall(float deltaTime) {
        Vector3 displacement = target - transform.position;
        if (displacement.sqrMagnitude >= 0.05f * 0.05f) {
            transform.position += displacement.normalized * speed * deltaTime;
        }
    }

    private void Rotate(float deltaTime) {
        // transform.eulerAngles += speed * Vector3.forward;
    }

    #endregion

}
