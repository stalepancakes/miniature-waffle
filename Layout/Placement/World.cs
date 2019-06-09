using Layout.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
		readonly Grid Grid = new Grid(10, 10);
		readonly List<V2> BusBeltEndpoints = new List<V2>();
		readonly List<V2> BeltEndpoints = new List<V2>();

		internal void AddInput(Item item)
		{
			AddOutput(item);
		}
		internal void AddOutput(Item item)
		{
			for (int y = 0; y < Grid.Height; ++y)
			{
				if (Grid[0, y].Tile == Tile.Empty)
				{
					Grid[0, y] = (Tile.Belt, item);
					BusBeltEndpoints.Add((0, y));
					return;
				}
			}
			throw new Exception();
		}

		internal void Generate(Assembler finalAssembler)
		{
			V2 p = (4, 4);
			var r = PlaceAssembler(p, finalAssembler);
			if (!r)
			{
				throw new Exception();
			}

			foreach (var op in InserterPositions(p))
			{
				if (PlaceInserter(op, finalAssembler.Output.Contents, out var undoInfo))
				{
					foreach (var _ in PlaceInputInserters(p, finalAssembler.Inputs, 0))
					{
						if (RouteBelts())
						{
							// Print();
						}
					}

					Undo(undoInfo);
				}
			}
		}

		private bool RouteBelts()
		{
			var xGrid = Grid.Clone();

			bool success = true;
			foreach (var x in BusBeltEndpoints)
			{
				var item = Grid[x].Type;
				foreach (var e in BeltEndpoints)
				{
					if (Grid[e].Type == item)
					{
						if (!RouteBelt(x, e, item))
						{
							success = false;
							goto Out;
						}
					}
				}
			}

			if (success)
				Print();

		Out:
			Grid.Absorb(xGrid);
			return success;
		}

		private static V2[] BeltOffsets = new V2[] { (0, 1), (0, -1), (1, 0), (-1, 0) };
		private bool RouteBelt(V2 q, V2 b, Item i)
		{
			var visited = new HashSet<V2>();
			var open = new HashSet<V2>() { q };
			var parent = new Dictionary<V2, V2>();

			while (open.Any())
			{
				var x = open.First();
				open.Remove(x);
				visited.Add(x);

				if (Grid[x].Tile == Tile.Belt && Grid[x].Type != i)
					continue;
				else if (Grid[x].Tile == Tile.Belt && Grid[x].Type == i)
					;
				else if (Grid[x].Tile != Tile.Empty)
					continue;

				foreach (var o in BeltOffsets)
				{
					var ao = x + o;
					if (ao.X < 0 || ao.Y < 0 || ao.X >= 10 || ao.Y >= 10)
						continue;

					if (ao == b)
					{
						do
						{
							Grid[x] = (Tile.Belt, i);
							x = parent[x];
						}
						while (parent.ContainsKey(x));

						return true;
					}

					if (!visited.Contains(ao))
					{
						parent[ao] = x;
						open.Add(ao);
					}
				}
			}
			return false;
		}

		private IEnumerable<object> PlaceInputInserters(V2 assemblerPos, Belt[] inputs, int idx)
		{
			if (idx < inputs.Length)
			{
				foreach (var p in InserterPositions(assemblerPos))
				{
					if (PlaceInserter(p, inputs[idx].Contents, out var undoInfo))
					{
						foreach (var _ in PlaceInputInserters(assemblerPos, inputs, idx + 1))
							yield return _;
						Undo(undoInfo);
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
					switch (Grid[x, y].Tile)
					{
						case Tile.Empty: Console.Write("."); break;
						case Tile.Assembler: Console.Write("A"); break;
						case Tile.Inserter: Console.Write("I"); break;
						case Tile.Belt: Console.Write("B"); break;
					}
				}
				Console.Write("\t");
				for (int x = 0; x < 10; ++x)
				{
					switch (Grid[x, y].Type)
					{
						case Item.None: Console.Write("."); break;
						case Item.Copper: Console.Write("c"); break;
						case Item.GreenCircuit: Console.Write("G"); break;
						case Item.Cable: Console.Write("@"); break;
						case Item.Iron: Console.Write("i"); break;
					}
				}
				Console.WriteLine("");
			}
			Console.WriteLine(string.Join(", ", BeltEndpoints));
		}

		private void Undo(InserterUndo x)
		{
			Grid[x.Pos] = (Tile.Empty, Item.None);
			Grid[x.BeltPos] = (Tile.Empty, Item.None);
			Debug.Assert(x.BeltIndex == BeltEndpoints.Count - 1);
			BeltEndpoints.RemoveAt(x.BeltIndex);
		}

		private IEnumerable<(V2 pos, V2 armOffset)> InserterPositions(V2 p)
		{
			for (int y = 0; y < 3; y++)
			{
				yield return ((p.X - 1, p.Y + y), (-1, 0));
			}

			for (int y = 0; y < 3; y++)
			{
				yield return ((p.X + 3, p.Y + y), (1, 0));
			}

			for (int x = 0; x < 3; x++)
			{
				yield return ((p.X + x, p.Y - 1), (0, -1));
			}

			for (int x = 0; x < 3; x++)
			{
				yield return ((p.X + x, p.Y + 3), (0, 1));
			}
		}

		private struct InserterUndo
		{
			public V2 Pos, BeltPos;
			public int BeltIndex;
		}

		private bool PlaceInserter((V2 pos, V2 armOffset) inserterPos, Item item, out InserterUndo inserterUndo)
		{
			var p = inserterPos.pos;
			var bp = p + inserterPos.armOffset;

			if (Grid[p].Tile != Tile.Empty || Grid[p].Tile != Tile.Empty)
			{
				inserterUndo = new InserterUndo();
				return false;
			}

			Grid[p] = (Tile.Inserter, item);
			Grid[bp] = (Tile.Belt, item);

			BeltEndpoints.Add(bp);
			inserterUndo = new InserterUndo()
			{
				Pos = p,
				BeltPos = bp,
				BeltIndex = BeltEndpoints.Count - 1
			};

			return true;
		}

		private bool PlaceAssembler(V2 p, Assembler _)
		{
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					if (Grid[p + (x, y)].Tile != Tile.Empty)
					{
						return false;
					}
				}
			}

			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					Grid[p + (x, y)] = (Tile.Assembler, Item.None);
				}
			}

			return true;
		}
	}
}
