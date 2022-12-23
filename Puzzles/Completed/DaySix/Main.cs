using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DaySix;

public class Main
{
	const string FOLDER = "DaySix";

	public static async Task SolvePuzzle()
	{
		var fileString = (await InputHandler.GetStringFromInputFile(FOLDER)).Trim();

		char[] chars = new char[14];

		var index = 1;
		foreach (char c in fileString.ToCharArray())
		{
			chars = ShiftIn(chars, c);
			if(HasDistinctItems(chars))
			{
				break;
			}
			index++;
		}

		Console.WriteLine(index);

	}

	public static char[] ShiftIn(char[] charArray, char newChar)
	{
		var shiftArray = new char[14];
		Array.Copy(charArray, 1, shiftArray, 0, 13);
		shiftArray[13] = newChar;
		Console.WriteLine(charArray);
		Console.WriteLine(shiftArray);
		return shiftArray;
	}

	public static bool HasDistinctItems(char[] charArray)
	{
		if(charArray.Contains('\0'))
			return false;

		return charArray.Length == charArray.Distinct().Count();
	}

}