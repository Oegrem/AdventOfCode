using AdventOfCode2022.InputHelper;
using System.Data;
using System.Reflection.PortableExecutable;

namespace Puzzles.Completed.DayTwentytwo;

public class Main
{
    const string FOLDER = "DayTwentytwo";

    const bool USE_SAMPLE = false;

    const int SideSize = USE_SAMPLE ? 4 : 50;

    const int MapSize = SideSize * 4 + 1;

    public static int[][] Map = new int[MapSize][];

    public static int[][][][] cubeSideMap = new int[4][][][];

    const bool DEBUG = false;

    public const int CW = -1;
    public const int CCW = -2;

    public const int RIGHT = 0;
    public const int DOWN = 1;
    public const int LEFT = 2;
    public const int UP = 3;

    public const int NO_TILE = 0;
    public const int EMPTY = 1;
    public const int BLOCK = 2;

    public const int C_TOP = 0;
    public const int C_FRONT = 1;
    public const int C_RIGHT = 2;
    public const int C_BACK = 3;
    public const int C_LEFT = 4;
    public const int C_BOTTOM = 5;

    public static Queue<int> Route = new();

    public static (int x, int y, int rot) Position = (0, 0, RIGHT);

    public static async Task SolvePuzzle()
    {
        var inputString = await InputHandler.GetStringFromInputFile(FOLDER);
        if (USE_SAMPLE)
            inputString = await InputHandler.GetStringFromSample(FOLDER);

        ParseInput(inputString);

        var nextTile = NO_TILE;
        var x = 0;
        while (nextTile == NO_TILE)
        {
            x++;
            nextTile = Map[0][x];
        }
        Position.x = x;

        MoveRouteOnMap();

        //ParseInputCube(inputString);

        var res = 1000 * (Position.y + 1) + 4 * (Position.x + 1) + Position.rot;
        Console.WriteLine(res);
    }

    public static void MoveRouteOnMap()
    {
        while (Route.TryDequeue(out var nextMove))
        {
            // turn and continue to next step
            if (nextMove < 0)
            {
                var rot = Position.rot;

                if (rot == RIGHT && nextMove == CCW)
                {
                    rot = UP;
                }
                else
                {
                    rot = (rot + (nextMove == CCW ? -1 : 1)) % 4;
                }

                Position.rot = rot;
                //PrintMap();
                continue;
            }


            // LOOP through steps
            for (var s = 0; s < nextMove; s++)
            {

                // Check next block depending on rot and position
                var nextTile = GetNextTile();

                // Stop steps
                if (nextTile == BLOCK) break;

                // continue if EMPTY
                if (nextTile == EMPTY)
                {
                    MoveStep();
                    continue;
                }

                // wrap
                if (nextTile == NO_TILE)
                {
                    if (DEBUG)
                    {
                        PrintMap();
                    }

                    // Wrap
                    if (!CubeWrap())
                    {
                        Console.Clear();
                        break;
                    }

                    if (DEBUG)
                    {
                        PrintMap();
                        Console.WriteLine();
                        Console.Clear();
                    }
                    /*
					// check next wrap position
					var backTile = EMPTY;
					var currPos = (Position.x, Position.y);
					var dir = GetDirection();
					while (backTile != NO_TILE)
					{
						currPos.x -= dir.x;
						currPos.y -= dir.y;
						if (currPos.x < 0 || currPos.y < 0)
						{
							break;
						}
						backTile = Map[currPos.y][currPos.x];
					}
					currPos.x += dir.x;
					currPos.y += dir.y;
					if (Map[currPos.y][currPos.x] == BLOCK) break;
					if (Map[currPos.y][currPos.x] == EMPTY)
					{
						Position.x = currPos.x;
						Position.y = currPos.y;
					}
					*/
                }
            }
        }
    }

