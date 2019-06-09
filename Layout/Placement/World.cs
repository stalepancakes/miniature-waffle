using Layout.Config;
using System;
using System.Collections.Generic;

namespace Layout.Placement
{
	public enum Tile
	{
		Empty,
		Assembler,
		Belt,
		Inserter
	}

	class World
	{
		readonly Tile[,] Grid = new Tile[10, 10];
		readonly Item[,] GridType = new Item[10, 10];

		internal void Generate(Assembler finalAssembler)
		{
			var visited = new HashSet<Assembler>();
			var open = new HashSet<Assembler> { finalAssembler };

			var p = (4, 4);
			var r = Place(p, finalAssembler);
			if (!r)
			{
				throw new Exception();
			}

			// output
			foreach (var ip in InserterPositions(p))
			{
				if (PlaceInserter(ip, finalAssembler.Output.Contents))
				{
					Print();
					Undo(ip);
				}
			}
			//PlaceInserter(p, finalAssembler.Output);
			//foreach (var input in finalAssembler.Inputs)
			//{
			//	PlaceInserter(p, finalAssembler.Output)
			//}
		}

		private void Print()
		{
			Console.WriteLine("==");
			for (int y = 0; y < 10; ++y)
			{
				for (int x = 0; x < 10; ++x)
				{
					switch (Grid[x, y])
					{
						case Tile.Empty: Console.Write("."); break;
						case Tile.Assembler: Console.Write("A"); break;
						case Tile.Inserter: Console.Write("I"); break;
						case Tile.Belt: Console.Write("B"); break;
					}
				}
				Console.WriteLine("");
			}
		}

		private void Undo((int x, int y) p)
		{
			Grid[p.x, p.y] = Tile.Empty;
			GridType[p.x, p.y] = Item.None;
		}

		private IEnumerable<(int, int)> InserterPositions((int x, int y) p)
		{
			for (int y = 0; y < 3; y++)
			{
				yield return (p.x - 1, p.y + y);
			}

			for (int y = 0; y < 3; y++)
			{
				yield return (p.x + 3, p.y + y);
			}

			for (int x = 0; x < 3; x++)
			{
				yield return (p.x + x, p.y - 1);
			}

			for (int x = 0; x < 3; x++)
			{
				yield return (p.x + x, p.y + 3);
			}
		}

		private bool PlaceInserter((int x, int y) p, Item item)
		{
			if (Grid[p.x, p.y] != Tile.Empty)
				return false;

			Grid[p.x, p.y] = Tile.Inserter;
			GridType[p.x, p.y] = item;

			return true;
		}

		private bool Place((int x, int y) p, Assembler a)
		{
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					if (Grid[p.x + x, p.y + y] != Tile.Empty)
					{
						return false;
					}
				}
			}

			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					Grid[p.x + x, p.y + y] = Tile.Assembler;
				}
			}

			return true;
		}

		//private void Place(HashSet<Assembler> visited, HashSet<Assembler> open, Assembler assembler)
		//{
		//	visited.Add(assembler);
		//	foreach (var position in Placements(assembler))
		//	{
		//		PlaceAssembler(position);


		//	}
		//}

		//private IEnumerable<object> Placements(Assembler assembler)
		//{
		//	throw new NotImplementedException();
		//}
	}
}
