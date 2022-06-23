using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public enum TicTacToeState{none, cross, circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}

public class TicTacToeAI : MonoBehaviour
{

	int _aiLevel;

	TicTacToeState[,] boardState;

	[SerializeField]
	private static bool _isPlayerTurn;

	[SerializeField]
	private int _gridSize = 3;
	
	[SerializeField]
	private TicTacToeState playerState = TicTacToeState.circle;
	TicTacToeState aiState = TicTacToeState.cross;

	[SerializeField]
	private GameObject _xPrefab;

	[SerializeField]
	private GameObject _oPrefab;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _triggers;
	static List<ClickTrigger> _triggerVals;
	static ClickTrigger[,] _triggerPlayer;
	static ClickTrigger[,] _triggerAI;
	
	public void OnPlayerWin(int winner)
    {
		onPlayerWin.Invoke(winner);
	}

	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
	}

	public void StartAI(int AILevel){
		_aiLevel = AILevel;
		StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
		_triggerVals.Add(clickTrigger);
	}

	private void StartGame()
	{
		PlayerTurn(true);
		_triggers = new ClickTrigger[3,3];
		_triggerVals = new List<ClickTrigger>();
		_triggerPlayer = new ClickTrigger[3, 3];
		_triggerAI = new ClickTrigger[3, 3];
		onGameStarted.Invoke();
	}

	public bool PlayerSelects(int coordX, int coordY){
		if (IsTriggerInListAndRemove(coordX, coordY)) 
        {
			SetVisual(coordX, coordY, playerState);
			
			return true;
		}
		else
        {
			return false;
        }
	}

	public void AiSelects(int coordX, int coordY){
		// if this wasnt a test for this program i would code this so it JUST removed the value, instead of using this funtion in the if() block
		// but I'm going to brute force it here because it'd be annoying to refactor. hope you don't mind.
		if (IsTriggerInListAndRemove(coordX, coordY))
        {
			SetVisual(coordX, coordY, aiState);
		}
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
	}

	public bool IsTriggerInListAndRemove(int coordX, int coordY)
    {
		for (int i = 0; i < _triggerVals.Count; i++)
        {
			if (_triggerVals[i].HasXAndYCoord(coordX, coordY))
            {
				if (_isPlayerTurn)
                {
					_triggerPlayer[coordX, coordY] = _triggerVals[i];
				}
				else
                {
					_triggerAI[coordX, coordY] = _triggerVals[i];
				}
				RemoveTrigger(_triggerVals[i]);
				return true;
            }
        }

		return false;
    }

	public static ClickTrigger IsTriggerInList(int coordX, int coordY)
	{
		for (int i = 0; i < _triggerVals.Count; i++)
		{
			if (_triggerVals[i].HasXAndYCoord(coordX, coordY))
			{
				return _triggerVals[i];
			}
		}

		return null;
	}

	public void RemoveTrigger(ClickTrigger ct)
    {
		_triggerVals.Remove(ct);
	}

	public static bool IsPlayerTurn()
    {
		return _isPlayerTurn;
    }

	public static void PlayerTurn(bool isPlayerTurn)
    {
		_isPlayerTurn = isPlayerTurn;
    }

	public static List<ClickTrigger> GetEmptySpots()
    {
		return _triggerVals;
	}

	public static int? CheckForEndState()
    {
		if (Win())
        {
			return 1;
        }
        else if (_triggerVals.Count == 0)
        {
			return -1;
        }

		return null;
    }

	public static bool Win()
    {
		if (( _triggerPlayer[0,2] != null && _triggerPlayer[0, 1] != null && _triggerPlayer[0, 0] != null) ||
			(_triggerPlayer[1, 2] != null && _triggerPlayer[1, 1] != null && _triggerPlayer[1, 0] != null) ||
			(_triggerPlayer[2, 2] != null && _triggerPlayer[2, 1] != null && _triggerPlayer[2, 0] != null) ||
			(_triggerPlayer[2, 2] != null && _triggerPlayer[1, 2] != null && _triggerPlayer[0, 2] != null) ||
			(_triggerPlayer[2, 1] != null && _triggerPlayer[1, 1] != null && _triggerPlayer[0, 1] != null) ||
			(_triggerPlayer[2, 0] != null && _triggerPlayer[1, 0] != null && _triggerPlayer[0, 0] != null) ||
			(_triggerPlayer[2, 2] != null && _triggerPlayer[1, 1] != null && _triggerPlayer[0, 0] != null) ||
			(_triggerPlayer[0, 2] != null && _triggerPlayer[0, 1] != null && _triggerPlayer[0, 0] != null))
        {
			return true;
        }

		return false;
    }

	public static List<int> GetAIMove()
    {
		List<int> aiMove = MoveAlgorithm();

		return aiMove;
	}

	public static List<int> MoveAlgorithm()
    {
		// very dumb, brute force way of doing this. don't want to code recursion rn. sorry!
		List<int> move = new List<int>();

		// left column
		if (_triggerPlayer[2, 2] != null && _triggerPlayer[1, 2] != null
			&& _triggerAI[0, 2] == null)
        {
			move.Add(0);
			move.Add(2);
        }
		else if (_triggerPlayer[2, 2] != null && _triggerPlayer[0, 2] != null
			&& _triggerAI[1, 2] == null)
        {
			move.Add(1);
			move.Add(2);
        }
		else if (_triggerPlayer[1, 2] != null && _triggerPlayer[0, 2] != null
			&& _triggerAI[2, 2] == null)
		{
			move.Add(2);
			move.Add(2);
		}

		// middle column
		else if (_triggerPlayer[2, 1] != null && _triggerPlayer[1, 1] != null
			&& _triggerAI[0, 1] == null)
		{
			move.Add(0);
			move.Add(1);
		}
		else if (_triggerPlayer[2, 1] != null && _triggerPlayer[0, 1] != null
			&& _triggerAI[1, 1] == null)
		{
			move.Add(1);
			move.Add(1);
		}
		else if (_triggerPlayer[1, 1] != null && _triggerPlayer[0, 1] != null
			&& _triggerAI[2, 1] == null)
		{
			move.Add(2);
			move.Add(1);
		}

		// right column
		else if (_triggerPlayer[2, 0] != null && _triggerPlayer[1, 0] != null
			&& _triggerAI[0, 0] == null)
		{
			move.Add(0);
			move.Add(0);
		}
		else if (_triggerPlayer[2, 0] != null && _triggerPlayer[0, 0] != null
			&& _triggerAI[1, 0] == null)
		{
			move.Add(1);
			move.Add(0);
		}
		else if (_triggerPlayer[1, 0] != null && _triggerPlayer[0, 0] != null
			&& _triggerAI[2, 0] == null)
		{
			move.Add(2);
			move.Add(0);
		}

		// top row
		else if (_triggerPlayer[2, 2] != null && _triggerPlayer[2, 1] != null
			&& _triggerAI[2, 0] == null)
		{
			move.Add(2);
			move.Add(0);
		}
		else if (_triggerPlayer[2, 2] != null && _triggerPlayer[2, 0] != null
			&& _triggerAI[2, 1] == null)
		{
			move.Add(2);
			move.Add(1);
		}
		else if (_triggerPlayer[2, 1] != null && _triggerPlayer[2, 0] != null
			&& _triggerAI[2, 2] == null)
		{
			move.Add(2);
			move.Add(2);
		}

		// middle row
		else if (_triggerPlayer[1, 2] != null && _triggerPlayer[1, 1] != null
			&& _triggerAI[1, 0] == null)
		{
			move.Add(1);
			move.Add(0);
		}
		else if (_triggerPlayer[1, 2] != null && _triggerPlayer[1, 0] != null
			&& _triggerAI[1, 1] == null)
		{
			move.Add(1);
			move.Add(1);
		}
		else if (_triggerPlayer[1, 1] != null && _triggerPlayer[1, 0] != null
			&& _triggerAI[1, 2] == null)
		{
			move.Add(1);
			move.Add(2);
		}

		// bottom row
		else if (_triggerPlayer[0, 2] != null && _triggerPlayer[0, 1] != null
			&& _triggerAI[0, 0] == null)
		{
			move.Add(0);
			move.Add(0);
		}
		else if (_triggerPlayer[0, 2] != null && _triggerPlayer[0, 0] != null
			&& _triggerAI[0, 1] == null)
		{
			move.Add(0);
			move.Add(1);
		}
		else if (_triggerPlayer[0, 1] != null && _triggerPlayer[0, 0] != null
			&& _triggerAI[0, 2] == null)
		{
			move.Add(0);
			move.Add(2);
		}

		// top left to bottom right diagonal
		else if (_triggerPlayer[2, 2] != null && _triggerPlayer[1, 1] != null
			&& _triggerAI[0, 0] == null)
		{
			move.Add(0);
			move.Add(0);
		}
		else if (_triggerPlayer[2, 2] != null && _triggerPlayer[0, 0] != null
			&& _triggerAI[1, 1] == null)
		{
			move.Add(1);
			move.Add(1);
		}
		else if (_triggerPlayer[1, 1] != null && _triggerPlayer[0, 0] != null
			&& _triggerAI[2, 2] == null)
		{
			move.Add(2);
			move.Add(2);
		}

		// bottom left to top right diagonal
		else if (_triggerPlayer[0, 2] != null && _triggerPlayer[1, 1] != null
			&& _triggerAI[2, 0] == null)
		{
			move.Add(2);
			move.Add(0);
		}
		else if (_triggerPlayer[0, 2] != null && _triggerPlayer[2, 0] != null
			&& _triggerAI[1, 1] == null)
		{
			move.Add(1);
			move.Add(1);
		}
		else if (_triggerPlayer[1, 1] != null && _triggerPlayer[2, 0] != null
			&& _triggerAI[0, 2] == null)
		{
			move.Add(0);
			move.Add(2);
		}

		// only execute this block if there wasn't a 2-pair with an imminent three in a row
		if (move.Count == 0)
        {
			if (_triggerPlayer[1, 1] == null
				&& _triggerAI[1, 1] == null)
            {
				move.Add(1);
				move.Add(1);

				return move;
            }
			else
            {
				move.Add(_triggerVals.First().ReturnX());
				move.Add(_triggerVals.First().ReturnY());

				return move;
			}
		}

		return move;
	}
}
