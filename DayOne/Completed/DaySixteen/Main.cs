using AdventOfCode2022.InputHelper;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace Puzzles.Completed.DaySixteen;

public class Main
{
    const string FOLDER = "DaySixteen";

    public static async Task SolvePuzzle()
    {
        var inputStrings = await InputHandler.GetStringFromInputFile(FOLDER);
        var sampleLines = await InputHandler.GetStringFromSample(FOLDER);

        //Game.ParseInput(sampleLines);
        Game.ParseInput(inputStrings);
    }
}

public class Game
{

    public static List<(bool[] openValves, int pressure)> Paths = new();

    public static Dictionary<BitArray, int> Paths2 = new();

    public static Dictionary<int, int> valveFlow = new();

    public static int Release = 0;
    public static void ParseInput(string input)
    {
        var cleanedInput = input.Replace(" tunnels lead to valves ", "")
            .Replace(" tunnel leads to valve ", "")
            .Replace("Valve ", "")
            .Replace(" has flow rate=", ",").Replace(", ", ",");
        var inputLines = InputHandler.GetStringLines(cleanedInput);

        var valveDictionary = new Dictionary<string, (int flow, string[] keys)>();
        var keyId = new Dictionary<string, int>();

        var i = 0;

        var usefulValves = new List<int>();

        foreach (var inputLine in inputLines)
        {
            var splitLine = inputLine.Split(";");
            var valveData = splitLine[0].Split(",");

            var flow = int.Parse(valveData[1]);

            if (flow > 0)
                usefulValves.Add(i);

            //valveDictionary.Add(valveData[0], (int.Parse(valveData[1]), splitLine[1].Split(",")));
            valveDictionary.Add(valveData[0], (flow, splitLine[1].Split(",")));
            keyId.Add(valveData[0], i);
            valveFlow.Add(i, flow);
            i++;
        }

        var V = valveDictionary.Count;

        // initialize minimum distance array initialized to infinity (-1)
        var minimumDistances = new int[V][];
        for (i = 0; i < V; i++)
        {
            minimumDistances[i] = new int[V];

            for (int b = 0; b < V; b++)
            {
                minimumDistances[i][b] = 100000;
            }
        }

        // add direct edges with distance/weight (here all 1)
        foreach (var entry in valveDictionary)
        {
            keyId.TryGetValue(entry.Key, out var u);

            foreach (var vKey in entry.Value.keys)
            {
                keyId.TryGetValue(vKey, out var v);
                minimumDistances[u][v] = 1;
            }
        }

        foreach (var entry in valveDictionary)
        {
            keyId.TryGetValue(entry.Key, out var u);

            // set distance to itself to 0
            minimumDistances[u][u] = 0;
        }

        //for (var a = 0; a < V; a++)
        //{
        //	for (var b = 0; b < V; b++)
        //	{
        //		var o = minimumDistances[a][b];
        //		if (o == 100000)
        //		{
        //			Console.Write('.');
        //			continue;
        //		}
        //		Console.Write(o);
        //	}
        //	Console.WriteLine();
        //}

        for (var k = 0; k < V; k++)
        {
            for (i = 0; i < V; i++)
            {
                for (var j = 0; j < V; j++)
                {
                    if (minimumDistances[i][j] > minimumDistances[i][k] + minimumDistances[k][j])
                    {
                        minimumDistances[i][j] = minimumDistances[i][k] + minimumDistances[k][j];
                    }
                }
            }
        }

        //for(var a = 0; a < V; a++)
        //{
        //	for(var b = 0; b < V; b++)
        //	{
        //		var o = minimumDistances[a][b];
        //		if(o == 100000)
        //		{
        //			Console.Write('.');
        //			continue;
        //		}
        //		Console.Write(o);
        //	}
        //	Console.WriteLine();
        //}



        Console.WriteLine(Solve3(keyId["AA"], 26, new bool[V], usefulValves, minimumDistances, 0));
        Console.WriteLine(Solve4(keyId["AA"], 26, new BitArray(V), usefulValves, minimumDistances, 0));

        Console.WriteLine(Paths.Count());

        var workedThrough = 0;

        var maxCombinedPressure = new List<int>();
        //foreach (var path in Paths)
        //{
        //	var nonMatchingPaths = Paths.Where(p => !HasSameOpenValve(path.openValves, p.openValves));
        //	if (!nonMatchingPaths.Any())
        //	{
        //		continue;
        //	}
        //	maxCombinedPressure.Add(path.pressure + nonMatchingPaths.Max(x => x.pressure));

        //	workedThrough++;
        //	if (workedThrough % 100 == 0)
        //		Console.WriteLine(workedThrough);
        //	//Console.WriteLine(path.pressure + nonMatchingPaths.Max(x => x.pressure));
        //}


        //var maxCombinedPressure2 = new List<int>();
        //foreach (var entry in Paths2)
        //{
        //	var nonMatchingPaths = Paths2.Where(p => ((BitArray)p.Key.Clone()).And(entry.Key).Cast<bool>().Any(x => x));
        //	if (!nonMatchingPaths.Any())
        //	{
        //		continue;
        //	}
        //	maxCombinedPressure2.Add(entry.Value + nonMatchingPaths.Max(x => x.Value));
        //	//Console.WriteLine(path.pressure + nonMatchingPaths.Max(x => x.pressure));
        //}

        Parallel.ForEach(Paths, path =>
        {
            var nonMatchingPaths = Paths.Where(p => !HasSameOpenValve(path.openValves, p.openValves));
            if (nonMatchingPaths.Any())
            {
                maxCombinedPressure.Add(path.pressure + nonMatchingPaths.Max(x => x.pressure));
                workedThrough++;
                if (workedThrough % 100 == 0)
                    Console.WriteLine(workedThrough);
            }
        });

        Console.WriteLine(maxCombinedPressure.Max());
        //Console.WriteLine(maxCombinedPressure2.Max());
    }

