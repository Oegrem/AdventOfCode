using System.Numerics;

namespace Common.InputHelper;
public static class StringExtension
{
	public static bool ContainsAny(this string input, params string[] checks)
	{
		foreach(var check in checks)
		{
			if (input.Contains(check)) return true;
		}
		return false;
	}

	public static string RemoveEmptyLines(this string input)
	{
		var lines = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
		   .Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))
		   .ToArray();

		return string.Join('\n', lines);
	}

	public static int MHDistance(this Vector2 v1, Vector2 v2)
	{
		return (int)(Math.Abs(v1.X-v2.X)+Math.Abs(v1.Y-v2.Y));
	}

	public static bool IsInRange(this Vector2 v1, Vector2 v2, int distance)
	{
		return v1.MHDistance(v2) <= distance;
	}

	public static Vector2 GetVectorDirection(this Vector2 v1, Vector2 v2)
	{
		return new Vector2(Math.Clamp(v2.X - v1.X, -1, 1), Math.Clamp(v2.Y - v1.Y, -1, 1));
	}

	public static List<Vector2> GetAllVectorsBetween(this Vector2 v1, Vector2 v2)
	{
		var direction = GetVectorDirection(v1, v2);

		var vectorList = new List<Vector2>();

		for(var start = v1; start != v2; start += direction)
		{
			vectorList.Add(start);
		}

		return vectorList;
	}

}
