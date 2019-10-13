using System;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class GChkModelBlock : GEdtModelBlock
  {
    private GChkModelBlock()
    {
      throw new InvalidOperationException();
    }

    protected GChkModelBlock(BlockType type)
      : base(type)
    {
    }

    public override bool Parse(System.IO.BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Flags |= 0x8000000C;

      return true;
    }
  }
}
