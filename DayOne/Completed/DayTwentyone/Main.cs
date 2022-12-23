using AdventOfCode2022.InputHelper;
using Puzzles.Completed.DayEleven;
using System.Diagnostics;

namespace Puzzles.Completed.DayTwentyone;

public class Main
{
    const string FOLDER = "DayTwentyone";

    const bool USE_SAMPLE = false;

    //public static Dictionary<string, Monkey> Monkeys = new();
    public static Dictionary<string, (bool isInt, long result, string op1, string op2, char opr)> Monkeys = new();

    public static Dictionary<string, (bool isInt, bool isVariable, long result, string op1, string op2, char opr)> PreSolvedMonkeys = new();

    public static bool found = false;

    public static long X = 0;

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));
        if (USE_SAMPLE)
            inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSample(FOLDER));

        ParseMonkeyString(inputStrings);
        TestInput(inputStrings);
        PreSolve("root");
        PreSolvedMonkeys.Add("root", (false, true, 0, Monkeys["root"].op1, Monkeys["root"].op2, Monkeys["root"].opr));
        long dif = 1;
        long dStep = 1;
        int lastD = 1;
        while (!found)
        {
            //Parallel.Invoke(
            //	() => CalcDeepPreSolved("root", dif),
            //	() => CalcDeepPreSolved("root", dif + 1),
            //	() => CalcDeepPreSolved("root", dif + 2),
            //	() => CalcDeepPreSolved("root", -dif),
            //	() => CalcDeepPreSolved("root", -dif - 1),
            //	() => CalcDeepPreSolved("root", -dif - 2)
            //);
            var r = CalcDeepPreSolved("root", dif);

            if (r == 1)
            {
                if (lastD != 1)
                {
                    dStep = 1;
                    lastD = 1;
                }
                else
                {
                    dStep *= 2;
                }
            }
            else if (r == 2)
            {
                if (lastD != 2)
                {
                    dStep = -1;
                    lastD = 2;
                }
                else
                {
                    dStep *= 2;
                }

                //dif = (long)Math.Floor(((double) (dif / 2)));
            }
            dif += dStep;

            //dif++;

        }
        Console.WriteLine(X);
    }

    public static long? PreSolve(string name)
    {
        var monkey = Monkeys[name];

        if (name == "humn") return null;

        if (monkey.isInt) return monkey.result;

        var op1 = PreSolve(monkey.op1);
        var op2 = PreSolve(monkey.op2);

        var isnull = false;
        if (op1 == null)
        {
            PreSolvedMonkeys.Add(monkey.op1, (false, true, 0,
                Monkeys[monkey.op1].op1, Monkeys[monkey.op1].op2, Monkeys[monkey.op1].opr));

            isnull = true;
        }
        else
        {
            PreSolvedMonkeys.Add(monkey.op1, (true, false, op1.Value,
                Monkeys[monkey.op1].op1, Monkeys[monkey.op1].op2, Monkeys[monkey.op1].opr));
        }
        if (op2 == null)
        {
            PreSolvedMonkeys.Add(monkey.op2, (false, true, 0,
                Monkeys[monkey.op2].op1, Monkeys[monkey.op2].op2, Monkeys[monkey.op2].opr));

            isnull = true;
        }
        else
        {
            PreSolvedMonkeys.Add(monkey.op2, (true, false, op2.Value,
                Monkeys[monkey.op1].op1, Monkeys[monkey.op1].op2, Monkeys[monkey.op1].opr));
        }
        if (isnull) return null;

        var res = monkey.opr switch
        {
            '+' => op1 + op2,
            '-' => op1 - op2,
            '*' => op1 * op2,
            '/' => op1 / op2,
            _ => throw new Exception($"Not valid operator {monkey.opr}")
        };

        return res;
    }

    public static void ParseMonkeyString(string[] inputStrings)
    {
        foreach (var inputString in inputStrings)
        {
            var splitString = inputString.Split(": ");
            var mName = splitString[0];
            if (int.TryParse(splitString[1], out var resInt))
            {
                Monkeys.Add(mName, (true, resInt, "", "", '.'));
                continue;
            }
            var formula = splitString[1].Split(" ");

            if (mName == "root")
            {
                Monkeys.Add(mName, (false, 0, formula[0], formula[2], '='));
            }
            else
            {
                Monkeys.Add(mName, (false, 0, formula[0], formula[2], char.Parse(formula[1])));
            }

        }
    }

    public static void TestInput(string[] inputStrings)
    {
        int id = 0;
        foreach (var m in Monkeys)
        {
            var mS = $"{m.Key}: {m.Value.op1} {m.Value.opr} {m.Value.op2}";
            if (m.Value.isInt)
                mS = $"{m.Key}: {m.Value.result}";

            if (mS != inputStrings[id]) Console.WriteLine(mS);

            id++;
        }
    }

    public static long CalcDeepPreSolved(string name, long x)
    {
        var monkey = PreSolvedMonkeys[name];

        if (name == "humn") return x;

        if (monkey.isInt) return monkey.result;

        var op1 = CalcDeepPreSolved(monkey.op1, x);
        var op2 = CalcDeepPreSolved(monkey.op2, x);

        if (monkey.opr == '=')
        {
            if (op1 == op2)
            {
                X = x - 1;
                found = true;
                return 0;
            }
            if (op1 > op2)
            {
                return 1;
            }
            else
            {
                return 2;
            }
            return 0;
        }

        var res = monkey.opr switch
        {
            '+' => op1 + op2,
            '-' => op1 - op2,
            '*' => op1 * op2,
            '/' => op1 / op2,
            _ => throw new Exception($"Not valid operator {monkey.opr}")
        };

        return res;
    }

    public static long CalcDeep(string name, long x)
    {
        var monkey = Monkeys[name];

        if (name == "humn") return x;

        if (monkey.isInt) return monkey.result;

        var op1 = CalcDeep(monkey.op1, x);
        var op2 = CalcDeep(monkey.op2, x);

        if (monkey.opr == '=')
        {
            if (op1 == op2)
            {
                found = true;
                return 0;
            }
            return 0;
        }

        var res = monkey.opr switch
        {
            '+' => op1 + op2,
            '-' => op1 - op2,
            '*' => op1 * op2,
            '/' => op1 / op2,
            _ => throw new Exception($"Not valid operator {monkey.opr}")
        };

        monkey.result = res;

        return res;
    }
}


public class Monkey
{
    public bool isDone = false;
    public long result = 0;
    public string op1 = string.Empty;
    public string op2 = string.Empty;
    public char opr = '.';

    public bool isInt = false;

    public Monkey(long res)
    {
        isDone = true;
        isInt = true;
        result = res;
    }

    public Monkey(string op1, string op2, char opr)
    {
        this.op1 = op1;
        this.op2 = op2;
        this.opr = opr;
    }
}