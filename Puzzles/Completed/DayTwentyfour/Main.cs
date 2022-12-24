using AdventOfCode2022.InputHelper;
using Common.Algorithm;
using Puzzles.Completed.DayTwo;

namespace Puzzles.Completed.DayTwentyfour;

public class Main
{
    const string FOLDER = "DayTwentyfour";

    const bool USE_SAMPLE = false;
    const bool USE_SAMPLE_SMALL = false;

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));
        if (USE_SAMPLE)
            inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSample(FOLDER));
        if (USE_SAMPLE_SMALL)
            inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSampleSmall(FOLDER));

        var bM = new BlizzardMap(inputStrings);

        var pf = new Pathfinder(bM, new Vector2Int(1, 0), new Vector2Int(120, 26));
        //var pf = new Pathfinder(bM, new Vector2Int(1, 0), new Vector2Int(6, 5));
        //var res = pf.FindPath();
        var res = pf.GetFullRetrievePath();

        Console.WriteLine(res);

    }
}

public class Pathfinder
{
    private int _minMoves = int.MaxValue;

    private Vector2Int _start;
    private Vector2Int _end;

    private BlizzardMap _blizzardMap;

    private Dictionary<(Vector2Int position, int cycle), int> _visitedPositions = new();
    private HashSet<(Vector2Int position, int moves)> _visited = new();

    private int _state = 0;

    public Pathfinder(BlizzardMap blizzardMap, Vector2Int start, Vector2Int end)
    {
        _blizzardMap = blizzardMap;
        _start = start;
        _end = end;
    }

    public int FindPath()
    {
        TryMove(_start, 0, 0);

        return _minMoves;
    }

    public void TryMove(Vector2Int position, int moves, int cycle)
    {
        if (_visitedPositions.TryGetValue((position, cycle), out var oldMoves))
        {
            if (oldMoves < moves) return;

            _visitedPositions[(position, cycle)] = moves;
        }
        else
        {
            _visitedPositions.Add((position, cycle), moves);
        }

        //Console.WriteLine($"Move: {moves}, Cycle: {cycle}");
        //_blizzardMap.PrintMap(cycle, position);
        var nextCycle = (cycle + 1) % _blizzardMap.CycleSize;
        if (position.Equals(_end))
        {
            if (_minMoves > moves) _minMoves = moves;
            return;
        }

        if (moves >= _minMoves) return;

        // try down
        if (position.Y < _blizzardMap.MapSize.Y - 1 &&
            !_blizzardMap.Map[nextCycle][position.Y + 1][position.X].Any(x => x))
            TryMove(position + (0, 1), moves + 1, nextCycle);

        // try left
        if (position.X > 1 &&
            !_blizzardMap.Map[nextCycle][position.Y][position.X - 1].Any(x => x))
            TryMove(position - (1, 0), moves + 1, nextCycle);

        // try up
        if (position.Y > 0 &&
            !_blizzardMap.Map[nextCycle][position.Y - 1][position.X].Any(x => x))
            TryMove(position - (0, 1), moves + 1, nextCycle);

        // try right
        if (position.X < _blizzardMap.MapSize.X - 1 &&
            !_blizzardMap.Map[nextCycle][position.Y][position.X + 1].Any(x => x))
            TryMove(position + (1, 0), moves + 1, nextCycle);

        // try Wait
        if (!_blizzardMap.Map[nextCycle][position.Y][position.X].Any(x => x))
            TryMove(position, moves + 1, nextCycle);
    }

    public int GetFullRetrievePath()
    {
        var moves = 0;
        moves = GetPossibleMoves(_start, _end, moves);
        moves = GetPossibleMoves(_end, _start, moves);
        moves = GetPossibleMoves(_start, _end, moves);
        return moves;
    }

    public int GetPossibleMoves(Vector2Int start, Vector2Int stop, int moves)
    {
        var possibleMoves = new List<Vector2Int>();

        var q = new Queue<(Vector2Int pos, int moves)>();


        q.Enqueue((start, moves));
        while (q.Any())
        {
            var v = q.Dequeue();
            //_blizzardMap.PrintMap(v.cycle, v.pos);

            if (v.pos.Equals(stop))
            {
                return v.moves;
            }

            foreach (var w in GetNextMoves(v.pos, v.moves))
            {
                q.Enqueue(w);
            }
        }
        return int.MaxValue;
    }

    public List<(Vector2Int pos, int moves)> GetNextMoves(Vector2Int position, int moves)
    {
        var nextMoves = new List<(Vector2Int pos, int moves)>();
        var nextCycle = (moves + 1) % _blizzardMap.CycleSize;

        if (!_visited.Add((position, moves)))
        {
            return nextMoves;
        }

        // try down
        if (position.Y < _blizzardMap.MapSize.Y - 1 &&
            !_blizzardMap.Map[nextCycle][position.Y + 1][position.X].Any(x => x))
            nextMoves.Add((position + (0, 1), moves + 1));

        // try left
        if (position.X > 1 &&
            !_blizzardMap.Map[nextCycle][position.Y][position.X - 1].Any(x => x))
            nextMoves.Add((position - (1, 0), moves + 1));

        // try up
        if (position.Y > 0 &&
            !_blizzardMap.Map[nextCycle][position.Y - 1][position.X].Any(x => x))
            nextMoves.Add((position - (0, 1), moves + 1));

        // try right
        if (position.X < _blizzardMap.MapSize.X - 1 &&
            !_blizzardMap.Map[nextCycle][position.Y][position.X + 1].Any(x => x))
            nextMoves.Add((position + (1, 0), moves + 1));

        // try Wait
        if (!_blizzardMap.Map[nextCycle][position.Y][position.X].Any(x => x))
            nextMoves.Add((position, moves + 1));

        return nextMoves;
    }


}

