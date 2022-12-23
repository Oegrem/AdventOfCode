using AdventOfCode2022.InputHelper;
using System.Data;

namespace Puzzles.Completed.DayTen;

public class Main
{
    const string FOLDER = "DayTen";

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));

        var cpu = new CPU(1, inputStrings, 40, 6);
        cpu.Run();

        cpu.CRT.DrawScreen();
    }
}

public class CPU
{
    public int CurrentCycle;

    public int X = 1;

    public Queue<Command> Program = new();

    public Dictionary<int, int> RegisterHistory = new();

    public CRT CRT;

    public CPU(int x, string[] program, int screenWidth, int screenHeight)
    {
        X = x;
        CurrentCycle = 0;
        ParseProgram(program);
        CRT = new CRT(screenWidth, screenHeight);
    }

    public void Run()
    {
        foreach (var command in Program)
        {
            while (command.Act(ref X, ref CurrentCycle))
            {
                CRT.DrawPixel(X);
                RegisterHistory.Add(CurrentCycle, X);
            }
        }
    }

    public void ParseProgram(string[] program)
    {
        foreach (var command in program)
        {
            Program.Enqueue(Command.ParseCommand(command));
        }
    }

    public int GetSignalStrength(int cycle)
    {
        int cycleX;
        RegisterHistory.TryGetValue(cycle, out cycleX);
        return cycleX * cycle;
    }
}

public class CRT
{
    public int CurrentCRTPointer;

    public Tuple<int, int> CurrentPixel => GetPixelTupleFromScreenPoint();

    public char[][] Screen;

    public int Width;

    public int Height;

    public CRT(int width, int height)
    {
        Width = width;
        Height = height;

        Screen = new char[height][];
        for (var i = 0; i < Screen.Length; i++)
        {
            Screen[i] = new char[width];
        }

        CurrentCRTPointer = 0;
    }

    public void DrawPixel(int x)
    {
        var currentPixel = CurrentPixel;
        var pixelValue = Pixel.DIM;
        if (Enumerable.Range(x - 1, 3).Contains(CurrentPixel.Item2))
        {
            pixelValue = Pixel.LIT;
        }

        Screen[currentPixel.Item1][currentPixel.Item2] = pixelValue;
        CurrentCRTPointer++;
    }

    public Tuple<int, int> GetPixelTupleFromScreenPoint()
    {
        var row = (int)Math.Floor((double)CurrentCRTPointer / Width);
        var column = CurrentCRTPointer % Width;
        return new Tuple<int, int>(row, column);
    }

    public void DrawScreen()
    {
        foreach (var row in Screen)
        {
            foreach (var column in row)
            {
                Console.Write(column);
            }
            Console.Write('\n');
        }
    }
}

public class Command
{
    public string CommandString = string.Empty;

    public Instruction Inst;
    public int? Value;
    public int ExecutionCycles;

    public static Command ParseCommand(string commandString)
    {
        var command = new Command();
        command.CommandString = commandString;

        if (commandString.StartsWith("noop"))
        {
            command.Inst = Instruction.NOOP;
            command.ExecutionCycles = 1;
        }
        else if (commandString.StartsWith("addx"))
        {
            command.Inst = Instruction.ADDX;
            command.Value = int.Parse(commandString.Split(' ')[1]);
            command.ExecutionCycles = 2;
        }
        return command;
    }

    public bool Act(ref int x, ref int cycles)
    {
        if (ExecutionCycles <= 0)
        {
            if (Inst == Instruction.ADDX && Value != null)
                x += Value.Value;
            return false;
        }

        cycles++;
        ExecutionCycles--;

        return true;
    }

}

public enum Instruction
{
    NOOP,
    ADDX
}

public class Pixel
{
    public const char LIT = '#';
    public const char DIM = '.';
}