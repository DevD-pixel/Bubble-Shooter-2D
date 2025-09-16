using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviourPunCallbacks
{
	private int score = 0;
	private int throws = 0;
	public static ScoreManager instance;

    void Awake()
    {
        instance = this;
    }

    public void UpdateScoreUI(int myScore, int opponentScore)
	{
		Text _score = GameObject.FindWithTag("Score").GetComponent<Text>();
		_score.text = $"My Score: {myScore}";

		Text _op_score = GameObject.FindWithTag("Score_OP").GetComponent<Text>();
		_op_score.text = $"Opp Score: {opponentScore}";
	}

	public void AddScore(int score)
	{	
		if (TurnManager.Instance.IsMyTurn())
		{
			int currentScore = 0;
			Debug.Log("====== AddScore ======");
			if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Score"))
				currentScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"];
			int newScore = currentScore + score;
			Debug.Log("====== New Score ======" + newScore);
			ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
			props["Score"] = newScore;
			PhotonNetwork.LocalPlayer.SetCustomProperties(props);
			Debug.Log("====== New Score updated ======");
		}
	}

	int myScore = 0;
	int opponentScore = 0;
	public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
	{
		if (changedProps.ContainsKey("Score"))
		{
			int score = (int)changedProps["Score"];
			if (targetPlayer.IsLocal)
				myScore = score;
			else
				opponentScore = score;

			UpdateScoreUI(myScore, opponentScore);
		}
	}


	public int GetScore()
	{
		return score;
	}

	public void AddThrows()
	{
		throws++;
	}

	public int GetThrows()
	{
		return throws;
	}

	public void Reset()
	{
		score = 0;
		throws = 0;
		UpdateScoreUI(0,0);
	}
}
