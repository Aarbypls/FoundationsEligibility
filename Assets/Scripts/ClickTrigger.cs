using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTrigger : MonoBehaviour
{
	TicTacToeAI _ai;

	[SerializeField]
	private int _myCoordX = 0;
	[SerializeField]
	private int _myCoordY = 0;

	[SerializeField]
	private bool canClick;

	private void Awake()
	{
		_ai = FindObjectOfType<TicTacToeAI>();
	}

	private void Start(){

		_ai.onGameStarted.AddListener(AddReference);
		_ai.onGameStarted.AddListener(() => SetInputEndabled(true));
		_ai.onPlayerWin.AddListener((win) => SetInputEndabled(false));
	}

	private void SetInputEndabled(bool val){
		canClick = val;
	}

	private void AddReference()
	{
		_ai.RegisterTransform(_myCoordX, _myCoordY, this);
		canClick = true;
	}

	private void OnMouseDown()
	{
		if(canClick && TicTacToeAI.IsPlayerTurn()){
			bool validSelection = _ai.PlayerSelects(_myCoordX, _myCoordY);

			int? winner = TicTacToeAI.CheckForEndState();
			if (winner == null)
            {
				if (validSelection)
				{
					TicTacToeAI.PlayerTurn(false);
					// Where the enemy AI goes
					StartCoroutine(WaitForAIMove());
				}
			}
			else
            {
				_ai.OnPlayerWin(Convert.ToInt32(winner));
			}
		}
	}

	private IEnumerator WaitForAIMove()
    {
		yield return new WaitForSeconds(2);

		List<int> aiSelection = TicTacToeAI.GetAIMove();
		_ai.AiSelects(aiSelection[0], aiSelection[1]);

		int? winner = TicTacToeAI.CheckForEndState();

		if (winner != null)
        {
			_ai.OnPlayerWin(Convert.ToInt32(winner));
        }
		else
        {
			TicTacToeAI.PlayerTurn(true);
		}
	}

	public bool HasXAndYCoord(int x, int y)
    {
		return (x == _myCoordX && y == _myCoordY);
    }

	public int ReturnX()
    {
		int x = _myCoordX;
		return x;

	}

	public int ReturnY()
	{
		int y = _myCoordY;
		return y;

	}
}
