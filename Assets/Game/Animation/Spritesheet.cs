/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Spritesheet : MonoBehaviour {

    /* --- Components --- */
    [Space(2), Header("Components")]
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Controller controller;
    [SerializeField] private Sprite[] sprites;

    /* --- Parameters --- */
    [Space(2), Header("Parameters")]
    [SerializeField] private int indexOffset;
    [SerializeField] private int idleFrames;
    [SerializeField] private int movementFrames;
    [SerializeField] private int risingFrames;
    [SerializeField] private int fallingFrames;
    [SerializeField] private int deathFrames;

    /* --- Properties --- */
    [Space(2), Header("Properties")]
    [HideInInspector] private Sprite[] idleAnimation;
    [HideInInspector] private Sprite[] movementAnimation;
    [HideInInspector] private Sprite[] risingAnimation;
    [HideInInspector] private Sprite[] fallingAnimation;
    [HideInInspector] private Sprite[] deadAnimation;

    [Space(2), Header("Animation Data")]
    [HideInInspector] private Sprite[] currentAnimation;
    [HideInInspector] private Sprite[] previousAnimation;
    [SerializeField, ReadOnly] private int currentFrame;

    [Space(2), Header("Ticks")]
    [SerializeField, ReadOnly] private float ticks;
    [SerializeField, ReadOnly] private float frameRate;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        Init();
    }

    void Update() {
        float deltaTime = Time.deltaTime;
        Animate(deltaTime);
        Rotate();
    }

    /* --- Methods --- */
    public void Init() {
        // Caching components.
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller>();
        Organize();
        StartCoroutine(IEAlive());
    }

    private void Animate(float deltaTime) {

        currentAnimation = GetAnimation();

        ticks = previousAnimation == currentAnimation ? ticks + deltaTime : 0f;
        currentFrame = (int)Mathf.Floor(ticks * frameRate) % currentAnimation.Length;

        // Set the current frame.
        spriteRenderer.sprite = currentAnimation[currentFrame];
        previousAnimation = currentAnimation;
    }

    // Gets the current animation info.
    public Sprite[] GetAnimation() {
        frameRate = GameRules.FrameRate;
        if (controller.GetComponent<Player>() != null && !controller.GetComponent<Player>().Alive) {
            frameRate *= 0.25f;
            return deadAnimation;
        }
        else if (controller.GetComponent<Chicken>() != null && !controller.GetComponent<Chicken>().Alive) {
            frameRate *= 0.25f;
            return deadAnimation;
        }
        else if (controller.airborneFlag != Controller.Airborne.Grounded) {
            switch (controller.airborneFlag) {
                case Controller.Airborne.Rising:
                    return risingAnimation;
                default:
                    return fallingAnimation;
            }
        }
        else if (controller.movementFlag == Controller.Movement.Moving) {
            frameRate *= 2f;
            return movementAnimation;
        }
        else {
            return idleAnimation;
        }
    }

    private void Rotate() {
        if (controller.directionFlag == Controller.Direction.Left) {
            transform.eulerAngles = 180f * Vector3.up;
        }
        else {
            transform.eulerAngles = Vector3.zero;
        }
    }

    // Organizes the sprite sheet into its animations.
    public void Organize() {
        int startIndex = indexOffset;

        startIndex = SliceSheet(startIndex, idleFrames, ref idleAnimation);
        startIndex = SliceSheet(startIndex, movementFrames, ref movementAnimation);
        startIndex = SliceSheet(startIndex, risingFrames, ref risingAnimation);
        startIndex = SliceSheet(startIndex, fallingFrames, ref fallingAnimation);
        startIndex = SliceSheet(startIndex, deathFrames, ref deadAnimation);

    }

    // Slices an animation out of the the sheet.
    private int SliceSheet(int startIndex, int length, ref Sprite[] array) {
        List<Sprite> splicedSprites = new List<Sprite>();
        for (int i = startIndex; i < startIndex + length; i++) {
            splicedSprites.Add(sprites[i]);
        }
        array = splicedSprites.ToArray();
        return startIndex + length;
    }

    private IEnumerator IEAlive() {
        float duration = 0.1f;
        while (true) {
            if (controller != null && controller.GetComponent<Player>() != null) {
                Player player = controller.GetComponent<Player>();
                if (!player.Alive) {
                    AfterImage(3 * duration, (1f - player.RespawnRatio) / 2f);
                }
            }
            else if (controller != null && controller.GetComponent<Chicken>() != null) {
                duration = 0.3f;
                Chicken chicken = controller.GetComponent<Chicken>();
                if (!chicken.Alive) {
                    AfterImage(3 * duration, (1f - GameRules.MainPlayer.RespawnRatio) / 2f);
                }
            }
            yield return new WaitForSeconds(duration);
        }
    }

    public void AfterImage(float delay, float transparency) {
        SpriteRenderer afterImage = new GameObject("AfterImage", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        // afterImage.transform.SetParent(transform);
        afterImage.transform.position = transform.position;
        afterImage.transform.localRotation = transform.localRotation;
        afterImage.transform.localScale = transform.localScale;
        afterImage.sprite = spriteRenderer.sprite;
        afterImage.color = Color.white * transparency;
        afterImage.sortingLayerName = spriteRenderer.sortingLayerName;
        Destroy(afterImage.gameObject, delay);
    }

}
