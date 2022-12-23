using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayTwentythree;

public class Main
{
    const string FOLDER = "DayTwentythree";

    const bool USE_SAMPLE = false;

    public static HashSet<(int x, int y)> Elves = new();

    public static Queue<(int x1, int y1, int x2, int y2)> ConsideredMoves = new();

    const int NORTH = 0;
    const int SOUTH = 1;
    const int WEST = 2;
    const int EAST = 3;

    private static int _cDirection;

    public static int CurrentDirection
    {
        get => _cDirection;
        set => _cDirection = value % 4;
    }

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));
        if (USE_SAMPLE)
            inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSample(FOLDER));
        //inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSampleSmall(FOLDER));


        ParseInput(inputStrings);
        Console.WriteLine();
        Print();
        var i = 1;
        while (ExecuteRound() == false)
        {
            Print();
            i++;
        }
        Console.WriteLine(i);

    }

    public static int GetFreeFields()
    {
        var y1 = Elves.MinBy(e => e.y).y;
        var y2 = Elves.MaxBy(e => e.y).y;
        var h = Math.Abs(y1 - y2) + 1;

        var x1 = Elves.MinBy(e => e.x).x;
        var x2 = Elves.MaxBy(e => e.x).x;
        var w = Math.Abs(x1 - x2) + 1;

        var eC = Elves.Count;

        return w * h - eC;
    }

    public static bool ExecuteRound()
    {
        foreach (var elf in Elves)
        {
            var blockedDirection = GetElfBlockedDirections(elf);
            if (blockedDirection.All(b => b) || blockedDirection.All(b => !b))
            {
                continue;
            }
            var d = CurrentDirection;
            for (var c = 0; c < 4; c++, d = (d + 1) % 4)
            {
                if (blockedDirection[d]) continue;
                AddConsideredMove(elf, d);
                break;
            }
        }
        DiscardSameTargetMoves();
        if (ConsideredMoves.Count == 0)
            return true;
        MoveElves();

        CurrentDirection += 1;
        return false;
    }

    public static void MoveElves()
    {
        while (ConsideredMoves.TryDequeue(out var move))
        {
            var t = Elves.Remove((move.x1, move.y1));
            var f = Elves.Add((move.x2, move.y2));
        }
    }

    public static void DiscardSameTargetMoves()
    {
        var query = ConsideredMoves.GroupBy(m => new { m.x2, m.y2 })
              .Where(g => g.Count() == 1)
              .SelectMany(g => g)
              .ToList();

        ConsideredMoves = new Queue<(int x1, int y1, int x2, int y2)>(query);
    }

    public static void AddConsideredMove((int x, int y) elf, int direction)
    {
        switch (direction)
        {
            case NORTH:
                ConsideredMoves.Enqueue((elf.x, elf.y, elf.x, elf.y - 1));
                return;
            case SOUTH:
                ConsideredMoves.Enqueue((elf.x, elf.y, elf.x, elf.y + 1));
                return;
            case WEST:
                ConsideredMoves.Enqueue((elf.x, elf.y, elf.x - 1, elf.y));
                return;
            case EAST:
                ConsideredMoves.Enqueue((elf.x, elf.y, elf.x + 1, elf.y));
                return;
            default: return;
        }
    }

    public static bool[] GetElfBlockedDirections((int x, int y) elf)
    {
        bool[] blockedDirections = new bool[4];
        var nElves = Elves.Where(e => e.y >= elf.y - 1 && e.y <= elf.y + 1
            && e.x >= elf.x - 1 && e.x <= elf.x + 1);
        if (nElves.Any(e => e.y < elf.y))
        {
            blockedDirections[NORTH] = true;
        }
        if (nElves.Any(e => e.y > elf.y))
        {
            blockedDirections[SOUTH] = true;
        }
        if (nElves.Any(e => e.x < elf.x))
        {
            blockedDirections[WEST] = true;
        }
        if (nElves.Any(e => e.x > elf.x))
        {
            blockedDirections[EAST] = true;
        }
        return blockedDirections;
    }

    public static void ParseInput(string[] inputStrings)
    {
        for (var y = 0; y < inputStrings.Length; y++)
        {
            for (var x = 0; x < inputStrings[y].Length; x++)
            {
                if (inputStrings[y][x] == '#')
                    Elves.Add((x, y));
            }
        }
    }

    public static void Print()
    {
        Console.SetCursorPosition(0, 0);
        for (var i = Elves.MinBy(e => e.y).y; i <= Elves.MaxBy(e => e.y).y; i++)
        {
            for (var j = Elves.MinBy(e => e.x).x; j <= Elves.MaxBy(e => e.x).x; j++)
            {
                if (Elves.TryGetValue((j, i), out _))
                {
                    Console.Write("#");
                    continue;
                }
                Console.Write(".");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}