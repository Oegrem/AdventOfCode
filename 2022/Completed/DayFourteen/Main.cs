using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayFourteen;

public class Main
{
    const string FOLDER = "DayFourteen";

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));

        //var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSample(FOLDER));

        var cave = new Cave();
        foreach (var inputString in inputStrings)
        {
            cave.ParseObstacles(inputString);
        }

        cave.Draw();
        Console.WriteLine();
        cave.DropSandUntilItRunsOver(500, 0);
        //for(var i = 0; i < 100; i++)
        //{
        //cave.DropSand(500, 0);
        //	cave.Draw();
        //	Console.WriteLine();

        //}
        cave.Draw();
        Console.WriteLine();

        Console.WriteLine(cave.DroppedSand);

    }
}

public class Cave
{
    public List<Obstacle> obstacles = new List<Obstacle>();

    public int[][] obstacleArray = new int[300][];

    public int FloorY = 0;

    public int DroppedSand = 0;

    public Cave()
    {
        for (var i = 0; i < 300; i++)
        {
            obstacleArray[i] = new int[1000];
        }
    }

    public void ParseObstacles(string obstacleString)
    {
        var obstacleCorners = new List<Obstacle>();
        var obstacleCornerStrings = obstacleString.Split(" -> ");

        foreach (var obstacleCornerString in obstacleCornerStrings)
        {

            var xyString = obstacleCornerString.Split(",");
            var x = int.Parse(xyString[0]);
            var y = int.Parse(xyString[1]);

            var newCornerObstacle = new Obstacle(x, y);
            obstacleCorners.Add(newCornerObstacle);

            obstacles.Add(newCornerObstacle);
            obstacleArray[y][x] = 1;
        }

        FillEdges(obstacleCorners);

        FloorY = obstacles.Max(obs => obs.y) + 2;
    }

    public void DropSandUntilItRunsOver(int x, int y)
    {
        while (DropSand(x, y))
        {
            DroppedSand++;
        }
    }

    public bool DropSand(int x, int y)
    {
        return SetNewObstaclePosition(x, y);
    }

    public bool SetNewObstaclePosition(int x, int y)
    {
        var nextY = GetNextObstacleY(x, y);
        if (nextY == -1)
        {
            obstacleArray[FloorY - 1][x] = 2;
            return true;
        }

        // Decide direction
        // if blocked => return;
        // if not:
        // Change x and y
        // Recursively call 
        var nextDirection = GetDirection(x, nextY);

        // sand is finished
        if (nextDirection == Direction.Blocked)
        {
            if (x == 500 && nextY == 1)
            {
                obstacleArray[nextY - 1][x] = 2;
                DroppedSand++;
                return false;
            }
            // add sand as obstacle
            obstacleArray[nextY - 1][x] = 2;
            return true;
        }

        return SetNewObstaclePosition(x + (int)nextDirection, nextY);
    }

    public int GetNextObstacleY(int x, int y)
    {
        var obstacle = 0;
        var yDiff = 0;
        while (obstacle == 0)
        {
            yDiff++;

            if (y + yDiff > FloorY)
            {
                return -1;
            }

            obstacle = obstacleArray[y + yDiff][x];
        }
        return y + yDiff;
    }

    public Direction GetDirection(int x, int y)
    {
        if (obstacleArray[y][x - 1] == 0)
        {
            return Direction.Left;
        }

        // try right
        if (obstacleArray[y][x + 1] == 0)
        {
            return Direction.Right;
        }

        //is blocked
        return Direction.Blocked;
    }

    public void FillEdges(List<Obstacle> obstacleCorners)
    {
        for (var i = 1; i < obstacleCorners.Count; i++)
        {
            var corner1 = obstacleCorners[i - 1];
            var corner2 = obstacleCorners[i];


            if (corner1.x != corner2.x)
            {
                var xDir = corner1.x < corner2.x ? 1 : -1;
                for (var x = corner1.x + xDir; x != corner2.x; x += xDir)
                {
                    obstacleArray[corner1.y][x] = 1;
                }
            }
            else if (corner1.y != corner2.y)
            {
                var yDir = corner1.y < corner2.y ? 1 : -1;
                for (var y = corner1.y + yDir; y != corner2.y; y += yDir)
                {
                    obstacleArray[y][corner1.x] = 1;

                }
            }
        }
    }

    public void Draw()
    {
        for (var y = 0; y <= obstacles.Max(obs => obs.y); y++)
        {
            for (var x = obstacles.Min(obs => obs.x); x <= obstacles.Max(obs => obs.x); x++)
            {
                var obstacle = obstacleArray[y][x];
                if (obstacle == 0)
                {
                    Console.Write('.');
                }
                else
                {
                    if (obstacle == 2)
                    {
                        Console.Write('o');
                    }
                    else if (obstacle == 1)
                    {
                        Console.Write('#');
                    }
                }
            }
            Console.WriteLine();
        }
    }
}

public class Obstacle
{
    public int x;
    public int y;

    public Obstacle(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public enum Direction
{
    Left = -1,
    Blocked = 0,
    Right = 1,
}