using AdventOfCode2022.InputHelper;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Puzzles.Completed.DayEight;

public class Main
{
    const string FOLDER = "DayEight";

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));

        var map = new Map();
        map.ParseMap(inputStrings);
        Console.WriteLine(map.GetVisibleTrees().Count);

        //var tree = map.Trees.First(x => x.Position.X == 90 && x.Position.Y == 55);
        //map.GetScenicValue(tree);

        Console.WriteLine(map.GetBestScenicValue());
    }
}

public class Map
{
    public List<Tree> Trees = new List<Tree>();

    public Point Size = new Point();

    public void ParseMap(string[] stringMap)
    {
        Size.Y = stringMap.Length;
        Size.X = stringMap[0].Length;

        var rowId = 0;
        foreach (var row in stringMap)
        {
            ParseRow(rowId, row);
            rowId++;
        }
    }

    public void ParseRow(int rowId, string mapRow)
    {
        var columnId = 0;
        foreach (var treeChar in mapRow.ToCharArray())
        {
            AddTree((int)char.GetNumericValue(treeChar), new Point(columnId, rowId));
            columnId++;
        }
    }

    public void AddTree(int height, Point position)
    {
        Trees.Add(new Tree(height, position));
    }

    public int GetBestScenicValue()
    {
        return Trees.Max(x => GetScenicValue(x));
    }

    public int GetScenicValue(Tree tree)
    {
        var scenicValue = 0;

        scenicValue = GetLeftTrees(tree).OrderByDescending(x => x.Position.X).GetScenicValue(tree)
            + (GetLeftTrees(tree).IsBlockingTree(tree) ? 1 : 0);

        scenicValue *= GetRightTrees(tree).OrderBy(x => x.Position.X).GetScenicValue(tree)
            + (GetRightTrees(tree).IsBlockingTree(tree) ? 1 : 0);

        scenicValue *= GetUpTrees(tree).OrderByDescending(x => x.Position.Y).GetScenicValue(tree)
            + (GetUpTrees(tree).IsBlockingTree(tree) ? 1 : 0);

        scenicValue *= GetDownTrees(tree).OrderBy(x => x.Position.Y).GetScenicValue(tree)
            + (GetDownTrees(tree).IsBlockingTree(tree) ? 1 : 0);

        return scenicValue;
    }

    public List<Tree> GetVisibleTrees()
    {
        return Trees.Where(x => IsTreeVisible(x)).ToList();
    }

    public bool IsTreeVisible(Tree tree)
    {
        if (IsEdgeTree(tree)) return true;

        if (!GetLeftTrees(tree).IsBlockingTree(tree))
            return true;
        if (!GetRightTrees(tree).IsBlockingTree(tree))
            return true;
        if (!GetUpTrees(tree).IsBlockingTree(tree))
            return true;
        if (!GetDownTrees(tree).IsBlockingTree(tree))
            return true;

        return false;
    }

    public List<Tree> GetLeftTrees(Tree tree)
    {
        return GetTreeRow(tree).WithoutTree(tree).Where(x => x.Position.X < tree.Position.X).ToList();
    }

    public List<Tree> GetRightTrees(Tree tree)
    {
        return GetTreeRow(tree).WithoutTree(tree).Where(x => x.Position.X > tree.Position.X).ToList();
    }

    public List<Tree> GetUpTrees(Tree tree)
    {
        return GetTreeColumn(tree).WithoutTree(tree).Where(x => x.Position.Y < tree.Position.Y).ToList();
    }

    public List<Tree> GetDownTrees(Tree tree)
    {
        return GetTreeColumn(tree).WithoutTree(tree).Where(x => x.Position.Y > tree.Position.Y).ToList();
    }

    public List<Tree> GetTreeRow(Tree tree)
    {
        return GetRow(tree.Position.Y);
    }

    public List<Tree> GetTreeColumn(Tree tree)
    {
        return GetColumn(tree.Position.X);

    }

    public List<Tree> GetRow(int id)
    {
        return Trees.Where(x => x.Position.Y == id).ToList();
    }

    public List<Tree> GetColumn(int id)
    {
        return Trees.Where(x => x.Position.X == id).ToList();
    }

    public bool IsEdgeTree(Tree tree)
    {
        return tree.Position.X == 0 || tree.Position.Y == 0 ||
            tree.Position.X == Size.X - 1 || tree.Position.Y == Size.Y - 1;
    }
}

public class Tree
{
    public int Height;
    public Point Position;

    public Tree(int height, Point position)
    {
        Height = height;
        Position = position;
    }
}

public static class Extensions
{
    public static List<Tree> WithoutTree(this List<Tree> list, Tree tree)
    {
        return list.Where(x => x != tree).ToList();
    }

    public static bool IsBlockingTree(this List<Tree> list, Tree tree)
    {
        return list.Any(x => x.Height >= tree.Height);
    }

    public static int GetScenicValue(this IEnumerable<Tree> list, Tree tree)
    {
        return list.TakeWhile(x => x.Height < tree.Height).Count();
    }
}