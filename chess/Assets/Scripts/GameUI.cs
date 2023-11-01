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

    [SerializeField] private TextMeshProUGUI GameAvailableText;


    public Action<bool> SetlocalGame;

    private void Awake() {
        Instance = this;
        server.Init(8007);
        client.Init("3.16.188.40", 8007);
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

    }

    public void OnOnlineGameButton() {
        menuAnimator.SetTrigger("OnlineMenu");
    }

    public void OnOnlineHostButton() {
        if (GameAvailableText.text == "No Matches Found") {
            SetlocalGame?.Invoke(false);
            menuAnimator.SetTrigger("HostMenu");
            NetPlayer msg = new NetPlayer();
            msg.matchName = addressInput.text;
            client.SendToServer(msg);
            client.SendToServer(new NetWelcome());
        }
    }

    public void OnOnlineConnectButton() {
        if (GameAvailableText.text != "No Matches Found") {
            SetlocalGame?.Invoke(false);
            menuAnimator.SetTrigger("HostMenu");
            client.SendToServer(new NetWelcome());
        }
    }

    public void OnOnlineBackButton() {
        menuAnimator.SetTrigger("StartMenu");
    }

    public void OnHostBackButton() {
        ThemeButton.SetActive(false);
        WaterButton.SetActive(false);
        menuAnimator.SetTrigger("OnlineMenu");
        NetPlayer msg = new NetPlayer();
        msg.matchName = "playercounterminus";
        client.SendToServer(msg);
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
