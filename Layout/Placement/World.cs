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
		Belt_Left,
		Belt_Right,
		Belt_Up,
		Belt_Down,
		Inserter
	}

	class World
	{
		readonly Grid Grid = new Grid(10, 10);
		readonly List<V2> BusBeltEndpoints = new List<V2>();
		readonly List<V2> BeltEndpoints = new List<V2>();

		internal void AddInput(Item item)
		{
			for (int y = 0; y < Grid.Height; ++y)
			{
				if (Grid[0, y].Tile == Tile.Empty)
				{
					Grid[0, y] = (Tile.Belt_Right, item);
					BusBeltEndpoints.Add((0, y));
					return;
				}
			}
			throw new Exception();
		}
		internal void AddOutput(Item item)
		{
			for (int y = 0; y < Grid.Height; ++y)
			{
				if (Grid[0, y].Tile == Tile.Empty)
				{
					Grid[0, y] = (Tile.Belt_Left, item);
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

				if (IsBelt(Grid[x].Tile) && Grid[x].Type != i)
					continue;
				else if (IsBelt(Grid[x].Tile) && Grid[x].Type == i)
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
						var o2 = o;
						do
						{
							Grid[x] = (Belt(o2), i);
							o2 = x - parent[x];
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

		private bool IsBelt(Tile tile)
		{
			return tile == Tile.Belt_Down
				|| tile == Tile.Belt_Up
				|| tile == Tile.Belt_Right
				|| tile == Tile.Belt_Left;
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
			Grid.Print();
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
			Grid[bp] = (Belt(inserterPos.armOffset), item);

			BeltEndpoints.Add(bp);
			inserterUndo = new InserterUndo()
			{
				Pos = p,
				BeltPos = bp,
				BeltIndex = BeltEndpoints.Count - 1
			};

			return true;
		}

		private Tile Belt(V2 offset)
		{
			if (offset.X == -1 && offset.Y ==  0) return Tile.Belt_Left;
			if (offset.X ==  1 && offset.Y ==  0) return Tile.Belt_Right;
			if (offset.X ==  0 && offset.Y == -1) return Tile.Belt_Up;
			if (offset.X ==  0 && offset.Y ==  1) return Tile.Belt_Down;
			throw new Exception();
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
