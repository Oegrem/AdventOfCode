using System.Data;

namespace Common.Algorithm;
public class Vector2Int 
{
	public int X { get; set; }
	public int Y { get; set; }

	public Vector2Int N { get => new Vector2Int(X, Y-1); }
	public Vector2Int E { get => new Vector2Int(X+1, Y); }
	public Vector2Int S { get => new Vector2Int(X, Y+1); }
	public Vector2Int W { get => new Vector2Int(X-1, Y); }

	public Vector2Int(int x, int y)
	{
		X = x;
		Y = y;
	}

	public override bool Equals(object? obj)
	{
		return (
			obj != null &&
			obj is Vector2Int vectorObj &&
			X == vectorObj.X &&
			Y == vectorObj.Y
		);
	}

	public override string ToString()
	{
		return $"({X}, {Y})";
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}

	public static Vector2Int operator +(Vector2Int a) => a; 
	public static Vector2Int operator -(Vector2Int a) => new Vector2Int(-a.X, -a.Y);
	public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new Vector2Int(a.X+b.X, a.Y+b.Y);
	public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new Vector2Int(a.X-b.X, a.Y-b.Y);
	public static Vector2Int operator *(Vector2Int a, Vector2Int b) => new Vector2Int(a.X*b.X, a.Y*b.Y);

	public static Vector2Int operator +(Vector2Int a, (int x, int y) b) => new Vector2Int(a.X+b.x, a.Y+b.y);
	public static Vector2Int operator -(Vector2Int a, (int x, int y) b) => new Vector2Int(a.X-b.x, a.Y-b.y);
}
