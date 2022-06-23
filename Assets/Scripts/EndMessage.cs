using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndMessage : MonoBehaviour
{

	[SerializeField]
	private static TMP_Text _playerMessage = null;

	public static void OnGameEnded(int winner)
	{
		_playerMessage.text = winner == -1 ? "Tie" : winner == 1 ? "AI wins" : "Player wins";
	}
}
