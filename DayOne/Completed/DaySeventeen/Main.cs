using AdventOfCode2022.InputHelper;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

namespace Puzzles.Completed.DaySeventeen;

public class Main
{
    const string FOLDER = "DaySeventeen";

    const int AIR = 0;
    const int ROCK = 1;
    const int FLOOR = 2;

    const int TUNNEL_WIDTH = 7;

    const char LEFT = '<';
    const char RIGHT = '>';
    const char DOWN = 'v';

    const long TARGET_ROCK_COUNT = 1000000000000;
    //const long TARGET_ROCK_COUNT = 2022;

    const bool RUN_SAMPLE = false;

    const long MAX_ROWS_SIZE = 10000000;

    const int PATTERN_ROW_AMOUNT = 1;

    public static async Task SolvePuzzle()
    {
        string input;
        input = (await InputHandler.GetStringFromInputFile(FOLDER)).Trim();

        if (RUN_SAMPLE)
            input = await InputHandler.GetStringFromSample(FOLDER);

        var rocks = CreateRocks();

        var rockSizes = new Dictionary<int, (int w, int h)>();

        for (var i = 0; i < rocks.Length; i++)
        {
            rockSizes.Add(i, GetRockSize(rocks[i]));
            //DrawRock(rocks[i]);
        }

        var currentRockPosition = (x: 2, y: 4);

        var rockRotator = 0;

        long rockCount = 0;

        var rows = new int[MAX_ROWS_SIZE][];
        rows[0] = Enumerable.Repeat(FLOOR, TUNNEL_WIDTH).ToArray();
        rows[1] = Enumerable.Repeat(AIR, TUNNEL_WIDTH).ToArray();
        rows[2] = Enumerable.Repeat(AIR, TUNNEL_WIDTH).ToArray();
        rows[3] = Enumerable.Repeat(AIR, TUNNEL_WIDTH).ToArray();
        rows[4] = Enumerable.Repeat(AIR, TUNNEL_WIDTH).ToArray();

        var moveIndex = 0;

        var highestRock = 0;

        var tunnelMaxIndex = 4;

        var sw = new Stopwatch();


        // Pattern has following identifiers:
        // - The type of Rock as int (index) (Position is always the same)
        // - The index of the input list at creation 
        // - amount? of rows below highRock (in order)

        // Needs comparer (maybe with enough primitives it could work

        var patterns = new (int rock, int moveIndex, long columns, int heightIncrease)[10000];

        var patternIndex = 0;

        var cycleStartIndex = 0;
        var cycleEndIndex = 0;

        sw.Start();

        // Run until cycle found
        while (true)
        {
            moveIndex = moveIndex % input.Length;

            var inputDirection = input[moveIndex];

            // move block gas direction (or dont if blocked)
            var dirMoveResult = TryMoveRock(rows, rocks[rockRotator],
                currentRockPosition, rockSizes[rockRotator].w, inputDirection, highestRock);

            currentRockPosition = (dirMoveResult.x, dirMoveResult.y);

            var moveResult = TryMoveRock(rows, rocks[rockRotator],
                currentRockPosition, rockSizes[rockRotator].w, DOWN, highestRock);

            currentRockPosition = (moveResult.x, moveResult.y);

            // - set fixed
            if (!moveResult.canMove)
            {
                var prevHighestRock = highestRock;

                FixRock(rows, rocks[rockRotator], currentRockPosition);

                // - spawn next block
                highestRock = GetHighestRock(rows, highestRock);

                currentRockPosition.x = 2;
                currentRockPosition.y = highestRock + 4;

                rockCount += 1;

                //rockRotator = rockCount % 5;

                rockRotator = (rockRotator + 1) % 5;

                // try match existing pattern
                var columns = GetColumnHeights(rows, highestRock);

                if (patterns.Any(x => x.rock == rockRotator && x.moveIndex == moveIndex && x.columns == columns))
                {
                    cycleStartIndex = Array.IndexOf(patterns, (rockRotator, moveIndex, columns, highestRock - prevHighestRock));
                    cycleEndIndex = patternIndex;
                    highestRock = prevHighestRock;
                    Console.WriteLine("MATCH");
                    break;
                }

                patterns[patternIndex++] = (rockRotator, moveIndex, columns, highestRock - prevHighestRock);

                // - add height to tunnel
                while (tunnelMaxIndex < currentRockPosition.y + 4)
                {
                    tunnelMaxIndex++;
                    rows[tunnelMaxIndex] = Enumerable.Repeat(AIR, TUNNEL_WIDTH).ToArray();
                }
            }

            moveIndex++;
        }

        sw.Stop();

        long heightValue = highestRock;

        Console.WriteLine(sw.Elapsed.ToString());

        long cycleRocks = cycleEndIndex - cycleStartIndex;
        long cycleHeight = 0;

        for (int i = cycleStartIndex; i < cycleEndIndex; i++)
        {
            cycleHeight += patterns[i].heightIncrease;
        }

        long remaining = TARGET_ROCK_COUNT - rockCount;
        long cycleRuns = (long)Math.Floor((double)(remaining / cycleRocks));

        rockCount += cycleRuns * cycleRocks;
        heightValue += cycleRuns * cycleHeight;

        for (long cycleCounter = 0; rockCount <= TARGET_ROCK_COUNT; rockCount++, cycleCounter++)
        {
            cycleCounter = cycleCounter % (cycleEndIndex - cycleStartIndex);

            heightValue += patterns[cycleCounter + cycleStartIndex].heightIncrease;
            //Console.WriteLine(highestRock);
        }

        Console.WriteLine(heightValue);
    }

