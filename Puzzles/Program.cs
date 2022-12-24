namespace Puzzles;

class Program
{
	static async Task Main(string[] args)
	{
		switch(int.Parse(args[0]))
		{
			case 1:
				await Completed.DayOne.Main.SolvePuzzle();
				break;
			case 2:
				await Completed.DayTwo.Main.SolvePuzzle();
				break;
			case 3:
				await Completed.DayThree.Main.SolvePuzzle();
				break;
			case 4:
				await Completed.DayFour.Main.SolvePuzzle();
				break;
			case 5:
				await Completed.DayFive.Main.SolvePuzzle();
				break;
			case 6:
				await Completed.DaySix.Main.SolvePuzzle();
				break;
			case 7:
				await Completed.DaySeven.Main.SolvePuzzle();
				break;
			case 8:
				await Completed.DayEight.Main.SolvePuzzle();
				break;
			case 9:
				await Completed.DayNine.Main.SolvePuzzle();
				break;
			case 10:
				await Completed.DayTen.Main.SolvePuzzle();
				break;
			case 11:
				await Completed.DayEleven.Main.SolvePuzzle();
				break;
			case 12:
				await Completed.DayTwelve.Main.SolvePuzzle();
				break;
			case 13:
				await Completed.DayThirteen.Main.SolvePuzzle();
				break;
			case 14:
				await Completed.DayFourteen.Main.SolvePuzzle();
				break;
			case 15:
				await Completed.DayFifteen.Main.SolvePuzzle();
				break;
			case 16:
				await Completed.DaySixteen.Main.SolvePuzzle();
				break;
			case 17:
				await Completed.DaySeventeen.Main.SolvePuzzle();
				break;
			case 18:
				await Completed.DayEighteen.Main.SolvePuzzle();
				break;
			case 19:
				await Completed.DayNineteen.Main.SolvePuzzle();
				break;
			case 20:
				await Completed.DayTwenty.Main.SolvePuzzle();
				break;
			case 21:
				await Completed.DayTwentyone.Main.SolvePuzzle();
				break;
			case 22:
				await Completed.DayTwentytwo.Main.SolvePuzzle();
				break;
			case 23:
				await Completed.DayTwentythree.Main.SolvePuzzle();
				break;
			case 24:
				await Completed.DayTwentyfour.Main.SolvePuzzle();
				break;
			case 25:
				await DayTwentyfive.Main.SolvePuzzle();
				break;
			default:
				break;

		}
	}
}