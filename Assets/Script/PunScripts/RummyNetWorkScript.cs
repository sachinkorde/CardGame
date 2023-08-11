using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class RummyNetWorkScript : MonoBehaviourPunCallbacks
{
    public TMP_InputField PlayerNameInput;
    public TMP_InputField roomNameText;
    public TMP_InputField maxPlayers;

    public TMP_Text ConnectionStatusText;

    public GameObject[] gamePanels;
    public GameObject roomListPrefab;
    public GameObject roomInstantiateParent;
    public GameObject playerListPrefab;
    public GameObject playerListPrefabParent;

    private readonly string connectionStatusMessage = "    Connection Status: ";

    private Dictionary<string, RoomInfo> roomListData;
    private Dictionary<string, GameObject> roomListGameObject;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        GamePanelHandler(gamePanels[0]);
        roomListData = new Dictionary<string, RoomInfo>();
        roomListGameObject = new Dictionary<string, GameObject>();
    }

    #region Ui_Methods
    void GamePanelHandler(GameObject panel)
    {
        for (int i = 0; i < gamePanels.Length; i++)
        {
            gamePanels[i].SetActive(false);
        }
        panel.SetActive(true);
    }

    void ActiveConnectingPanel()
    {
        isConnecting = true;
        time = 0.0f;
        loading_time = 0.0f;
        GamePanelHandler(gamePanels[4]);
    }

    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
            GamePanelHandler(gamePanels[4]);
            isConnecting = true;
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    public void OpenCreateRoomPanel()
    {
        ActiveConnectingPanel();
        Invoke(nameof(CreateRoomPanel), 0.5f);
    }

    void CreateRoomPanel()
    {
        GamePanelHandler(gamePanels[2]);
    }

    public void OnClickRoomCreated()
    {
        if(string.IsNullOrEmpty(roomNameText.text)) 
        {
            roomNameText.text = "Room " + Random.Range(0, 50);
        }

        ActiveConnectingPanel();
        Invoke(nameof(CreateRoomProcess), 0.45f);
    }

    void CreateRoomProcess()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) int.Parse(maxPlayers.text);

        PhotonNetwork.CreateRoom(roomNameText.text);
    }

    public void OnClickCancel()
    {
        GamePanelHandler(gamePanels[0]);
    }

    public void OnClickRoomListOpen()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActiveConnectingPanel();
        Invoke(nameof(OpenRoomListView), 0.5f);
    }

    void OpenRoomListView()
    {
       GamePanelHandler(gamePanels[5]);
    }

    public void RoomJoinFromList(string roomName)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName);
            GamePanelHandler(gamePanels[6]);
        }
    }

    public void ClearRoomList()
    {
        if(roomListGameObject.Count  > 0)
        foreach (var item in roomListGameObject.Values)
        {
            Destroy(item);
        }
        roomListGameObject.Clear();
    }

    public void BackFromRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            ActiveConnectingPanel();

            Invoke(nameof(CreateRoomPanel), 0.5f);
        }
    }

    #endregion

    #region Photon_CallBacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
        GamePanelHandler(gamePanels[4]);
        isConnecting = true;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conneted");
        GamePanelHandler(gamePanels[1]);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " : Room Created");
        GamePanelHandler(gamePanels[3]);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " : player joined room");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Clear Old RoomList Data
        ClearRoomList();

        foreach (RoomInfo room in roomList)
        {
            Debug.Log("Room Name : " + room.Name);
            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (roomListData.ContainsKey(room.Name))
                {
                    roomListData.Remove(room.Name);
                }
            }
            else
            {
                if (roomListData.ContainsKey(room.Name))
                {
                    roomListData[room.Name] = room;
                }
                else
                {
                    roomListData.Add(room.Name, room);
                }
            }
        }

        //Instantiate Created Rooms
        foreach (RoomInfo roomItems in roomListData.Values)
        {
            GameObject roomListObject = Instantiate(roomListPrefab, roomInstantiateParent.transform);
            roomListObject.transform.localScale = Vector3.one;

            roomListObject.transform.GetChild(0).transform.GetComponent<TMP_Text>().text = roomItems.Name;
            roomListObject.transform.GetChild(1).transform.GetComponent<TMP_Text>().text = roomItems.PlayerCount + "/" + roomItems.MaxPlayers;
            roomListObject.transform.GetChild(2).transform.GetComponent<Button>().onClick.AddListener(() => RoomJoinFromList(roomItems.Name));
            roomListGameObject.Add(roomItems.Name, roomListObject);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomList();
        roomListData.Clear();
    }

    public override void OnJoinedRoom()
    {
        GamePanelHandler(gamePanels[6]);

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject playerListItem = Instantiate(playerListPrefab, playerListPrefabParent.transform);
            playerListItem.transform.localScale = Vector3.one;

            playerListItem.transform.GetChild(0).transform.GetComponent<TMP_Text>().text = p.NickName;

            if(p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
    #endregion

    public TMP_Text loading_txt, dots_txt;
    public float time, loading_time;
    public bool isConnecting = false;
    public void Update()
    {
        ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;

        if (isConnecting)
        {
            time += Time.deltaTime;

            if (time > 0.8f)
            {
                isConnecting = false;
            }
        }

        if (loading_txt != null)
        {
            if (loading_txt.text != null)
            {
                dots_txt.text = ".";

                loading_time += Time.deltaTime;

                if (loading_time > 0.25f && loading_time < 0.5f)
                {
                    dots_txt.text = "..";
                }
                else if (loading_time > 0.5f && loading_time < 0.75f)
                {
                    dots_txt.text = "...";
                }
                else if (loading_time > 0.75f && loading_time < 1)
                {
                    dots_txt.text = "";
                }
                else if (loading_time > 1f)
                {
                    loading_time = 0;
                }
            }
        }
    }
}
