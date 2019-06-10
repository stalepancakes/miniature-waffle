using Layout.Config;
using Layout.Placement;
using System;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Layout
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			// green circuit
			// 1.5 copper  -> 3x cable --> circuit 1x
			//                1x iron  -'
			// 3 copper -> 6 cable --> 2 circuit
			//             2 iron  -'
			// copper -> cable ---> circuit -->>
			//        -> cable '       ^- iron

			var bC = new Belt(Item.Copper);
			var bCC = new Belt(Item.Cable);
			var bI = new Belt(Item.Iron);
			var bG = new Belt(Item.GreenCircuit);

			var aC1 = new Assembler(bCC, bC);
			var aC2 = new Assembler(bCC, bC);
			var aG = new Assembler(bG, bCC, bI);

			var w = new World();
			w.AddInput(Item.Copper);
			w.AddInput(Item.Iron);
			w.AddInput(Item.Cable);
			w.AddOutput(Item.GreenCircuit);
			w.Generate(aG);

			Console.WriteLine($"total count: {World.Count}");
		}
	}
}
