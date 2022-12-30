using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayFour;

public class Main
{
    const string FOLDER = "DayFour";

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));

        int resultContained = 0;
        int resultOverlapped = 0;

        foreach (var inputString in inputStrings)
        {
            if (IsFullyContained(inputString))
            {
                resultContained++;
            }

            if (IsOverlapped(inputString))
            {
                resultOverlapped++;
            }
        }

        Console.WriteLine(resultOverlapped);
    }

    public static bool IsFullyContained(string assignmentString)
    {
        var assignmentStrings = assignmentString.Split(',');

        var leftAssignment = new Range(assignmentStrings[0]);
        var rightAssignment = new Range(assignmentStrings[1]);

        return Range.IsContained(leftAssignment, rightAssignment);
    }

    public static bool IsOverlapped(string assignmentString)
    {
        var assignmentStrings = assignmentString.Split(',');

        var leftAssignment = new Range(assignmentStrings[0]);
        var rightAssignment = new Range(assignmentStrings[1]);

        return Range.IsOverlapping(leftAssignment, rightAssignment);
    }
}

public class Range
{
    public int Start { get; set; }

    public int End { get; set; }

    public Range(int start, int end)
    {
        Start = start;
        End = end;
    }

    public Range(string inputString)
    {
        var nums = inputString.Split("-");
        Start = int.Parse(nums[0]);
        End = int.Parse(nums[1]);
    }

    public static bool IsContained(Range firstRange, Range secondRange)
    {
        if (firstRange.Start <= secondRange.Start && firstRange.End >= secondRange.End)
        {
            return true;
        }
        if (secondRange.Start <= firstRange.Start && secondRange.End >= firstRange.End)
        {
            return true;
        }
        return false;
    }

    public static bool IsOverlapping(Range firstRange, Range secondRange)
    {
        return !(firstRange.End < secondRange.Start || secondRange.End < firstRange.Start);
    }
}