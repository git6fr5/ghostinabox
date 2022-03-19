/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;

/// <summary>
/// Screen.
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class Screen : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

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

    public bool alive;

    public Vector2 offset;
    public Vector2 bounds;

    #endregion

    /* --- Unity --- */
    #region Unity

    // Runs once before the first frame.
    void Start() {
        Init();
    }

    void Update() {
        if (GameRules.MainPlayer.Alive) {
            volume.sharedProfile = volumeProfileA;
            SetPosition();
        }
        else {
            volume.sharedProfile = volumeProfileB;
            OutOfBounds();
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
    private void OutOfBounds() {
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
            GameRules.MainPlayer.transform.position = position;
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
