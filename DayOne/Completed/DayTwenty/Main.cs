using AdventOfCode2022.InputHelper;
using System.Security.Cryptography;

namespace Puzzles.Completed.DayTwenty;

public class Main
{
    const string FOLDER = "DayTwenty";

    const bool USE_SAMPLE = false;

    public static Dictionary<int, Item> numbers = new();

    public const long dKey = 811589153;

    public const bool PART2 = true;

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));
        if (USE_SAMPLE)
            inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSample(FOLDER));

        //inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromFileName(FOLDER, "sample2.txt"));

        CreateLinkedList(inputStrings);

        //PrintNum();
        for (var m = 0; m < (PART2 ? 10 : 1); m++)
        {
            for (var i = 0; i < numbers.Keys.Count; i++)
            {
                MoveItem2(i);
            }
            Console.WriteLine($"Finished Round {m + 1}");
        }

        Console.WriteLine(GetItemAfterZero(1000));
        Console.WriteLine(GetItemAfterZero(2000));
        Console.WriteLine(GetItemAfterZero(3000));

        Console.WriteLine(GetItemAfterZero(1000) + GetItemAfterZero(2000) + GetItemAfterZero(3000));
    }

    public static void CreateLinkedList(string[] inputStrings)
    {
        var index = 0;
        foreach (var inputString in inputStrings)
        {
            var move = long.Parse(inputString.Trim());

            if (PART2)
                move *= dKey;

            numbers.Add(index, new Item(index, index - 1, index + 1, move));

            index++;
        }

        // Close circle
        numbers[0].left = index - 1;
        numbers[index - 1].right = 0;
    }

    public static long GetItemAfterZero(int moves)
    {
        var zeroItem = numbers.Where(x => x.Value.move == 0).Select(x => x.Value).FirstOrDefault();
        if (zeroItem == null)
        {
            Console.WriteLine("no zero found");
            return -1;
        }

        var targetItem = zeroItem;
        while (moves > 0)
        {
            targetItem = targetItem.RightItem;
            moves--;
        }

        return targetItem.move;
    }

    public static void MoveItem2(int index)
    {
        var item = numbers[index];

        var actualMove = item.move;

        // reverse int
        switch (Math.Sign(actualMove))
        {
            case 1:
                actualMove = actualMove % (numbers.Keys.Count - 1);
                break;
            case -1:
                actualMove = -(Math.Abs(item.move) % (numbers.Keys.Count - 1)) + numbers.Keys.Count - 1;
                break;
            default: return;
        }

        if (actualMove == 0) return;

        for (var i = actualMove; i != 0; i--)
        {
            // merge left and right of current item
            item.LeftItem.RightItem = item.RightItem;
            item.RightItem.LeftItem = item.LeftItem;

            // set item as right of right and left of rights right

            var pRR = item.RightItem.RightItem;

            item.RightItem.RightItem.LeftItem = item;
            item.RightItem.RightItem = item;

            item.LeftItem = item.RightItem;
            item.RightItem = pRR;
        }
    }

    public static void PrintNum()
    {
        var item = numbers[0];
        var count = 0;
        do
        {
            Console.WriteLine(item.move);
            item = item.RightItem;
            count++;
        } while (item.index != 0);
        Console.WriteLine("Count: " + count);
        Console.WriteLine();

    }
}

public class Item
{
    public int index;
    public int left;
    public int right;
    public long move;

    public Item RightItem { get => Main.numbers[right]; set => right = value.index; }
    public Item LeftItem { get => Main.numbers[left]; set => left = value.index; }

    public Item(int index, int left, int right, long move)
    {
        this.index = index;
        this.left = left;
        this.right = right;
        this.move = move;
    }
}