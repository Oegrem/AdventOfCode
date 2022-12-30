using AdventOfCode2022.InputHelper;
using Common.Algorithm;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Puzzles.Completed.DayNineteen;

public class Main
{
    const string FOLDER = "DayNineteen";

    const bool USE_SAMPLE = true;

    public static List<Blueprint> Blueprints = new();

    public static List<int> GeodeResults = new();

    public const int MINUTES = 32;

    public static async Task SolvePuzzle()
    {
        var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));
        if (USE_SAMPLE)
            inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromSample(FOLDER));

        ParseBueprints(inputStrings);

        var quality = 1;

        Parallel.ForEach(Blueprints.Take(3), bp =>
        {
            var bfs = new BFS(bp);
            bfs.SearchDeepRecursive2(MINUTES, 0, 0, 0, 0, 1, 0, 0, 0);
            Console.WriteLine(bfs.maxGeodes);
            quality *= bfs.maxGeodes;
        });

        Console.WriteLine(quality);
    }

    public static void ParseBueprints(string[] input)
    {
        foreach (var line in input)
        {
            var filteredString = new string(line.Where(c => char.IsDigit(c) || char.IsWhiteSpace(c)).ToArray());
            var numbers = Regex.Replace(filteredString, @"\s+", " ").Trim().Split(" ").Select(num => byte.Parse(num)).ToArray();

            var newBp = new Blueprint()
            {
                id = numbers[0],
                oreRobotOre = numbers[1],
                clayRobotOre = numbers[2],
                obsRobotOre = numbers[3],
                obsRobotClay = numbers[4],
                geoRobotOre = numbers[5],
                geoRobotObs = numbers[6]
            };

            newBp.maxOreCost = new int[] { newBp.oreRobotOre, newBp.clayRobotOre, newBp.obsRobotOre, newBp.geoRobotOre }.Max();

            Blueprints.Add(newBp);
        }
    }
}

public class BFS
{
    public Blueprint Blueprint { get; set; }

    public int maxGeodes = 0;

    public BFS(Blueprint bp)
    {
        Blueprint = bp;
    }

