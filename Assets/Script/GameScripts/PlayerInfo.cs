using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerInfo : MonoBehaviourPunCallbacks
{
    public TMP_Text playerNameText;
    public Transform playerPositionText;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
    }

    private void OnEnable()
    {
        playerNameText.text = PhotonNetwork.LocalPlayer.NickName;

        // Set initial position text
        UpdatePlayerPositionText(transform.position);
    }

    private void UpdatePlayerPositionText(Vector3 position)
    {
        //playerPositionText.text = $"Position: {position.ToString()}";
        playerPositionText.position = position;
    }
}
