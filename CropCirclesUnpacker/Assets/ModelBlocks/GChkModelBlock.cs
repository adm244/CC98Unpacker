using System;

namespace CropCirclesUnpacker.Assets.ModelBlocks
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
      Flags |= 0x8000000C;
    }
  }
}
