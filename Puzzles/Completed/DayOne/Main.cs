using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayOne;

public class Main
{
	const string FOLDER = "DayOne";

	public static async Task SolvePuzzle()
	{
		var inputText = await InputHandler.GetStringFromInputFile(FOLDER);
		Console.WriteLine(inputText);

		var elves = CreateElves(inputText);
		Console.WriteLine(elves);

		var maxCalories = GetTopCalory(elves, 3);
		Console.Write(maxCalories);

	}

	static List<Elf> CreateElves(string caloryText)
	{
		var elves = new List<Elf>();
		int id = 0;

		foreach(string elfCalories in caloryText.Split("\n\n"))
		{
			var newElf = new Elf(id++);
			foreach(string elfCalory in elfCalories.Split("\n")) {
				if(elfCalory != String.Empty) { 
					newElf.AddCalory(int.Parse(elfCalory));
				}
			}
			elves.Add(newElf);
		}

		return elves;
	}

	static int GetTopCalory(List<Elf> elves, int elfAmount)
	{
		elves.Sort();
		var topElves = elves.Take(elfAmount);
		Console.WriteLine(topElves);
		return topElves.Sum(elf => elf.CalorySum);
	}
}