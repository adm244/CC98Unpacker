using System.IO;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class GLstModelBlock : GEdtModelBlock
  {
    public GLstModelBlock()
      : base(BlockType.GLst)
    {
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Unk02 *= 0.06666666666666667d;
      Flags |= 0x8000000C;

      return true;
    }
  }
}