    public static void FixRock(int[][] rows, (int x, int y)[] rock, (int x, int y) rockPosition)
    {
        foreach (var rockPart in rock)
        {
            var rockPartPos = (x: rockPosition.x + rockPart.x, y: rockPosition.y + rockPart.y);

            rows[rockPartPos.y][rockPartPos.x] = ROCK;
        }
    }

    public static (bool canMove, int x, int y) TryMoveRock(int[][] rows,
    (int x, int y)[] rock, (int x, int y) rockPosition, int width, char direction, int maxHeight)
    {
        // down move
        if (direction == DOWN)
        {
            if (rockPosition.y > maxHeight + 1)
            {
                return (true, rockPosition.x, rockPosition.y - 1);
            }

            if (rockPosition.y - 1 < 0)
            {
                return (false, rockPosition.x, rockPosition.y);
            }

            foreach (var rockPart in rock)
            {
                // get real rockpart pos
                var rockPartPos = (x: rockPosition.x + rockPart.x, y: rockPosition.y + rockPart.y);

                // get check rockpart against collision
                if (rows[rockPartPos.y - 1][rockPartPos.x] != AIR)
                {
                    return (false, rockPosition.x, rockPosition.y);
                }
            }
            return (true, rockPosition.x, rockPosition.y - 1);
        }

        // left move
        if (direction == LEFT)
        {
            if (rockPosition.x <= 0)
            {
                return (false, rockPosition.x, rockPosition.y);
            }

            foreach (var rockPart in rock)
            {
                // get real rockpart pos
                var rockPartPos = (x: rockPosition.x + rockPart.x, y: rockPosition.y + rockPart.y);

                // get check rockpart against collision
                if (rows[rockPartPos.y][rockPartPos.x - 1] != AIR)
                {
                    return (false, rockPosition.x, rockPosition.y);
                }
            }
            return (true, rockPosition.x - 1, rockPosition.y);
        }

        // right move
        if (direction == RIGHT)
        {
            if (rockPosition.x + width >= TUNNEL_WIDTH)
            {
                return (false, rockPosition.x, rockPosition.y);
            }

            foreach (var rockPart in rock)
            {
                // get real rockpart pos
                var rockPartPos = (x: rockPosition.x + rockPart.x, y: rockPosition.y + rockPart.y);

                // get check rockpart against collision
                if (rows[rockPartPos.y][rockPartPos.x + 1] != AIR)
                {
                    return (false, rockPosition.x, rockPosition.y);
                }
            }
            return (true, rockPosition.x + 1, rockPosition.y);
        }
        return (false, rockPosition.x, rockPosition.y);
    }

