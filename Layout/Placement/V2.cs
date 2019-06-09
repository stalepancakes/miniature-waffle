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

		public static implicit operator V2((int x, int y) v) => new V2(v.x, v.y);
		public override string ToString() => $"({X}, {Y})";
	}
}