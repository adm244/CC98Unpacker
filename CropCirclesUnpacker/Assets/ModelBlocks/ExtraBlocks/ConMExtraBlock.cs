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
  }
}