    public static bool CubeWrap()
    {
        if (Position.y < 50)
        {
            // left top side of map
            if (Position.x < 100)
            {
                if (Position.rot == LEFT)
                {
                    // 49 > 100 => 149 - 49
                    // 0 > 149 => 149 - 0
                    if (Map[149 - Position.y][0] == BLOCK) return false;

                    Position.x = 0;
                    Position.y = 149 - Position.y;
                    Position.rot = RIGHT;
                    return true;
                }
                if (Position.rot == UP)
                {
                    // y 0 x 50 => y 150 x 0
                    // y 0 x 70 => y 170 x 0
                    if (Map[100 + Position.x][0] == BLOCK) return false;

                    Position.y = Position.x + 100;
                    Position.x = 0;
                    Position.rot = RIGHT;
                    return true;
                }
            }
            // right top side of map
            if (Position.x >= 100)
            {
                if (Position.rot == UP)
                {
                    // y 0 x 100  => y 199 x 0
                    // y 0 x 149  => y 199 x 49
                    if (Map[199][Position.x - 100] == BLOCK) return false;

                    Position.y = 199;
                    Position.x = Position.x - 100;
                    return true;
                }

                if (Position.rot == RIGHT)
                {
                    // 49 > 100 => 149 - 49
                    // 0 > 149 => 149 - 0
                    if (Map[149 - Position.y][99] == BLOCK) return false;

                    Position.x = 99;
                    Position.y = 149 - Position.y;
                    Position.rot = LEFT;
                    return true;
                }

                if (Position.rot == DOWN)
                {
                    // 50 > 100 => 149 - 49
                    // 0 > 149 => 149 - 0
                    // x 100 > y 50 x 149 > y 99
                    if (Map[Position.x - 50][99] == BLOCK) return false;

                    Position.y = Position.x - 50;
                    Position.x = 99;
                    Position.rot = LEFT;
                    return true;
                }
            }
        }
        if (Position.y < 100)
        {
            if (Position.rot == LEFT)
            {
                // y 50 > x 0
                // y 99 > x 49
                // y - 50
                if (Map[100][Position.y - 50] == BLOCK) return false;

                Position.x = Position.y - 50;
                Position.y = 100;
                Position.rot = DOWN;
                return true;
            }

            if (Position.rot == RIGHT)
            {
                // 50 > 100 => 149 - 49
                // 0 > 149 => 149 - 0
                // x 100 > y 50 x 149 > y 99
                if (Map[49][Position.y + 50] == BLOCK) return false;

                Position.x = Position.y + 50;
                Position.y = 49;
                Position.rot = UP;
                return true;
            }
        }
        if (Position.y < 150)
        {
            if (Position.x < 50)
            {
                if (Position.rot == LEFT)
                {
                    // y 100 > y 49
                    // y 149 > y 0
                    // 149 - y
                    if (Map[149 - Position.y][50] == BLOCK) return false;

                    Position.y = 149 - Position.y;
                    Position.x = 50;
                    Position.rot = RIGHT;
                    return true;
                }
                if (Position.rot == UP)
                {
                    // x 0 => y 50
                    // x 49 => y 99
                    // x + 50
                    if (Map[Position.x + 50][50] == BLOCK) return false;

                    Position.y = Position.x + 50;
                    Position.x = 50;
                    Position.rot = RIGHT;
                    return true;
                }
            }

            if (Position.rot == RIGHT)
            {
                // y 100 => y 49
                // y 149 => y 0
                if (Map[149 - Position.y][149] == BLOCK) return false;

                Position.x = 149;
                Position.y = 149 - Position.y;
                Position.rot = LEFT;
                return true;
            }
            if (Position.rot == DOWN)
            {
                // x 50 => y 150
                // x 99 => y 199
                // x + 100 => y
                if (Map[Position.x + 100][49] == BLOCK) return false;

                Position.y = Position.x + 100;
                Position.x = 49;
                Position.rot = LEFT;
                return true;
            }
        }
        if (Position.y >= 150)
        {
            if (Position.rot == LEFT)
            {
                // y 150 => x 50
                // y 199 => x 99
                if (Map[0][Position.y - 100] == BLOCK) return false;

                Position.x = Position.y - 100;
                Position.y = 0;
                Position.rot = DOWN;
                return true;
            }

            if (Position.rot == DOWN)
            {
                // x 0  => x 100
                // x 49  => x 149
                if (Map[0][Position.x + 100] == BLOCK) return false;

                Position.y = 0;
                Position.x = Position.x + 100;
                return true;
            }

            if (Position.rot == RIGHT)
            {
                // y 150 => x 50
                // y 199 => x 99
                // y - 100 => x
                if (Map[149][Position.y - 100] == BLOCK) return false;

                Position.x = Position.y - 100;
                Position.y = 149;
                Position.rot = UP;
                return true;
            }
        }
        return false;
    }

