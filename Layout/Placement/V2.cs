using System;

namespace Layout.Placement
{
	public struct V2
	{
		public readonly int X;
		public readonly int Y;

		public V2(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
		public static bool operator ==(V2 a, V2 b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(V2 a, V2 b) => a.X != b.X || a.Y != b.Y;
		public static V2 operator +(V2 a, V2 b) => (a.X + b.X, a.Y + b.Y);
		public static V2 operator -(V2 a, V2 b) => (a.X - b.X, a.Y - b.Y);
		public static implicit operator V2((int x, int y) v) => new V2(v.x, v.y);
		public override string ToString() => $"({X}, {Y})";

		public override bool Equals(object obj) => obj is V2 v && X == v.X && Y == v.Y;
		public override int GetHashCode() => HashCode.Combine(X, Y);
	}
}