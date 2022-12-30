using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayTwentyfive
    ;

public class Main
{
    const string FOLDER = "DayTwentyfive";

    const bool USE_SAMPLE = false;
    const bool USE_SAMPLE_SMALL = false;

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));
        if (USE_SAMPLE)
            inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSample(FOLDER));
        if (USE_SAMPLE_SMALL)
            inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSampleSmall(FOLDER));


        long n = 0;
        foreach (var s in inputStrings)
        {
            n += SnafuNumber.GetLongFromSnafu(s);
        }
        Console.WriteLine(n);
        Console.WriteLine(SnafuNumber.GetSnafuFromNumber(n));

    }


}