    public static int GetNextTile()
    {
        if (Position.rot == UP)
        {
            if (Position.y != 0)
            {
                return Map[Position.y - 1][Position.x];
            }
            return NO_TILE;
        }

        if (Position.rot == DOWN)
        {
            return Map[Position.y + 1][Position.x];
        }

        if (Position.rot == RIGHT)
        {
            return Map[Position.y][Position.x + 1];

        }

        if (Position.rot == LEFT)
        {
            if (Position.x != 0)
            {
                return Map[Position.y][Position.x - 1];
            }
            return NO_TILE;
        }

        return NO_TILE;
    }

    public static void MoveStep()
    {
        var newPos = GetDirection();
        Position.x += newPos.x;
        Position.y += newPos.y;
    }

    public static (int x, int y) GetDirection()
    {
        return Position.rot switch
        {
            RIGHT => (1, 0),
            DOWN => (0, 1),
            LEFT => (-1, 0),
            UP => (0, -1),
            _ => throw new Exception("invalid move direction")
        };
    }

    public static void ParseInput(string inputString)
    {
        var splitString = inputString.Split("\n\n");

        var mapStrings = InputHandler.GetStringLines(splitString[0], false);
        var routeString = splitString[1];

        var i = 0;
        foreach (var mapLine in mapStrings)
        {
            Map[i] = new int[MapSize];
            var j = 0;
            foreach (var c in mapLine)
            {
                Map[i][j] = c switch
                {
                    '.' => EMPTY,
                    '#' => BLOCK,
                    _ => NO_TILE
                };
                j++;
            }
            // Add empty character
            Map[i][j] = NO_TILE;
            i++;
        }
        // Add empty line
        Map[i] = new int[MapSize];
        for (var j = 0; j < MapSize; j++)
        {
            Map[i][j] = 0;
        }

        var splitRouteString = routeString.Replace("L", " L ").Replace("R", " R ").Split(" ");

        foreach (var move in splitRouteString)
        {
            if (int.TryParse(move, out int steps))
            {
                Route.Enqueue(steps);
                continue;
            }

            var turn = move switch
            {
                "R" => CW,
                "L" => CCW,
                _ => throw new Exception("Not valid non int in route")
            };
            Route.Enqueue(turn);
        }
    }

    public static void ParseInputCube(string inputString)
    {
        var splitString = inputString.Split("\n\n");

        var mapStrings = InputHandler.GetStringLines(splitString[0], false);
        var routeString = splitString[1];

        for (var cubeMapY = 0; cubeMapY < 4; cubeMapY++)
        {
            cubeSideMap[cubeMapY] = new int[4][][];
            for (var cubeMapX = 0; cubeMapX < 4; cubeMapX++)
            {
                int[][] mapSide = new int[SideSize][];
                for (var y = 0; y < SideSize; y++)
                {
                    mapSide[y] = new int[SideSize];
                    for (var x = 0; x < SideSize; x++)
                    {
                        var inputY = cubeMapY * SideSize + y;
                        var inputX = cubeMapX * SideSize + x;
                        if (inputY < mapStrings.Length && inputX < mapStrings[inputY].Length)
                        {
                            mapSide[y][x] = mapStrings[inputY][inputX] switch
                            {
                                '.' => EMPTY,
                                '#' => BLOCK,
                                _ => NO_TILE
                            };
                        }
                        else
                        {
                            mapSide[y][x] = NO_TILE;
                        }
                    }
                }
                cubeSideMap[cubeMapY][cubeMapX] = (int[][])mapSide.Clone();
            }
        }

        var splitRouteString = routeString.Replace("L", " L ").Replace("R", " R ").Split(" ");

        foreach (var move in splitRouteString)
        {
            if (int.TryParse(move, out int steps))
            {
                Route.Enqueue(steps);
                continue;
            }

            var turn = move switch
            {
                "R" => CW,
                "L" => CCW,
                _ => throw new Exception("Not valid non int in route")
            };
            Route.Enqueue(turn);
        }
    }

    public static void WrapToOtherFace()
    {

    }

    public static void PrintMap()
    {
        for (var i = 0; i < MapSize && Map[i] != null; i++)
        {
            for (var j = 0; j < MapSize; j++)
            {
                var dChar = Map[i][j] switch
                {
                    0 => ' ',
                    1 => '.',
                    2 => '#',
                    _ => ' '
                };

                if (Position.x == j && Position.y == i)
                {
                    dChar = Position.rot switch
                    {
                        RIGHT => '>',
                        UP => '^',
                        DOWN => 'V',
                        LEFT => '<',
                        _ => 'O'
                    };
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(dChar);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Write(dChar);
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}