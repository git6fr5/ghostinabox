/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[DefaultExecutionOrder(-1000)]
public class GameRules : MonoBehaviour {

    /* --- Static Tags --- */
    public static string PlayerTag = "Player";
    public static string GroundTag = "Ground";

    /* --- Static Objects --- */
    // Player.
    public static Player MainPlayer;
    public static bool PlayerInLight;
    // Camera.
    public static UnityEngine.Camera MainCamera;
    // Loader.
    public static LevelLoader MainLoader;

    /* --- Static Variables --- */
    // Mouse.
    public static Vector3 MousePosition => MainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
    // Movement.
    public static float VelocityDamping = 0.95f;
    public static float MovementPrecision = 0.05f;
    public static float GravityScale = 1f;
    public static float GroundCheckRadius = 0.1f;
    // Animation.
    public static float FrameRate = 8f;
    public static float OutlineWidth = 1f / 16f;

    /* --- Debug --- */
    [SerializeField] private float timeScale;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        Init();
    }

    // Runs once every frame.
    void Update() {

        if (!CheckRules()) {
            return;
        }

        // Check out of bounds.
        
        CheckReset();

        Time.timeScale = timeScale;

    }

    /* --- Methods --- */
    public static void Init() {
        MainPlayer = (Player)GameObject.FindObjectOfType(typeof(Player));
        MainLoader = (LevelLoader)GameObject.FindObjectOfType(typeof(LevelLoader));
        MainCamera = Camera.main;
    }

    public bool CheckRules() {
        if (MainPlayer == null) {
            return false;
        }
        if (MainLoader == null) {
            return false;
        }
        if (MainCamera == null) {
            return false;
        }
        return true;
    }

    public static void NextLevel() {
        if (MainLoader != null) {
            if (MainLoader.GetLevelByID(MainLoader.lDtkData, MainLoader.id + 1) != null) {
                MainLoader.id += 1;
            }
            MainLoader.load = true;
        }
        else {
            print("Resetting Level");
        }
    }

    public GameObject reset;

    private void CheckReset() {
        if (reset.activeSelf) {
            timeScale = 0f;
        }
        else {
            timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            if (reset.activeSelf) {
                ResetLevel();
            }
            reset.SetActive(!reset.activeSelf);
        }
    }

    public static void ResetLevel() {
        if (MainLoader != null) {
            MainLoader.Load();
            if (MainPlayer != null) {
                MainPlayer.Respawn();
            }
        }
        else {
            print("Resetting Level");
        }
    }

}
