/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Screen.
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class Screen : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    public static Screen Instance;

    // Components.
    [HideInInspector] public Camera mainCamera;
    [SerializeField] public PixelPerfectCamera pixelPerfectCamera;
    
    // Settings.
    [SerializeField, ReadOnly] private Vector2 screenSize;
    public static Vector2 Size;

    // Post Processing.
    public Volume volume;
    public VolumeProfile volumeProfileA;
    public VolumeProfile volumeProfileB;
    public VolumeProfile volumeProfileC;

    public LensDistortion lensA;
    public LensDistortion lensB;
    public ChromaticAberration chroma;
    public AnimationCurve chromaCurve;

    public DepthOfField depthOfField;
    public AnimationCurve depthCurve;

    public bool alive;

    public Vector2 offset;
    public Vector2 bounds;

    Vector3 origin;

    [Header("Shake")]
    public AnimationCurve curve;
    [SerializeField, ReadOnly] public float shakeStrength = 1f;
    [SerializeField, ReadOnly] public float shakeDuration = 0.5f;
    [SerializeField, ReadOnly] float elapsedTime = 0f;
    [SerializeField, ReadOnly] public bool shake;

    #endregion

    /* --- Unity --- */
    #region Unity

    // Runs once before the first frame.
    void Start() {
        Init();
    }

    void Update() {
        transform.position = origin;

        if (GameRules.MainPlayer.Alive) {
            volume.sharedProfile = volumeProfileA;
            // SetPosition();
            lensA.intensity.value = 0f; 
            OutOfBounds(true);
        }
        else {
            volume.sharedProfile = volumeProfileB;
            OutOfBounds();

            lensB.intensity.value = 0.5f * chromaCurve.Evaluate(GameRules.MainPlayer.RespawnRatio);
            chroma.intensity.value = 1f * chromaCurve.Evaluate(GameRules.MainPlayer.RespawnRatio);
        }

        if (GameRules.Resetting) {
            // volume.sharedProfile = volumeProfileC;
            lensA.intensity.value = 1f * depthCurve.Evaluate(GameRules.ResetTicks);
            lensB.intensity.value = 1f * depthCurve.Evaluate(GameRules.ResetTicks);
        }

        if (shake) {
            shake = Shake();
        }
    }

    private void SetPosition() {
        Vector2 target = (Vector2)transform.position + offset;
        Vector2 pos = (Vector2)GameRules.MainPlayer.transform.position;
        bool checkX = pos.x < target.x + bounds.x / 2f && pos.x > target.x - bounds.x / 2f;
        bool checkY = pos.y < target.y + bounds.y / 2f && pos.y > target.y - bounds.y / 2f;

        if (!checkX || !checkY) {
            // float speed = Mathf.Max(1f, Mathf.Min((target - pos).magnitude, 5f));
            Vector2 dx = (pos - target).normalized * 10f * Time.deltaTime;
            transform.position += (Vector3)dx;
        }
    }

    /* --- Static Methods --- */
    private void OutOfBounds(bool reset = false) {
        Vector3 position = GameRules.MainPlayer.transform.position;
        Vector3 cameraPosition = transform.position;

        if (position.x < cameraPosition.x - screenSize.x / 2f - GameRules.MovementPrecision) {
            position.x += screenSize.x;
        }
        if (position.x > cameraPosition.x + screenSize.x / 2f + GameRules.MovementPrecision) {
            position.x -= screenSize.x;
        }

        if (position.y < cameraPosition.y - screenSize.y / 2f - GameRules.MovementPrecision) {
            position.y += screenSize.y;
        }
        if (position.y > cameraPosition.y + screenSize.y / 2f + GameRules.MovementPrecision) {
            position.y -= screenSize.y;
        }

        if (GameRules.MainPlayer.transform.position != position) {
            if (reset) {
                GameRules.Resetting = true;
                GameRules.OverrideManualReset = true;
            }
            else {
                GameRules.MainPlayer.transform.position = position;
            }
        }

        if (GameRules.MainPlayer.transform.position == position && GameRules.OverrideManualReset) {
            GameRules.Resetting = false;
            GameRules.OverrideManualReset = false;
        }
    }

    public bool Shake() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shakeDuration) {
            elapsedTime = 0f;
            return false;
        }
        float strength = shakeStrength * curve.Evaluate(elapsedTime / shakeDuration);
        transform.position += (Vector3)Random.insideUnitCircle * strength;
        return true;
    }

    public static void CameraShake(float strength, float duration) {
        if (strength == 0f) {
            return;
        }
        if (!Instance.shake) {
            Instance.shakeStrength = strength;
            Instance.shakeDuration = duration;
            Instance.shake = true;
        }
        else {
            Instance.shakeStrength = Mathf.Max(Instance.shakeStrength, strength);
            Instance.shakeDuration = Mathf.Max(Instance.shakeDuration, Instance.elapsedTime + duration);
        }
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    // Initializes this script.
    public void Init() {
        mainCamera = GetComponent<Camera>();
        volume.sharedProfile = null;
        Size = screenSize;
        origin = transform.position;

        volumeProfileA.TryGet<LensDistortion>(out lensA);
        volumeProfileB.TryGet<LensDistortion>(out lensB);
        volumeProfileB.TryGet<ChromaticAberration>(out chroma);
        volumeProfileC.TryGet<DepthOfField>(out depthOfField);

        Instance = this;

    }

    #endregion

    /* --- Debugging --- */
    #region Debugging

    void OnDrawGizmos() {
        Vector3 screenSize = new Vector3((float)pixelPerfectCamera.refResolutionX / pixelPerfectCamera.assetsPPU, (float)pixelPerfectCamera.refResolutionY / pixelPerfectCamera.assetsPPU, 0f);
        Gizmos.DrawWireCube(transform.position, screenSize);
        this.screenSize = screenSize;

        Gizmos.DrawWireCube(transform.position + (Vector3)offset, (Vector3)bounds);

    }

    #endregion


}