    public static int GetHighestRock(int[][] rows, int previousHighest)
    {
        for (var i = previousHighest + 4; i > 0; i--)
        {
            if (rows[i].Any(r => r != AIR))
            {
                return i;
            }
        }
        return 0;
    }

    public static int GetClosedRow(int[][] rows, int highestRock)
    {
        var closingColumns = new bool[7];
        for (var i = highestRock; i > 0; i--)
        {
            if (closingColumns.All(r => r))
            {
                return i;
            }

            // OR 
            for (var j = 0; j < 7; j++)
            {
                if (closingColumns[j] == false && rows[i][j] != AIR) closingColumns[j] = true;
            }


        }
        return 0;
    }

    public static long GetColumnHeights(int[][] rows, int highestRock)
    {
        var columnHeights = new int[7];
        for (var c = 0; c < 7; c++)
        {
            for (var i = highestRock; i > 0; i--)
            {
                if (rows[i][c] != 0)
                {
                    columnHeights[c] = highestRock - i;
                    break;
                }
            }
        }
        return long.Parse(string.Join(string.Empty, columnHeights));
    }


    public static bool IsPatternExisting(int rock, int moveIndex, int[] columns,
        List<(int rock, int moveIndex, int[] columns)> patterns)
    {
        return false;
    }

    public static (int x, int y)[][] CreateRocks()
    {
        var rocks = new (int x, int y)[5][];

        // Create lying rock
        rocks[0] = new (int x, int y)[4];

        rocks[0][0] = (0, 0);
        rocks[0][1] = (1, 0);
        rocks[0][2] = (2, 0);
        rocks[0][3] = (3, 0);

        // Create plus rock
        rocks[1] = new (int x, int y)[5];

        rocks[1][0] = (1, 0);
        rocks[1][1] = (0, 1);
        rocks[1][2] = (1, 1);
        rocks[1][3] = (2, 1);
        rocks[1][4] = (1, 2);

        // Create l rock
        rocks[2] = new (int x, int y)[5];

        rocks[2][0] = (2, 2);
        rocks[2][1] = (2, 1);
        rocks[2][2] = (0, 0);
        rocks[2][3] = (1, 0);
        rocks[2][4] = (2, 0);

        // Create standing rock
        rocks[3] = new (int x, int y)[4];

        rocks[3][0] = (0, 0);
        rocks[3][1] = (0, 1);
        rocks[3][2] = (0, 2);
        rocks[3][3] = (0, 3);

        // Create square rock
        rocks[4] = new (int x, int y)[4];

        rocks[4][0] = (0, 0);
        rocks[4][1] = (0, 1);
        rocks[4][2] = (1, 0);
        rocks[4][3] = (1, 1);

        return rocks;
    }

    public static void DrawRock((int x, int y)[] rock)
    {
        var rockSize = GetRockSize(rock);

        for (var y = rockSize.height - 1; y >= 0; y--)
        {
            for (var x = 0; x < rockSize.width; x++)
            {
                Console.Write(rock.Any(r => r.x == x && r.y == y) ? '#' : '.');
            }
            Console.WriteLine();
        }
    }

    public static void DrawTunnel(int[][] rows, int maxIndex, (int x, int y)[] rock, (int x, int y) rockPosition)
    {
        for (var y = maxIndex; y >= 0; y--)
        {
            for (var x = 0; x < 7; x++)
            {
                var item = rows[y][x];

                switch (rows[y][x])
                {
                    case AIR:
                        Console.Write('.');
                        break;
                    case ROCK:
                        Console.Write('#');
                        break;
                    case FLOOR:
                        Console.Write('=');
                        break;
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine(maxIndex);
        Console.WriteLine();
    }

    public static (int width, int height) GetRockSize((int x, int y)[] rock)
    {
        var width = rock.Max(r => r.x) + 1;
        var height = rock.Max(r => r.y) + 1;

        return (width, height);
    }
}