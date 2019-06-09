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

			V2 p = (4, 4);
			var r = PlaceAssembler(p, finalAssembler);
			if (!r)
			{
				throw new Exception();
			}

			foreach (var op in InserterPositions(p))
			{
				if (PlaceInserter(op, finalAssembler.Output.Contents))
				{
					foreach (var _ in PlaceInputInserters(p, finalAssembler.Inputs, 0))
					{
						Print();
					}

					Undo(op);
				}
			}
		}

		private IEnumerable<object> PlaceInputInserters(V2 assemblerPos, Belt[] inputs, int idx)
		{
			if (idx < inputs.Length)
			{
				foreach (var p in InserterPositions(assemblerPos))
				{
					if (PlaceInserter(p, inputs[idx].Contents))
					{
						foreach (var _ in PlaceInputInserters(assemblerPos, inputs, idx + 1))
							yield return _;
						Undo(p);
					}
				}
			}
			else
			{
				yield return null;
			}
		}

		public static int Count = 0;
		private void Print()
		{
			++Count;
			Console.SetCursorPosition(0, 0);
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

		private void Undo(V2 p)
		{
			Grid[p.X, p.Y] = Tile.Empty;
			GridType[p.X, p.Y] = Item.None;
		}

		private IEnumerable<V2> InserterPositions(V2 p)
		{
			for (int y = 0; y < 3; y++)
			{
				yield return (p.X - 1, p.Y + y);
			}

			for (int y = 0; y < 3; y++)
			{
				yield return (p.X + 3, p.Y + y);
			}

			for (int x = 0; x < 3; x++)
			{
				yield return (p.X + x, p.Y - 1);
			}

			for (int x = 0; x < 3; x++)
			{
				yield return (p.X + x, p.Y + 3);
			}
		}

		private bool PlaceInserter(V2 p, Item item)
		{
			if (Grid[p.X, p.Y] != Tile.Empty)
				return false;

			Grid[p.X, p.Y] = Tile.Inserter;
			GridType[p.X, p.Y] = item;

			return true;
		}

		private bool PlaceAssembler(V2 p, Assembler _)
		{
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					if (Grid[p.X + x, p.Y + y] != Tile.Empty)
					{
						return false;
					}
				}
			}

			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					Grid[p.X + x, p.Y + y] = Tile.Assembler;
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
