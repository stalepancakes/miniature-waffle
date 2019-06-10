using System;
using Xunit;
using Layout.Placement;
using System.IO;
using Layout.Config;

namespace Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Test0()
		{
			TestBeltPrinting(
				"...",
				"...",
				"...",
				Tile.Empty, Tile.Empty, Tile.Empty,
				Tile.Empty, Tile.Empty, Tile.Empty,
				Tile.Empty, Tile.Empty, Tile.Empty
			);
		}
		[Fact]
		public void Test1()
		{

			TestBeltPrinting(
				".↑.",
				".└←",
				"...",
				Tile.Empty, Tile.Belt_Up, Tile.Empty,
				Tile.Empty, Tile.Belt_Up, Tile.Belt_Left,
				Tile.Empty, Tile.Empty, Tile.Empty
			);
			TestBeltPrinting(
				"...",
				".┌←",
				".↓.",
				Tile.Empty, Tile.Empty, Tile.Empty,
				Tile.Empty, Tile.Belt_Down, Tile.Belt_Left,
				Tile.Empty, Tile.Belt_Down, Tile.Empty
			);
			TestBeltPrinting(
				".↑.",
				"→┘.",
				"...",
				Tile.Empty, Tile.Belt_Up, Tile.Empty,
				Tile.Belt_Right, Tile.Belt_Up, Tile.Empty,
				Tile.Empty, Tile.Empty, Tile.Empty
			);
			TestBeltPrinting(
				"...",
				"→┐.",
				".↓.",
				Tile.Empty, Tile.Empty, Tile.Empty,
				Tile.Belt_Right, Tile.Belt_Down, Tile.Empty,
				Tile.Empty, Tile.Belt_Down, Tile.Empty
			);


			TestBeltPrinting(
				".↓.",
				".└→",
				"...",
				Tile.Empty, Tile.Belt_Down, Tile.Empty,
				Tile.Empty, Tile.Belt_Right, Tile.Belt_Right,
				Tile.Empty, Tile.Empty, Tile.Empty
			);
			TestBeltPrinting(
				".↓.",
				"←┘.",
				"...",
				Tile.Empty, Tile.Belt_Down, Tile.Empty,
				Tile.Belt_Left, Tile.Belt_Left, Tile.Empty,
				Tile.Empty, Tile.Empty, Tile.Empty
			);

			TestBeltPrinting(
				"...",
				".┌→",
				".↑.",
				Tile.Empty, Tile.Empty, Tile.Empty,
				Tile.Empty, Tile.Belt_Right, Tile.Belt_Right,
				Tile.Empty, Tile.Belt_Up, Tile.Empty
			);
			TestBeltPrinting(
				"...",
				"←┐.",
				".↑.",
				Tile.Empty, Tile.Empty, Tile.Empty,
				Tile.Belt_Left, Tile.Belt_Left, Tile.Empty,
				Tile.Empty, Tile.Belt_Up, Tile.Empty
			);
		}

		private void TestBeltPrinting(string v0, string v1, string v2, params Tile[] tiles)
		{
			var g = MakeGrid(tiles);

			var sw = new StringWriter();
			Console.SetOut(sw);
			g.Print(tiles: true, items: false);

			Assert.Equal(ToString(v0, v1, v2), sw.ToString());
		}

		private string ToString(string v1, string v2, string v3)
		{
			return $"{v1}\r\n{v2}\r\n{v3}\r\n";
		}

		private static Grid MakeGrid(params Tile[] tiles)
		{
			var g = new Grid(3, 3);
			for (int x = 0; x < g.Width; ++x)
				for (int y = 0; y < g.Width; ++y)
					g[x, y] = (tiles[3 * y + x], Item.Iron);
			return g;
		}
	}
}
