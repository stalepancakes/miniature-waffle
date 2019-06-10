using Layout.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Layout.Placement
{
	class Grid
	{
		Tile[,] m_Tiles;
		Item[,] m_Items;

		public readonly int Width;
		public readonly int Height;

		public Grid(int width, int height)
		{
			this.Width = width;
			this.Height = height;

			m_Tiles = new Tile[width, height];
			m_Items = new Item[width, height];
		}

		public (Tile Tile, Item Type) this[int x, int y]
		{
			get => this[(x, y)];
			set => this[(x, y)] = value;
		}

		public (Tile Tile, Item Type) this[V2 v]
		{
			get => 0 <= v.X && v.X < m_Tiles.GetLength(0)
				&& 0 <= v.Y && v.Y < m_Tiles.GetLength(1)
				? (m_Tiles[v.X, v.Y], m_Items[v.X, v.Y])
				: (Tile.Empty, Item.None);
			set
			{
				m_Tiles[v.X, v.Y] = value.Tile;
				m_Items[v.X, v.Y] = value.Type;
			}
		}

		internal Grid Clone()
		{
			var g = new Grid(Width, Height);
			for (int x = 0; x < Width; ++x)
				for (int y = 0; y < Height; ++y)
					g[x, y] = this[x, y];
			return g;
		}
		internal void Absorb(Grid g)
		{
			for (int x = 0; x < Width; ++x)
				for (int y = 0; y < Height; ++y)
					this[x, y] = g[x, y];
		}

		internal void Print(bool tiles = true, bool items = true)
		{
			for (int y = 0; y < Height; ++y)
			{
				if (tiles)
				{
					for (int x = 0; x < Width; ++x)
					{
						switch (this[x, y].Tile)
						{
							case Tile.Empty: Console.Write("."); break;
							case Tile.Assembler: Console.Write("A"); break;
							case Tile.Inserter: Console.Write("I"); break;
							case Tile.Belt_Left: WriteHorizontalBelt(x, y, "┘", "┐", "←"); break;
							case Tile.Belt_Right: WriteHorizontalBelt(x, y, "└", "┌", "→"); break;
							case Tile.Belt_Up: WriteVerticalBelt(x, y, "┘", "└", "↑"); break;
							case Tile.Belt_Down: WriteVerticalBelt(x, y, "┐", "┌", "↓"); break;
						}
					}
				}

				if (items)
				{
					if (tiles) Console.Write("\t");
					for (int x = 0; x < Width; ++x)
					{
						switch (this[x, y].Type)
						{
							case Item.None: Console.Write("."); break;
							case Item.Copper: Console.Write("c"); break;
							case Item.GreenCircuit: Console.Write("G"); break;
							case Item.Cable: Console.Write("@"); break;
							case Item.Iron: Console.Write("i"); break;
						}
					}
				}
				Console.WriteLine("");
			}
		}

		private void WriteHorizontalBelt(int x, int y, string l, string r, string v)
		{
			Console.Write(
				this[x, y - 1].Tile == Tile.Belt_Down ? l
				: this[x, y + 1].Tile == Tile.Belt_Up ? r
				: v);
		}

		private void WriteVerticalBelt(int x, int y, string l, string r, string v)
		{
			Console.Write(
				this[x - 1, y].Tile == Tile.Belt_Right ? l
				: this[x + 1, y].Tile == Tile.Belt_Left ? r
				: v);
		}
	}
}