    public void SearchDeepRecursive2(int minute, int ore, int clay, int obs, int geo,
        int ore_r, int clay_r, int obs_r, int geo_r)
    {

        int nextMinute = minute - 1;
        int nextOre = ore + ore_r;
        int nextClay = clay + clay_r;
        int nextObs = obs + obs_r;
        int nextGeo = geo + geo_r;

        if (geo > maxGeodes)
            maxGeodes = geo;

        if (minute <= 0)
        {
            return;
        }
        //geodes collected +geode robots* time remaining + T(time remaining) <= best total geodes found so far.
        if (geo + geo_r * minute + TriangularNumber(minute - 1) <= maxGeodes)
        {
            return;
        }

        // Get maximum possible amount 
        if (obs_r >= Blueprint.geoRobotObs && ore_r >= Blueprint.geoRobotOre)
        {
            var maxG = geo + geo_r * minute + TriangularNumber(minute - 1);
            if (maxG > maxGeodes)
                maxGeodes = maxG;

            return;
        }

        // Save ( change to jump to one of the three robots )

        // jump to 

        //SearchDeepRecursive2(
        //	nextMinute,
        //	nextOre,
        //	nextClay,
        //	nextObs,
        //	nextGeo,
        //	ore_r,
        //	clay_r,
        //	obs_r,
        //	geo_r
        //);


        // Create Ore Robot
        if (ore_r < Blueprint.maxOreCost)
        {
            if (ore >= Blueprint.oreRobotOre)
            {
                SearchDeepRecursive2(
                    nextMinute,
                    nextOre - Blueprint.oreRobotOre,
                    nextClay,
                    nextObs,
                    nextGeo,
                    ore_r + 1,
                    clay_r,
                    obs_r,
                    geo_r
                );
            }
            else
            {
                var neededTime = (int)Math.Ceiling((decimal)((Blueprint.oreRobotOre - nextOre) / ore_r));
                if (nextMinute - neededTime >= 2)
                {
                    SearchDeepRecursive2(
                        nextMinute - neededTime,
                        nextOre + ore_r * neededTime,
                        nextClay + clay_r * neededTime,
                        nextObs + obs_r * neededTime,
                        nextGeo + geo_r * neededTime,
                        ore_r + 1,
                        clay_r,
                        obs_r,
                        geo_r
                    );
                }
            }
        }

        // Create Clay Robot
        if (clay_r < Blueprint.obsRobotClay)
        {
            if (ore >= Blueprint.clayRobotOre)
            {
                SearchDeepRecursive2(
                    nextMinute,
                    nextOre - Blueprint.clayRobotOre,
                    nextClay,
                    nextObs,
                    nextGeo,
                    ore_r,
                    clay_r + 1,
                    obs_r,
                    geo_r
                );
            }
            else
            {
                var neededTime = (int)Math.Ceiling((decimal)((Blueprint.clayRobotOre - nextOre) / ore_r));
                if (nextMinute - neededTime >= 3)
                {
                    SearchDeepRecursive2(
                        nextMinute - neededTime,
                        nextOre + ore_r * neededTime,
                        nextClay + clay_r * neededTime,
                        nextObs + obs_r * neededTime,
                        nextGeo + geo_r * neededTime,
                        ore_r,
                        clay_r + 1,
                        obs_r,
                        geo_r
                    );
                }
            }
        }
        // Create Obs Robot
        if (obs_r < Blueprint.geoRobotObs)
        {
            if (ore >= Blueprint.obsRobotOre && clay >= Blueprint.obsRobotClay)
            {
                SearchDeepRecursive2(
                    nextMinute,
                    nextOre - Blueprint.obsRobotOre,
                    nextClay - Blueprint.obsRobotClay,
                    nextObs,
                    nextGeo,
                    ore_r,
                    clay_r,
                    obs_r + 1,
                    geo_r
                );
            }
            else if (clay_r > 0)
            {
                var neededTime1 = (int)Math.Ceiling((decimal)((Blueprint.obsRobotOre - nextOre) / ore_r));
                var neededTime2 = (int)Math.Ceiling((decimal)((Blueprint.obsRobotClay - nextClay) / clay_r));
                var neededTime = Math.Max(neededTime1, neededTime2);
                if (nextMinute - neededTime >= 2)
                {
                    SearchDeepRecursive2(
                        nextMinute - neededTime,
                        nextOre + ore_r * neededTime,
                        nextClay + clay_r * neededTime,
                        nextObs + obs_r * neededTime,
                        nextGeo + geo_r * neededTime,
                        ore_r,
                        clay_r,
                        obs_r + 1,
                        geo_r
                    );
                }
            }
        }

        // Create Geo Robot
        if (nextOre >= Blueprint.geoRobotOre && obs >= Blueprint.geoRobotObs)
        {
            SearchDeepRecursive2(
                nextMinute,
                nextOre - Blueprint.geoRobotOre,
                nextClay,
                nextObs - Blueprint.geoRobotObs,
                nextGeo,
                ore_r,
                clay_r,
                obs_r,
                geo_r + 1
            );
        }
        else if (obs_r > 0)
        {
            var neededTime1 = (int)Math.Ceiling((decimal)((Blueprint.geoRobotOre - nextOre) / ore_r));
            var neededTime2 = (int)Math.Ceiling((decimal)((Blueprint.geoRobotObs - nextObs) / obs_r));
            var neededTime = Math.Max(neededTime1, neededTime2);
            if (nextMinute - neededTime >= 1)
            {
                SearchDeepRecursive2(
                    nextMinute - neededTime,
                    nextOre + ore_r * neededTime,
                    nextClay + clay_r * neededTime,
                    nextObs + obs_r * neededTime,
                    nextGeo + geo_r * neededTime,
                    ore_r,
                    clay_r,
                    obs_r,
                    geo_r + 1
                );
            }
        }
    }

    public static int TriangularNumber(int x) => x * (x + 1) / 2;

}
public class Blueprint
{
    public int id;
    public int oreRobotOre;
    public int clayRobotOre;
    public int obsRobotOre;
    public int obsRobotClay;
    public int geoRobotOre;
    public int geoRobotObs;

    public int maxOreCost;
}