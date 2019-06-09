using Layout.Config;
using Layout.Placement;
using System;

namespace Layout
{
	class Program
	{
		static void Main(string[] args)
		{
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
			w.Generate(aG);

			Console.WriteLine($"total count: {World.Count}");
		}
	}
}
