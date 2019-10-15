using System.IO;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ExtraBlocks
{
  public class ConMExtraBlock : ExtraBlock
  {
    public ExtraBlock[] Blocks
    {
      get;
      private set;
    }

    public ConMExtraBlock()
      : base(ExtraBlockType.ConM)
    {
      Blocks = new ExtraBlock[0];
    }

    public override bool Parse(BinaryReader inputReader)
    {
      Blocks = ExtraBlock.ParseExtraBlocks(inputReader);

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      if (!ExtraBlock.WriteExtraBlocks(outputWriter, Blocks))
        return false;

      return true;
    }
  }
}