public class BlizzardMap
{
    private readonly Vector2Int _mapSize;
    private bool[][][][] _map;
    private int _cycleSize;

    const int Wall = 0;
    const int BlizzUp = 1;
    const int BlizzDown = 2;
    const int BlizzLeft = 3;
    const int BlizzRight = 4;

    public bool[][][][] Map { get => _map; }

    public int CycleSize { get => _cycleSize; }

    public Vector2Int MapSize { get => _mapSize; }

    public BlizzardMap(string[] inputStrings)
    {
        _mapSize = new Vector2Int(inputStrings[0].Length, inputStrings.Length);
        CalculateCycleSize();

        _map = new bool[_cycleSize][][][];
        _map[0] = new bool[_mapSize.Y][][];

        for (var y = 0; y < _mapSize.Y; y++)
        {
            _map[0][y] = new bool[_mapSize.X][];
            for (var x = 0; x < _mapSize.X; x++)
            {
                _map[0][y][x] = inputStrings[y][x] switch
                {
                    '.' => Place.Free(),
                    '#' => Place.Wall(),
                    '^' => Place.BlizzUp(),
                    '<' => Place.BlizzLeft(),
                    '>' => Place.BlizzRight(),
                    'v' => Place.BlizzDown(),
                    _ => throw new Exception(
                        $"Could not parse character {inputStrings[y][x]} at Y: {y}, X: {x}")
                };
            }
        }


        // create cycled map
        for (var i = 1; i < _cycleSize; i++)
        {
            _map[i] = new bool[_mapSize.Y][][];
            // move
            for (var y = 0; y < _mapSize.Y; y++)
            {
                _map[i][y] = new bool[_mapSize.X][];
                for (var x = 0; x < _mapSize.X; x++)
                {
                    _map[i][y][x] = new bool[5];
                    if (y == 0 || y == _mapSize.Y - 1 ||
                        x == 0 || x == _mapSize.X - 1)
                    {
                        _map[i][y][x][Wall] = _map[0][y][x][Wall];
                        continue;
                    }

                    // Check up
                    if (_map[i - 1][y - 1][x][Wall])
                    {
                        _map[i][y][x][BlizzDown] = _map[i - 1][_mapSize.Y - 2][x][BlizzDown];
                    }
                    else
                    {
                        _map[i][y][x][BlizzDown] = _map[i - 1][y - 1][x][BlizzDown];
                    }

                    // Check down
                    if (_map[i - 1][y + 1][x][Wall])
                    {
                        _map[i][y][x][BlizzUp] = _map[i - 1][1][x][BlizzUp];
                    }
                    else
                    {
                        _map[i][y][x][BlizzUp] = _map[i - 1][y + 1][x][BlizzUp];
                    }

                    // Check left
                    if (_map[i - 1][y][x - 1][Wall])
                    {
                        _map[i][y][x][BlizzRight] = _map[i - 1][y][_mapSize.X - 2][BlizzRight];
                    }
                    else
                    {
                        _map[i][y][x][BlizzRight] = _map[i - 1][y][x - 1][BlizzRight];
                    }

                    // Check right
                    if (_map[i - 1][y][x + 1][Wall])
                    {
                        _map[i][y][x][BlizzLeft] = _map[i - 1][y][1][BlizzLeft];
                    }
                    else
                    {
                        _map[i][y][x][BlizzLeft] = _map[i - 1][y][x + 1][BlizzLeft];
                    }
                }
            }
        }
    }

    private void CalculateCycleSize()
    {
        var minSide = Math.Min(_mapSize.X, _mapSize.Y) - 2;
        var maxSide = Math.Max(_mapSize.X, _mapSize.Y) - 2;

        for (var i = 1; i <= maxSide; i++)
        {
            if (minSide * i % maxSide == 0)
            {
                _cycleSize = minSide * i;
                return;
            }
        }
    }

    public void PrintMap(int state = 0, Vector2Int? marker = null)
    {
        var y = 0;
        foreach (var row in _map[state])
        {
            var x = 0;
            foreach (var place in row)
            {
                var c = Place.BoolArrayToInt(place) switch
                {
                    0 => '.',
                    1 => '#',
                    2 => '^',
                    4 => 'v',
                    8 => '<',
                    16 => '>',
                    _ => place.Count(x => x).ToString()[0]
                };
                if (marker != null && marker.X == x && marker.Y == y)
                    c = 'E';
                Console.Write(c);
                x++;
            }
            y++;
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}

public static class Place
{
    public static bool[] Free()
    {
        return new bool[5];
    }

    public static bool[] Wall()
    {
        return new bool[5] { true, false, false, false, false };
    }

    public static bool[] BlizzUp()
    {
        return new bool[5] { false, true, false, false, false };
    }

    public static bool[] BlizzDown()
    {
        return new bool[5] { false, false, true, false, false };
    }

    public static bool[] BlizzLeft()
    {
        return new bool[5] { false, false, false, true, false };
    }

    public static bool[] BlizzRight()
    {
        return new bool[5] { false, false, false, false, true };
    }

    public static int BoolArrayToInt(bool[] bArray, int size = 5)
    {
        var value = 0;
        for (var i = 0; i < size; i++)
        {
            if (bArray[i])
                value += (int)Math.Pow(2, i);
        }
        return value;
    }
}