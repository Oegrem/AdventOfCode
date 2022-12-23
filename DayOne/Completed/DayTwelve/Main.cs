using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayTwelve;

public class Main
{
    const string FOLDER = "DayTwelve";

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));

        //Pathfinder.ParseMapString(inputStrings);
        PathfinderReverse.ParseMapString(inputStrings);



        //DijkstraAlgo(Pathfinder.StartX, Pathfinder.StartY);
        DijkstraAlgo2(PathfinderReverse.StartX, PathfinderReverse.StartY);

    }

    public static void DijkstraAlgo(int x, int y)
    {
        if (x == Pathfinder.EndX && y == Pathfinder.EndY)
        {
            Console.WriteLine(Pathfinder.Map[y][x].Distance);
            return;
        }
        ref var currentNode = ref Pathfinder.Map[y][x];
        var unvisitedNeighbours = Pathfinder.GetUnvisitedNeighbours(x, y);

        foreach (var neighbour in unvisitedNeighbours)
        {
            ref var neighbourNode = ref Pathfinder.Map[neighbour.Item2][neighbour.Item1];
            if (neighbourNode.Distance == -1 || neighbourNode.Distance > currentNode.Distance + 1)
                neighbourNode.Distance = currentNode.Distance + 1;
        }
        currentNode.Visited = true;
        var smallestNode = Pathfinder.GetSmallestNode();

        if (smallestNode == null)
            return;

        DijkstraAlgo(smallestNode.Item1, smallestNode.Item2);
    }

    public static void DijkstraAlgo2(int x, int y)
    {
        if (PathfinderReverse.Map[y][x].Height == 1)
        {
            Console.WriteLine(PathfinderReverse.Map[y][x].Distance);
            return;
        }
        ref var currentNode = ref PathfinderReverse.Map[y][x];
        var unvisitedNeighbours = PathfinderReverse.GetUnvisitedNeighbours(x, y);

        foreach (var neighbour in unvisitedNeighbours)
        {
            ref var neighbourNode = ref PathfinderReverse.Map[neighbour.Item2][neighbour.Item1];
            if (neighbourNode.Distance == -1 || neighbourNode.Distance > currentNode.Distance + 1)
                neighbourNode.Distance = currentNode.Distance + 1;
        }
        currentNode.Visited = true;
        var smallestNode = PathfinderReverse.GetSmallestNode();

        if (smallestNode == null)
            return;

        DijkstraAlgo2(smallestNode.Item1, smallestNode.Item2);
    }
}

public static class Pathfinder
{
    public static Node[][] Map;

    public static int MapXSize;
    public static int MapYSize;

    public static int StartX;
    public static int StartY;

    public static int EndX;
    public static int EndY;

    public static List<int> MoveCounts = new();

    public static List<Tuple<int, int>> Path = new();

    public static void ParseMapString(string[] inputStrings)
    {
        MapYSize = inputStrings.Length;
        MapXSize = inputStrings[0].Length;

        Map = new Node[MapYSize][];

        int y = 0;
        foreach (var yString in inputStrings)
        {
            Map[y] = new Node[MapXSize];
            int x = 0;
            foreach (var heightChar in yString)
            {
                var height = heightChar;
                var distance = -1;

                if (heightChar == 'S')
                {
                    StartX = x;
                    StartY = y;
                    height = 'a';
                    distance = 0;
                }
                if (heightChar == 'E')
                {
                    EndX = x;
                    EndY = y;
                    height = 'z';
                }

                Map[y][x] = new Node(false, height - 96, distance);
                x++;
            }
            y++;
        }
    }

