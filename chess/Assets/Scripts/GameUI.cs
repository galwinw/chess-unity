using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;


public enum GameCameraAngle {
    menu = 0,
    whiteTeam = 1,
    blackTeam = 2,

}

public class GameUI : MonoBehaviour {
    public static GameUI Instance {set; get;}

    public Server server;
    public Client client;

    [SerializeField] private Animator menuAnimator;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] public GameObject[] cameraAngles;
    [SerializeField] private GameObject ThemeButton;
    [SerializeField] private GameObject WaterButton;
    [SerializeField] private GameObject LeaveButton;


    public Action<bool> SetlocalGame;

    private void Awake() {
        Instance = this;
        server.Init(8007);
        //client.Init("3.15.222.39", 8007);
        RegisterEvents();
    }

    // Cameras
    public void changeCamera(GameCameraAngle angle) {
        for (int i = 0; i < cameraAngles.Length; i++) {
            cameraAngles[i].SetActive(false);
        }
        cameraAngles[(int)angle].SetActive(true);
    }

    //Buttons
    public void OnLocalGameButton() {
        menuAnimator.SetTrigger("InGameMenu");
        SetlocalGame?.Invoke(true);
        //server.Init(8008);
        client.Init("127.0.0.1", 8007);
    }

    public void OnOnlineGameButton() {
        menuAnimator.SetTrigger("OnlineMenu");
    }

    public void OnOnlineHostButton() {
        SetlocalGame?.Invoke(false);
        //server.Init(8008);
        client.Init("3.15.222.39", 8007);
        //client.Init("127.0.0.1", 8007);
        menuAnimator.SetTrigger("HostMenu");
    }

    public void OnOnlineConnectButton() {
        SetlocalGame?.Invoke(false);
        client.Init(addressInput.text, 8008);
        menuAnimator.SetTrigger("HostMenu");
    }

    public void OnOnlineBackButton() {
        menuAnimator.SetTrigger("StartMenu");
    }

    public void OnHostBackButton() {
        server.Shutdown();
        client.Shutdown();
        ThemeButton.SetActive(false);
        WaterButton.SetActive(false);
        menuAnimator.SetTrigger("OnlineMenu");
    }

    public void OnLeaveFromGameMenu() {
        changeCamera(GameCameraAngle.menu);
        menuAnimator.SetTrigger("StartMenu");

    }

    public void OnInGameLeaveBtn() {
        Chessboard.Instance.OnMenuButton();
    }

    #region
    private void RegisterEvents() {
        NetUtility.C_START_GAME += OnStartGameClient;
    }
    private void UnregisterEvents() {
        NetUtility.C_START_GAME -= OnStartGameClient;
    }

    private void OnStartGameClient(NetMessage msg) {
        menuAnimator.SetTrigger("InGameMenu");
        LeaveButton.SetActive(true);
    }

    private void OnDestroy() {
       UnregisterEvents();
    }

    #endregion

}
