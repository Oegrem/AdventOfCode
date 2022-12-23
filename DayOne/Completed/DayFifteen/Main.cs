using AdventOfCode2022.InputHelper;
using Common.InputHelper;
using System.ComponentModel;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Puzzles.Completed.DayFifteen;

public class Main
{
    const string FOLDER = "DayFifteen";

    public static async Task SolvePuzzle()
    {
        var inputStrings = await InputHandler.GetStringFromInputFile(FOLDER);
        //var inputStrings = await InputHandler.GetStringFromSample(FOLDER);

        var sn = SensorNetwork.ParseInputString(inputStrings);

        //Console.WriteLine(sn.GetCoveredArea(10));
        //Console.WriteLine(sn.GetCoveredArea(2000000));

        long min = 0;
        //long max = 20;
        long max = 4000000;

        //for (var i = min; i <= max; i++)
        //{
        //	Console.WriteLine(sn.GetFreeArea(i, min, max));
        //}
        var res = sn.GetFreeArea((int)min, (int)max).DistinctBy(v => (v.X, v.Y)).First();
        Console.WriteLine($"x={res.X} y={res.Y}");

        //max = 4000000;

        var resNum = (long)res.X * max + (long)res.Y;

        Console.WriteLine($"Result = {resNum}");


    }
}

public class SensorNetwork
{
    public List<Sensor> Sensors = new List<Sensor>();

    public List<Vector2> Beacons = new List<Vector2>();

    public List<Vector2> GetFreeArea(int min, int max)
    {
        //var coveredTiles = new List<Vector2>();
        //foreach (var sensor in Sensors)
        //{
        //	coveredTiles.AddRange(sensor.GetCoveredRowArea(y));
        //}
        //return (max - min + 1) - coveredTiles.Distinct()
        //	.Where(v => v.X >= min && v.X <= max)
        //	.Count();

        // Get all Edge Vectors
        var vList = new List<Vector2>();
        foreach (var sensor in Sensors)
        {
            vList.AddRange(sensor.GetAdjacentVectors()
                .Where(v => v.X >= min && v.X <= max && v.Y >= min && v.Y <= max));
        }

        //Draw(min, max, vList);
        //Console.WriteLine();

        foreach (var sensor in Sensors)
        {
            vList = sensor.FilterVectorsInRange(vList);
        }

        //Draw(min, max, vList);
        //Console.WriteLine();

        return vList;
    }

    public void Draw(int min, int max, List<Vector2> vectors)
    {
        for (var y = min; y <= max; y++)
        {
            for (var x = min; x <= max; x++)
            {
                if (vectors.Any(v => v.X == x && v.Y == y))
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
    }

    public int GetCoveredArea(int y)
    {
        var coveredTiles = new List<Vector2>();
        foreach (var sensor in Sensors)
        {
            coveredTiles.AddRange(sensor.GetCoveredRowArea(y));
        }
        return coveredTiles.Distinct().Where(v => !Beacons.Contains(v))
            .Count();
    }

    public static SensorNetwork ParseInputString(string inputString)
    {
        var sensorNetwork = new SensorNetwork();
        var inputLines = InputHandler.GetStringLines(inputString.Replace("Sensor at ", "")
            .Replace("x=", "").Replace("y=", ""));
        foreach (var line in inputLines)
        {
            var sensorData = line.Split(": closest beacon is at ");


            var newSensor = new Sensor
            {
                // Sensor Position
                Position = Sensor.ReadPositionFromString(sensorData[0]),

                // Beacon Position
                BeaconPosition = Sensor.ReadPositionFromString(sensorData[1])
            };

            sensorNetwork.Beacons.Add(Sensor.ReadPositionFromString(sensorData[1]));

            newSensor.SetRange();

            sensorNetwork.Sensors.Add(newSensor);
        }
        return sensorNetwork;
    }
}

public class Sensor
{
    public Vector2 Position;
    public Vector2 BeaconPosition;
    public int Range;

    public static Vector2 ReadPositionFromString(string position)
    {
        var xyStrings = position.Split(", ");
        return new Vector2(int.Parse(xyStrings[0]), int.Parse(xyStrings[1]));
    }

    public void SetRange()
    {
        Range = Position.MHDistance(BeaconPosition);
    }

    public List<Vector2> GetAdjacentVectors()
    {
        var corners = new Vector2[4];

        corners[0] = new Vector2(Position.X - Range - 1, Position.Y);
        corners[1] = new Vector2(Position.X, Position.Y - Range - 1);
        corners[2] = new Vector2(Position.X + Range + 1, Position.Y);
        corners[3] = new Vector2(Position.X, Position.Y + Range + 1);

        var vectorList = new List<Vector2>();

        vectorList.AddRange(corners[0].GetAllVectorsBetween(corners[1]));
        vectorList.AddRange(corners[1].GetAllVectorsBetween(corners[2]));
        vectorList.AddRange(corners[2].GetAllVectorsBetween(corners[3]));
        vectorList.AddRange(corners[3].GetAllVectorsBetween(corners[0]));

        return vectorList.Distinct().ToList();
    }

    public List<Vector2> FilterVectorsInRange(List<Vector2> list)
    {
        return list.Where(v => !Position.IsInRange(v, Range)).ToList();
    }

    public Vector2[] GetBoundaryBox()
    {
        var corners = new Vector2[4];

        corners[0] = new Vector2(Position.X - Range, Position.Y);
        corners[1] = new Vector2(Position.X, Position.Y - Range);
        corners[2] = new Vector2(Position.X + Range, Position.Y);
        corners[3] = new Vector2(Position.X, Position.Y + Range);

        return corners;
    }

    public (int? Left, int? Right) GetCoveredEdge(int y)
    {
        if (!Position.IsInRange(new Vector2(Position.X, y), Range))
            return (null, null);

        var rowDistance = Math.Abs(Position.Y - y);
        var coveredRange = Range - rowDistance;

        var Left = (int)(Position.X - coveredRange);
        var Right = (int)(Position.X + coveredRange);
        return (Left, Right);
    }

    public List<Vector2> GetCoveredRowArea(int y)
    {
        var rowArea = new List<Vector2>();
        if (!Position.IsInRange(new Vector2(Position.X, y), Range))
            return rowArea;

        var rowDistance = Math.Abs(Position.Y - y);
        var coveredRange = Range - rowDistance;
        for (var i = -coveredRange; i <= coveredRange; i++)
        {
            rowArea.Add(new Vector2(Position.X + i, y));
        }

        return rowArea;
    }

}