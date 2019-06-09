namespace Layout.Config
{
	internal class Assembler
	{
		public readonly Belt[] Inputs;
		public readonly Belt Output;

		public Assembler(Belt output, params Belt[] inputs)
		{
			this.Output = output;
			this.Inputs = inputs;
		}
	}
}