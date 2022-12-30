using AdventOfCode2022.InputHelper;

namespace Puzzles.Completed.DayEleven;

public class Main
{
    const string FOLDER = "DayEleven";

    public static async Task SolvePuzzle()
    {
        var inputStrings = (await InputHandler.GetStringFromInputFile(FOLDER)).Split("\n\n");


        foreach (var inputString in inputStrings)
        {
            Monkey.ParseMonkeyString(inputString);
        }

        var roundCount = 10000;

        for (var i = 0; i < roundCount; i++)
        {
            // A round
            foreach (var monkey in Monkey.Monkeys)
            {
                monkey.Turn();
            }
        }

        foreach (var monkey in Monkey.Monkeys)
        {
            Console.WriteLine($"Monkey {monkey.Id} inspected {monkey.InspectCount} times");
        }

        var top2MonkeyInspects = Monkey.Monkeys
            .OrderByDescending(x => x.InspectCount).Take(2).Select(x => x.InspectCount);

        long longMonkeyInspectCount = top2MonkeyInspects.ElementAt(0) * top2MonkeyInspects.ElementAt(1);

        Console.WriteLine(longMonkeyInspectCount);
    }
}

public class Monkey
{
    public int Id;

    public Queue<Item> Items;

    public Operation Operation;

    public TestOperation TestOperation;

    public static List<Monkey> Monkeys = new List<Monkey>();

    public long InspectCount = 0;

    public static long ModuloNumber = 1;

    public Monkey(int id, Queue<Item> items, Operation operation, TestOperation testOperation)
    {
        Id = id;
        Items = items;
        Operation = operation;
        TestOperation = testOperation;
    }

    public void Turn()
    {
        while (Items.TryDequeue(out var inspectItem))
        {
            InspectCount++;
            inspectItem.WorryLevel = Operation.Result(inspectItem.WorryLevel);

            Console.WriteLine(inspectItem.WorryLevel);

            // Limit worry levels
            inspectItem.WorryLevel %= ModuloNumber;

            //inspectItem.WorryLevel = (int)Math.Floor((double)inspectItem.WorryLevel / 3);
            ThrowToMonkey(inspectItem, TestOperation.GetTarget(inspectItem.WorryLevel));
        }
    }

    public static Monkey GetMonkeyById(int id)
    {
        return Monkeys.First(x => x.Id == id);
    }

    public void ThrowToMonkey(Item item, int id)
    {
        GetMonkeyById(id).Items.Enqueue(item);
    }

    public static Monkey ParseMonkeyString(string monkeyString)
    {
        var monkeyStringLines = monkeyString.Split('\n');

        int id = 0;
        var items = new Queue<Item>();
        Operation operation = null;
        TestOperation testOperation = null;

        foreach (var monkeyStringLine in monkeyStringLines)
        {
            var monkeyLine = monkeyStringLine.Trim();
            if (monkeyLine.StartsWith("Monkey"))
                id = int.Parse(monkeyLine.Split(" ")[1].Replace(":", ""));

            if (monkeyLine.StartsWith("Starting items:"))
            {
                var startItems = monkeyLine.Replace("Starting items: ", "").Split(", ");
                foreach (var startItem in startItems)
                {
                    items.Enqueue(new Item(long.Parse(startItem)));
                }
            }

            if (monkeyLine.StartsWith("Operation"))
                operation = Operation.ParseOperation(monkeyLine);

            if (monkeyLine.StartsWith("Test"))
                testOperation = TestOperation.ParseTestOperationString(monkeyLine);

            if (monkeyLine.StartsWith("If "))
                testOperation.ParseResultTarget(monkeyLine);
        }

        var monkey = new Monkey(id, items, operation, testOperation);
        Monkeys.Add(monkey);
        return monkey;
    }
}

public class Item
{
    public long WorryLevel;

    public Item(long worryLevel)
    {
        WorryLevel = worryLevel;
    }
}

public class Operation
{
    public int? Operand2;

    public char Op;

    public static Operation ParseOperation(string stringOperation)
    {
        var operation = stringOperation.Split('=')[1].Trim();
        var op = operation.Split(' ')[1].ToCharArray().First();
        int? operand2;
        if (operation.Split(' ')[2] == "old")
        {
            operand2 = null;
        }
        else
        {
            operand2 = int.Parse(operation.Split(' ')[2]);
        }
        return new Operation(op, operand2);
    }

    public Operation(char op, int? operand2 = null)
    {
        Op = op;
        Operand2 = operand2;
    }

    public long Result(long old)
    {
        var op2 = Operand2 ?? old;
        return Op switch
        {
            '+' => old + op2,
            '-' => old - op2,
            '*' => old * op2,
            _ => old,
        };
    }
}

public class TestOperation
{
    public int Divisor;

    public int TrueTarget;

    public int FalseTarget;

    public TestOperation(int divisor)
    {
        Divisor = divisor;
    }

    public static TestOperation ParseTestOperationString(string operationString)
    {
        var divisor = int.Parse(operationString.Replace("Test: divisible by ", ""));
        Monkey.ModuloNumber *= divisor;
        return new TestOperation(divisor);
    }

    public void ParseResultTarget(string resultTargetString)
    {
        //"If false: throw to monkey 1";
        var boolTarget = resultTargetString.Replace("If ", "");

        var result = boolTarget.StartsWith("true") && !boolTarget.StartsWith("false");

        var target = int.Parse(boolTarget.Replace(": throw to monkey", "").Split(" ")[1]);

        if (result)
        {
            TrueTarget = target;
        }
        else
        {
            FalseTarget = target;
        }
    }

    public bool Result(long value)
    {
        return value % Divisor == 0;
    }

    public int GetTarget(long value)
    {
        if (Result(value))
        {
            return TrueTarget;
        }
        return FalseTarget;
    }
}