using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayTwo;

public class Main
{
	const string FOLDER = "DayTwo";

	public static async Task SolvePuzzle()
	{
		var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));

		var endResult = 0;

		foreach(var matchString in inputStrings)
		{
			//endResult += Match.ParseMatchStringFirst(matchString).ResultScore;
			endResult += Match.ParseMatchStringSecond(matchString).ResultScore;
		}
		Console.WriteLine(endResult);
	}
}