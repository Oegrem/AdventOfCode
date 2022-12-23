using AdventOfCode2022.InputHelper;
using System.Data;
using System.Numerics;

namespace Puzzles.Completed.DayNine;

public class Main
{
    const string FOLDER = "DayNine";

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));

        var head = new MovingObject(new Vector2(0, 0));
        var currentTail = head.AddTail();
        for (var i = 0; i < 8; i++)
        {
            currentTail = currentTail.AddTail();
        }
        foreach (var inputString in inputStrings)
        {
            var nextMoveCommand = MoveCommand.ParseMoveCommand(inputString);
            for (var i = 0; i < nextMoveCommand.Movements; i++)
            {
                head.Move(nextMoveCommand.Direction);
            }
        }

        Console.WriteLine(head.GetLastTail().GetDistinctVisitedPositions());
    }
}

public class MovingObject
{
    public Vector2 Position;

    public List<Vector2> VisitedPositions = new();

    public MovingObject? Head = null;

    public MovingObject? Tail = null;

    public MovingObject(Vector2 position)
    {
        Position = position;
        VisitedPositions.Add(Position);
    }

    public MovingObject AddTail()
    {
        var tail = new MovingObject(Position);
        tail.Head = this;
        Tail = tail;
        return Tail;
    }

    public MovingObject GetLastTail()
    {
        if (Tail == null)
            return this;
        return Tail.GetLastTail();
    }

    public void CatchUp()
    {
        if (Head == null || GetHeadDistance() < 2)
            return;
        var direction = Vector2.Subtract(Head.Position, Position);
        Position.Y += Math.Sign(direction.Y);
        Position.X += Math.Sign(direction.X);
        VisitedPositions.Add(Position);

        if (Tail == null)
            return;
        Tail.CatchUp();
    }


    public int GetDistinctVisitedPositions()
    {
        return VisitedPositions.Distinct().Count();
    }

    public void Move(Direction direction)
    {
        var moveVector = new Vector2(0, 0);
        switch (direction)
        {
            case Direction.Up:
                moveVector.Y = -1;
                break;
            case Direction.Down:
                moveVector.Y = 1;
                break;
            case Direction.Left:
                moveVector.X = -1;
                break;
            case Direction.Right:
                moveVector.X = 1;
                break;
        }
        Position = Vector2.Add(Position, moveVector);
        VisitedPositions.Add(Position);

        if (Tail != null)
            Tail.CatchUp();
    }
    public float GetHeadDistance()
    {
        if (Head == null)
            return 0f;
        return Vector2.Distance(Position, Head.Position);
    }

    public float GetDistance(MovingObject otherObject)
    {
        return Vector2.Distance(Position, otherObject.Position);
    }
}

public class MoveCommand
{
    public Direction Direction;
    public int Movements;

    public static MoveCommand ParseMoveCommand(string command)
    {
        var splitCommand = command.Split(' ');

        var newMoveCommand = new MoveCommand();
        newMoveCommand.Movements = int.Parse(splitCommand[1]);

        switch (splitCommand[0])
        {
            case "U":
                newMoveCommand.Direction = Direction.Up;
                break;
            case "D":
                newMoveCommand.Direction = Direction.Down;
                break;
            case "L":
                newMoveCommand.Direction = Direction.Left;
                break;
            case "R":
                newMoveCommand.Direction = Direction.Right;
                break;
            default:
                break;
        }
        return newMoveCommand;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}