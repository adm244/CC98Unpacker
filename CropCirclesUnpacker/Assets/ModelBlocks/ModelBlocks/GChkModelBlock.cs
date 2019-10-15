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
  }
}
