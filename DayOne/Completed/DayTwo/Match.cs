using System.Runtime.CompilerServices;

namespace Puzzles.Completed.DayTwo;
internal class Match
{
	public Move OpponentMove { get; set; }
	public Move MyMove { get; set; }

	public MatchResult Result { get; set; }
	public int ResultScore => (int)MyMove+(int)Result;

	public static Match ParseMatchStringFirst(string matchString)
	{
		var match = new Match();
		
		var moveStrings = matchString.Split(' ');
		match.OpponentMove = MapMoveCharToEnum(char.Parse(moveStrings[0]));
		match.MyMove = MapMoveCharToEnum(char.Parse(moveStrings[1]));

		match.SetMatchResult();

		return match;
	}

	public static Match ParseMatchStringSecond(string matchString)
	{
		var match = new Match();

		var moveStrings = matchString.Split(' ');
		match.OpponentMove = MapMoveCharToEnum(char.Parse(moveStrings[0]));

		match.Result = match.MapReactionCharToEnum(char.Parse(moveStrings[1]));

		match.SetMyMove();

		return match;
	}

	private static Move MapMoveCharToEnum(char moveChar)
	{
		switch(moveChar) {
			case 'A':
			case 'X':
				return Move.Rock;
			case 'B':
			case 'Y':
				return Move.Paper;
			case 'C':
			case 'Z':
				return Move.Scissor;
			default:
				throw new Exception("No valid character");
		}
	}

	private MatchResult MapReactionCharToEnum(char moveChar)
	{
		return moveChar switch
		{
			'X' => MatchResult.Lose,
			'Y' => MatchResult.Draw,
			'Z' => MatchResult.Win,
			_ => throw new Exception("No valid character")
		};
	}

	private void SetMyMove()
	{
		if (Result == MatchResult.Draw)
		{
			MyMove = OpponentMove;
			return;
		}

		switch (Result)
		{
			case MatchResult.Win:
				MyMove = OpponentMove.GetWinnerMove();
				return;
			case MatchResult.Lose:
				MyMove = OpponentMove.GetLoserMove();
				return;
		}
	}

	private void SetMatchResult()
	{
		if (OpponentMove == MyMove)
		{
			Result = MatchResult.Draw;
			return;
		}

		switch(OpponentMove)
		{
			case Move.Rock:
				Result = MyMove == Move.Paper ? MatchResult.Win : MatchResult.Lose;
				return;
			case Move.Paper:
				Result = MyMove == Move.Scissor ? MatchResult.Win : MatchResult.Lose;
				return;
			case Move.Scissor:
				Result = MyMove == Move.Rock ? MatchResult.Win : MatchResult.Lose;
				return;
		}
	}
}

public enum Move
{
	Rock = 1,
	Paper = 2,
	Scissor = 3
}

public enum MatchResult
{
	Lose = 0,
	Draw = 3,
	Win = 6
}
