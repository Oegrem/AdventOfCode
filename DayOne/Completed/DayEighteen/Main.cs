using AdventOfCode2022.InputHelper;
using Common.Algorithm;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Puzzles.Completed.DayEighteen;

public class Main
{
    const string FOLDER = "DayEighteen";

    const int MATRIX_SIZE = 100;

    const int GRID_SIZE = 30;

    const bool USE_SAMPLE = false;

    public static Dictionary<(int x, int y, int z), Cube> cubeDictionary = new();

    public static int VisitedSites = 0;

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));
        if (USE_SAMPLE)
            inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSample(FOLDER));

        for (var i = 0; i < GRID_SIZE; i++)
        {
            for (var j = 0; j < GRID_SIZE; j++)
            {
                for (var k = 0; k < GRID_SIZE; k++)
                {
                    cubeDictionary.Add((i, j, k), new Cube(i, j, k));
                }
            }
        }

        foreach (var line in inputStrings)
        {
            // Needed to shift whole block one into the grid to be able to
            // check the cubes that touch the outside of the grid
            var splitLine = line.Split(',');
            (int x, int y, int z) cubePos = (int.Parse(splitLine[0]) + 1,
                int.Parse(splitLine[1]) + 1,
                int.Parse(splitLine[2]) + 1);

            cubeDictionary[cubePos].Occupied = true;
        }

        var bfs = new BFS(cubeDictionary[(0, 0, 0)]);

        var sw = new Stopwatch();
        sw.Start();
        var result = bfs.Search();
        sw.Stop();
        Console.WriteLine($"{sw.Elapsed}");
		Console.WriteLine(VisitedSites);
    }

    public static void PrintAllWallsAmount(string[] inputStrings)
    {
        var walls = new bool[MATRIX_SIZE][][][];

        for (var i = 0; i < MATRIX_SIZE; i++)
        {
            walls[i] = new bool[MATRIX_SIZE][][];
            for (var j = 0; j < MATRIX_SIZE; j++)
            {
                walls[i][j] = new bool[MATRIX_SIZE][];
                for (var k = 0; k < MATRIX_SIZE; k++)
                {
                    walls[i][j][k] = new bool[3];
                    for (var l = 0; l < 3; l++)
                    {
                        walls[i][j][k][l] = false;
                    }
                }
            }
        }

        foreach (var line in inputStrings)
        {
            var splitLine = line.Split(',');
            (int x, int y, int z) cube = (int.Parse(splitLine[0]),
                int.Parse(splitLine[1]),
                int.Parse(splitLine[2]));

            walls[cube.x][cube.y][cube.z][0] ^= true;
            walls[cube.x][cube.y][cube.z][1] ^= true;
            walls[cube.x][cube.y][cube.z][2] ^= true;

            walls[cube.x + 1][cube.y][cube.z][1] ^= true;
            walls[cube.x][cube.y + 1][cube.z][2] ^= true;
            walls[cube.x][cube.y][cube.z + 1][0] ^= true;
        }

        var c = 0;
        for (var i = 0; i < MATRIX_SIZE; i++)
        {
            for (var j = 0; j < MATRIX_SIZE; j++)
            {
                for (var k = 0; k < MATRIX_SIZE; k++)
                {
                    for (var l = 0; l < 3; l++)
                    {
                        c += walls[i][j][k][l] ? 1 : 0;
                    }
                }
            }
        }

        Console.WriteLine(c);
    }
}

public class Cube : IBFSObject
{
    public bool IsExplored { get; set; }
    public bool IsGoal { get; set; }
    public IBFSObject? Parent { get; set; }
    public bool Occupied = false;

    public (int x, int y, int z) Coords;

    public Cube(int x, int y, int z)
    {
        Coords = (x, y, z);
    }

    public Cube((int x, int y, int z) coords, bool occupied = false)
    {
        Coords = coords;
        Occupied = occupied;
    }

    public void ExecuteLogic()
    {
        //throw new NotImplementedException();
        for (var d = -1; d <= 1; d += 2)
        {
            Main.VisitedSites += SharesWall((d, 0, 0)) ? 1 : 0;
            Main.VisitedSites += SharesWall((0, d, 0)) ? 1 : 0;
            Main.VisitedSites += SharesWall((0, 0, d)) ? 1 : 0;
        }
    }

    public List<IBFSObject> GetNext()
    {
        var nextCubes = new List<Cube>();

        for (var d = -1; d <= 1; d += 2)
        {
            if (Main.cubeDictionary.TryGetValue((Coords.x + d, Coords.y, Coords.z),
                out var nextCube))
            {
                if (!nextCube.Occupied)
                    nextCubes.Add(nextCube);
            }
            if (Main.cubeDictionary.TryGetValue((Coords.x, Coords.y + d, Coords.z),
                out nextCube))
            {
                if (!nextCube.Occupied)
                    nextCubes.Add(nextCube);
            }
            if (Main.cubeDictionary.TryGetValue((Coords.x, Coords.y, Coords.z + d),
                out nextCube))
            {
                if (!nextCube.Occupied)
                    nextCubes.Add(nextCube);
            }
        }

        return nextCubes.ToList<IBFSObject>();
    }

    public bool IsAdjacent((int x, int y, int z) coord)
    {
        if (coord.x == Coords.x && coord.y == Coords.y && coord.z == Coords.z)
            return false;

        if (Coords.x - 1 <= coord.x && coord.x <= Coords.x + 1)
        {
            if (Coords.x - 1 <= coord.y && coord.y <= Coords.y + 1)
            {
                if (Coords.z - 1 <= coord.z && coord.z <= Coords.z + 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool SharesWall((int x, int y, int z) coord)
    {
        if (Main.cubeDictionary
            .TryGetValue((Coords.x + coord.x, Coords.y + coord.y, Coords.z + coord.z),
                out var nextCube))
        {
            if (nextCube.Occupied)
                return true;
        }
        return false;
    }
}