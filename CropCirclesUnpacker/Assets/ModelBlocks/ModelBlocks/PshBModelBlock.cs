using System.IO;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class PshBModelBlock : GButModelBlock
  {
    public PshBModelBlock()
      : base(BlockType.PshB)
    {
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Flags |= 0x80000002;

      return true;
    }
  }
}
