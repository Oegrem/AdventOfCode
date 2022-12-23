using AdventOfCode2022.InputHelper;

namespace Puzzles.DayTwentyfour;

public class Main
{
	const string FOLDER = "DayTwentyfour";

	const bool USE_SAMPLE = true;

	public static async Task SolvePuzzle()
	{
		var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));
		if( USE_SAMPLE)
			inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSample(FOLDER));

	}
}