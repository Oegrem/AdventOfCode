using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayThree;

public class Main
{
	const string FOLDER = "DayThree";

	public static async Task SolvePuzzle()
	{
		var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));

		var sharedItemResult = 0;
		var badgeItemResult = 0;
		var index = 1;
		var elfGroup = new ElfGroup();
		foreach(var inputString in inputStrings)
		{
			var ruck = new Rucksack(inputString);
			if(ruck.SharedItemType != null)
			{
				sharedItemResult += ruck.SharedItemType.Priority;
			}
			elfGroup.AddRucksack(ruck);
			if(index%3 == 0)
			{
				badgeItemResult += elfGroup.GetBadge().Priority;
				elfGroup = new ElfGroup();
			}
			index++;
		}

		Console.WriteLine(badgeItemResult);
	}
}