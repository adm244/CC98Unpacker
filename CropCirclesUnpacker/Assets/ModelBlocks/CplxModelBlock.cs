using System.IO;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class CplxModelBlock : ModelBlock
  {
    public CplxModelBlock()
      : base(BlockType.Cplx)
    {
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      if (!ParseSubBlock(inputReader))
        return false;

      return true;
    }
  }
}