    public static List<Tuple<int, int>> GetUnvisitedNeighbours(int x, int y, bool visited = false)
    {
        var possibleMoves = new List<Tuple<int, int>>();
        for (int dir = -1; dir <= 1; dir += 2)
        {
            if (x + dir >= 0 && x + dir < MapXSize)
            {
                if (Map[y][x].Height + 1 >= Map[y][x + dir].Height && Map[y][x + dir].Visited == visited)
                {
                    possibleMoves.Add(new Tuple<int, int>(x + dir, y));
                }
            }
            if (y + dir >= 0 && y + dir < MapYSize)
            {
                if (Map[y][x].Height + 1 >= Map[y + dir][x].Height && Map[y + dir][x].Visited == visited)
                {
                    possibleMoves.Add(new Tuple<int, int>(x, y + dir));
                }
            }
        }

        return possibleMoves;
    }

    public static Tuple<int, int>? GetSmallestNode()
    {
        var smallestDistance = -1;

        var nodeX = 0;
        var nodeY = 0;

        var y = 0;
        foreach (var line in Map)
        {
            var x = 0;
            foreach (var node in line)
            {
                if (!node.Visited && node.Distance != -1 &&
                    (smallestDistance == -1 || node.Distance < smallestDistance))
                {
                    smallestDistance = node.Distance;
                    nodeX = x;
                    nodeY = y;
                }
                x++;
            }
            y++;
        }

        if (smallestDistance == -1)
        {
            return null;
        }

        return new Tuple<int, int>(nodeX, nodeY);
    }
}

public static class PathfinderReverse
{
    public static Node[][] Map;

    public static int MapXSize;
    public static int MapYSize;

    public static int StartX;
    public static int StartY;

    public static int EndX;
    public static int EndY;

    public static List<Tuple<int, int>> Path = new();

    public static void ParseMapString(string[] inputStrings)
    {
        MapYSize = inputStrings.Length;
        MapXSize = inputStrings[0].Length;

        Map = new Node[MapYSize][];

        int y = 0;
        foreach (var yString in inputStrings)
        {
            Map[y] = new Node[MapXSize];
            int x = 0;
            foreach (var heightChar in yString)
            {
                var height = heightChar;
                var distance = -1;

                if (heightChar == 'E')
                {
                    StartX = x;
                    StartY = y;
                    height = 'z';
                    distance = 0;
                }
                if (heightChar == 'S')
                {
                    height = 'a';
                }

                Map[y][x] = new Node(false, height - 96, distance);
                x++;
            }
            y++;
        }
    }

    public static List<Tuple<int, int>> GetUnvisitedNeighbours(int x, int y)
    {
        var possibleMoves = new List<Tuple<int, int>>();
        for (int dir = -1; dir <= 1; dir += 2)
        {
            if (x + dir >= 0 && x + dir < MapXSize)
            {
                if (Map[y][x + dir].Height + 1 >= Map[y][x].Height && Map[y][x + dir].Visited == false)
                {
                    possibleMoves.Add(new Tuple<int, int>(x + dir, y));
                }
            }
            if (y + dir >= 0 && y + dir < MapYSize)
            {
                if (Map[y + dir][x].Height + 1 >= Map[y][x].Height && Map[y + dir][x].Visited == false)
                {
                    possibleMoves.Add(new Tuple<int, int>(x, y + dir));
                }
            }
        }

        return possibleMoves;
    }

    public static Tuple<int, int>? GetSmallestNode()
    {
        var smallestDistance = -1;

        var nodeX = 0;
        var nodeY = 0;

        var y = 0;
        foreach (var line in Map)
        {
            var x = 0;
            foreach (var node in line)
            {
                if (!node.Visited && node.Distance != -1 &&
                    (smallestDistance == -1 || node.Distance < smallestDistance))
                {
                    smallestDistance = node.Distance;
                    nodeX = x;
                    nodeY = y;
                }
                x++;
            }
            y++;
        }

        if (smallestDistance == -1)
        {
            return null;
        }

        return new Tuple<int, int>(nodeX, nodeY);
    }
}

public class Node
{
    public bool Visited = false;
    public int Height = 1;
    public int Distance = -1;

    public Node() { }

    public Node(bool visited, int height, int distance)
    {
        Visited = visited;
        Height = height;
        Distance = distance;
    }
}
