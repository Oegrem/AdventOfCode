using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayFive;

public class Main
{
    const string FOLDER = "DayFive";

    public static async Task SolvePuzzle()
    {
        var fileStrings = (await InputHandler.GetStringFromInputFile(FOLDER)).Split("\n\n");

        Console.WriteLine(fileStrings[0]);
        // initialize CrateStacks
        ConvertStartStringToCrateStacks(fileStrings[0]);

        foreach (var inputString in InputHandler.GetStringLines(fileStrings[1]))
        {
            CrateStack.MoveCrateBetter(CrateMove.ParseCrateMove(inputString));
        }

        Console.WriteLine(CrateStack.GetCurrentTopWord());
    }


    public static void ConvertStartStringToCrateStacks(string startString)
    {
        var stringLines = startString.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        foreach (var stringLine in stringLines.Reverse().Skip(1))
        {
            for (int i = 0, id = 1; i < stringLine.Length; i += 4, id++)
            {
                if (CrateStack.GetStackById(id) == null)
                    CrateStack.CreateCrateStack(id);

                var crateString = stringLine.Substring(i, Math.Min(4, stringLine.Length - i)).Trim();
                if (!string.IsNullOrWhiteSpace(crateString))
                {
                    var crateChar = char.Parse(crateString
                        .Replace("[", string.Empty).Replace("]", string.Empty));
                    var crateStack = CrateStack.GetStackById(id);
                    if (crateStack != null)
                        crateStack.Crates.Push(crateChar);
                }
            }
        }
    }

}

public class CrateStack
{
    public static List<CrateStack> CrateStacks = new List<CrateStack>();

    public int Id = 0;

    public Stack<char> Crates = new Stack<char>();

    public CrateStack(int id)
    {
        Id = id;
    }

    public static void CreateCrateStack(int id)
    {
        CrateStacks.Add(new CrateStack(id));
    }

    public static string GetCurrentTopWord()
    {
        var word = string.Empty;

        var maxId = CrateStacks.Max(x => x.Id);
        for (var i = 1; i <= maxId; i++)
        {
            word += GetStackById(i).Crates.Peek();
        }

        return word;
    }

    public static CrateStack? GetStackById(int id)
    {
        return CrateStacks.FirstOrDefault(c => c.Id == id);
    }

    public static void MoveCrateBetter(CrateMove crateMove)
    {
        MoveCrateBetter(crateMove.Source, crateMove.Target, crateMove.Moves);
    }

    public static void MoveCrateBetter(int source, int target, int moves = 1)
    {
        var sourceStack = GetStackById(source);
        var targetStack = GetStackById(target);
        if (targetStack == null || sourceStack == null)
            return;

        var tempStack = new Stack<char>();

        for (int i = 0; i < moves; i++)
        {
            tempStack.Push(sourceStack.Crates.Pop());
        }

        for (int i = 0; i < moves; i++)
        {
            targetStack.Crates.Push(tempStack.Pop());
        }
    }

    public static void MoveCrate(CrateMove crateMove)
    {
        MoveCrate(crateMove.Source, crateMove.Target, crateMove.Moves);
    }

    public static void MoveCrate(int source, int target, int moves = 1)
    {
        var sourceStack = GetStackById(source);
        var targetStack = GetStackById(target);
        if (targetStack == null || sourceStack == null)
            return;
        for (int i = 0; i < moves; i++)
        {
            targetStack.Crates.Push(sourceStack.Crates.Pop());
        }
    }
}

public class CrateMove
{
    public string Predicate = string.Empty;

    public int Source;
    public int Target;
    public int Moves;

    public static CrateMove ParseCrateMove(string predicate)
    {
        var predicateParts = predicate.Split(' ');

        return new CrateMove
        {
            Predicate = predicate,
            Moves = int.Parse(predicateParts[1]),
            Source = int.Parse(predicateParts[3]),
            Target = int.Parse(predicateParts[5])
        };
    }
}