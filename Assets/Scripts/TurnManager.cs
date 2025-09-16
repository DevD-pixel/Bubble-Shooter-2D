using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager Instance;

    [SerializeField] private float turnDuration = 10f; // Duration of each turn in seconds
    private int currentPlayerIndex = 0;
    private float turnTimer = 0f;
    private bool isTurnActive = false;

    [SerializeField] private GameObject myTurnIndicator;
    [SerializeField] private GameObject opponentTurnIndicator;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return; // Master controls turns

        if (isTurnActive)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0f)
            {
                NextTurn();
            }
        }
    }

    /// <summary>
    /// Called when the current player performs an action
    /// </summary>
    public void OnPlayerAction(int actorNumber)
    {
        if (PhotonNetwork.PlayerList[currentPlayerIndex].ActorNumber == actorNumber)
        {
            // End turn immediately
            NextTurn();
        }
    }

    public void StartTurn()
    {
        if (PhotonNetwork.PlayerList.Length == 0) return;

        isTurnActive = true;
        turnTimer = turnDuration;

        int actorNumber = PhotonNetwork.PlayerList[currentPlayerIndex].ActorNumber;

        // Inform all clients whose turn it is
        photonView.RPC(nameof(RPC_StartTurn), RpcTarget.All, actorNumber, turnDuration,currentPlayerIndex);
    }

    private void NextTurn()
    {
        isTurnActive = false;

        currentPlayerIndex++;
        if (currentPlayerIndex >= PhotonNetwork.PlayerList.Length)
            currentPlayerIndex = 0;

        StartTurn();
    }

    /// <summary>
    /// Returns true if it's this client's turn
    /// </summary>
    public bool IsMyTurn()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[currentPlayerIndex].ActorNumber;
    }

    [PunRPC]
    private void RPC_StartTurn(int actorNumber, float duration, int currentPlayerIndex)
    {
        this.currentPlayerIndex = currentPlayerIndex;
        isTurnActive = true;
        turnTimer = duration;
        Debug.Log("Turn started for player " + actorNumber + " Duration: " + duration);
        myTurnIndicator.SetActive(IsMyTurn());
        opponentTurnIndicator.SetActive(!IsMyTurn());
    }
}
