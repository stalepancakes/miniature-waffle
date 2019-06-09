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
			get => (m_Tiles[v.X, v.Y], m_Items[v.X, v.Y]);
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
	}
}