    public static bool HasSameOpenValve(bool[] first, bool[] second)
    {
        for (var i = 0; i < first.Count(); i++)
        {
            if (first[i] && second[i]) return true;
        }
        return false;
    }


    public static int Solve3(int currentValve, int minute, bool[] isValveOpen,
        List<int> usefulValves, int[][] minimumDistance, int pressure)
    {
        //var pressure = 0;


        var targetIndices = usefulValves.Where(x => x != currentValve).Where(x => !isValveOpen[x]);

        foreach (var targetIndex in targetIndices)
        {
            var cost = minimumDistance[currentValve][targetIndex];

            // doesnt make sense to move
            if (cost >= minute)
            {
                //Paths.Add(((bool[])isValveOpen.Clone(), pressure));

                continue;
            }

            var isOpenClone = (bool[])isValveOpen.Clone();
            isOpenClone[targetIndex] = true;

            var timeAfterOpen = minute - cost - 1;

            var next_pressure = valveFlow[targetIndex] * timeAfterOpen;

            //pressure = Math.Max(pressure, next_pressure + Solve3(targetIndex, timeAfterOpen, isOpenClone,
            //	usefulValves, minimumDistance));

            Solve3(targetIndex, timeAfterOpen, isOpenClone,
                usefulValves, minimumDistance, pressure + next_pressure);

            // after opening valve:
            // isValveOpen[index] = false;
            // bool[])isValveOpen.Clone()
        }
        Paths.Add(((bool[])isValveOpen.Clone(), pressure));

        return pressure;

    }

    public static int Solve4(int currentValve, int minute, BitArray isValveOpen,
    List<int> usefulValves, int[][] minimumDistance, int pressure)
    {
        //var pressure = 0;


        var targetIndices = usefulValves.Where(x => x != currentValve).Where(x => !isValveOpen[x]);

        foreach (var targetIndex in targetIndices)
        {
            var cost = minimumDistance[currentValve][targetIndex];

            // doesnt make sense to move
            if (cost >= minute)
            {
                //Paths.Add(((bool[])isValveOpen.Clone(), pressure));

                continue;
            }

            var isOpenClone = new BitArray(isValveOpen);

            isOpenClone[targetIndex] = true;

            var timeAfterOpen = minute - cost - 1;

            var next_pressure = valveFlow[targetIndex] * timeAfterOpen;

            //pressure = Math.Max(pressure, next_pressure + Solve3(targetIndex, timeAfterOpen, isOpenClone,
            //	usefulValves, minimumDistance));

            Solve4(targetIndex, timeAfterOpen, isOpenClone,
                usefulValves, minimumDistance, pressure + next_pressure);

            // after opening valve:
            // isValveOpen[index] = false;
            // bool[])isValveOpen.Clone()
        }
        if (!Paths2.TryGetValue(isValveOpen, out var existingPressure))
        {
            Paths2.Add(isValveOpen, pressure);
        }
        else if (existingPressure < pressure)
            Paths2[isValveOpen] = pressure;

        return pressure;

    }
}