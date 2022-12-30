namespace Puzzles.Completed.DayTwo;
static class EnumExtensions
{
	public static Move GetWinnerMove(this Move opponentMove)
	{
		return opponentMove switch
		{
			Move.Rock => Move.Paper,
			Move.Paper => Move.Scissor,
			Move.Scissor => Move.Rock,
			_ => Move.Rock
		} ;
	}

	public static Move GetLoserMove(this Move opponentMove)
	{
		return opponentMove switch
		{
			Move.Rock => Move.Scissor,
			Move.Paper => Move.Rock,
			Move.Scissor => Move.Paper,
			_ => Move.Rock
		};
	}
}
