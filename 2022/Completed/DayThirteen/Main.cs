using AdventOfCode2022.InputHelper;
using Common.InputHelper;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;
using System.Text.Json.Serialization;

namespace Puzzles.Completed.DayThirteen;

public class Main
{
    const string FOLDER = "DayThirteen";


    public static async Task SolvePuzzle()
    {
        //await SolvePuzzlePart1();
        await SolvePuzzlePart2();
    }

    public static async Task SolvePuzzlePart1()
    {
        var inputStringObjects = (await InputHandler.GetStringFromInputFile(FOLDER)).Split("\n\n");

        int i = 1;

        int indiceSum = 0;

        foreach (var inputString in inputStringObjects)
        {
            var leftAndRightStrings = inputString.Split("\n");
            var left = PacketObjectParser.ParsePackageString(leftAndRightStrings[0]);
            var right = PacketObjectParser.ParsePackageString(leftAndRightStrings[1]);

            // compare
            var comparer = new PacketObjectComparer(left, right);
            var result = comparer.RunCompare();
            Console.WriteLine($"{i} {result}");

            if (result) indiceSum += i;

            i++;
        }
        Console.WriteLine(indiceSum);
    }

    public static async Task SolvePuzzlePart2()
    {
        var inputStringObjects = InputHandler.GetStringLines(
            (await InputHandler.GetStringFromInputFile(FOLDER)).RemoveEmptyLines());

        //inputStringObjects = InputHandler.GetStringLines(
        //	(await InputHandler.GetStringFromSample(FOLDER)).RemoveEmptyLines());


        var packList = new List<PacketList>();

        foreach (var inputString in inputStringObjects)
        {
            packList.Add((PacketList)PacketObjectParser
                .ParsePackageString(inputString));

            //var stuff = JsonConvert.DeserializeObject(inputString);
            //Console.WriteLine(stuff);
        }


        packList.Add((PacketList)PacketObjectParser
                .ParsePackageString("[[2]]"));
        packList.Add((PacketList)PacketObjectParser
                .ParsePackageString("[[6]]"));

        packList.Sort();

        var decoder1 = 0;
        var decoder2 = 0;

        decoder1 = packList.FindIndex(x => x.ToString() == "[[2]]") + 1;
        decoder2 = packList.FindIndex(x => x.ToString() == "[[6]]") + 1;

        Console.WriteLine($"{decoder1} * {decoder2} = {decoder1 * decoder2}");

        Console.WriteLine();
    }
}

public class PacketObjectParser
{
    private int _charPointer;
    private string _packetString;

    public PacketObjectParser(string packetString)
    {
        _packetString = packetString;
        _charPointer = 1;
    }

    public static IPacketObject ParsePackageString(string packetString)
    {
        var packetParser = new PacketObjectParser(packetString);
        return packetParser.RecursivelyParsePacketObject();
    }

    public IPacketObject RecursivelyParsePacketObject()
    {
        var packetList = new PacketList();

        var nextChar = _packetString[_charPointer];
        while (nextChar != ']')
        {
            if (nextChar == ',')
            {
                _charPointer += 1;
                nextChar = _packetString[_charPointer];
            }

            if (!isCharSeparator(nextChar))
            {
                var intString = "";
                while (!isCharSeparator(nextChar))
                {
                    intString += nextChar;
                    _charPointer += 1;
                    nextChar = _packetString[_charPointer];
                }
                packetList.ChildObjects.Add(new PacketInt(int.Parse(intString)));
                //continue;
            }

            if (nextChar == '[')
            {
                _charPointer += 1;
                packetList.ChildObjects.Add(RecursivelyParsePacketObject());
                nextChar = _packetString[_charPointer];
            }
        }
        _charPointer += 1;
        return packetList;
    }

    public bool isCharSeparator(char c)
    {
        return c == ',' || c == '[' || c == ']';
    }
}


public class PacketObjectComparer
{
    private readonly IPacketObject Left;
    private readonly IPacketObject Right;

    public PacketObjectComparer(IPacketObject left, IPacketObject right)
    {
        Left = left;
        Right = right;
    }

    public static bool RunCompare(PacketList left, PacketList right)
    {
        var comparer = new PacketObjectComparer(left, right);
        return comparer.RunCompare();
    }

    public bool RunCompare()
    {
        var compareResult = CompareLists((PacketList)Left, (PacketList)Right);
        if (compareResult == null)
        {
            //return true;
            throw new Exception("Could not make out result");
        }
        return compareResult.Value;
    }

    public bool? CompareInt(int leftInt, int rightInt)
    {
        // if they are the same => check further
        if (leftInt == rightInt) return null;

        // If left is lower => right order
        // if left is higher => wrong order
        return leftInt < rightInt;
    }

    public bool? CompareLists(PacketList leftList, PacketList rightList)
    {
        var leftListCount = leftList.ChildObjects.Count;
        var rightListCount = rightList.ChildObjects.Count;

        var listResult = CompareInt(leftListCount, rightListCount);

        for (var i = 0; i < Math.Min(leftListCount, rightListCount); i++)
        {
            var leftListItem = leftList.ChildObjects.ElementAt(i);
            var rightListItem = rightList.ChildObjects.ElementAt(i);

            if (leftListItem.IsInt())
            {
                if (rightListItem.IsInt())
                {
                    var compareResult = CompareInt(
                        ((PacketInt)leftListItem).Value,
                        ((PacketInt)rightListItem).Value
                    );
                    if (compareResult != null) return compareResult.Value;
                }
                else
                {
                    var typeConverted = new PacketList();
                    typeConverted.ChildObjects.Add(leftListItem);
                    var compareResult = CompareLists(typeConverted, (PacketList)rightListItem);
                    if (compareResult != null) return compareResult.Value;
                }
            }
            else
            {
                if (rightListItem.IsList())
                {
                    var compareResult = CompareLists((PacketList)leftListItem, (PacketList)rightListItem);
                    if (compareResult != null) return compareResult.Value;
                }
                else
                {
                    var typeConverted = new PacketList();
                    typeConverted.ChildObjects.Add(rightListItem);
                    var compareResult = CompareLists((PacketList)leftListItem, typeConverted);
                    if (compareResult != null) return compareResult.Value;
                }
            }
        }
        return listResult;
    }
}

public interface IPacketObject
{
    public bool IsInt();
    public bool IsList();
}

public class PacketList : IPacketObject, IComparable<PacketList>
{
    public List<IPacketObject> ChildObjects = new();

    public int CompareTo(PacketList? other)
    {
        var result = PacketObjectComparer.RunCompare(this, other);
        if (result)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    public bool IsInt()
    {
        return false;
    }

    public bool IsList()
    {
        return true;
    }

    public override string ToString()
    {
        var returnString = "[";
        returnString += string.Join(',', ChildObjects.Select(x => x.ToString()));
        returnString += "]";
        return returnString;
    }
}

public class PacketInt : IPacketObject
{
    public int Value;

    public PacketInt(int value)
    {
        Value = value;
    }

    public bool IsInt()
    {
        return true;
    }

    public bool IsList()
    {
        return false;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
