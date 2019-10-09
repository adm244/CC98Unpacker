using System.IO;
using CropCirclesUnpacker.Assets.ModelBlocks.Base;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class CplxModelBlock : ModelSubBlock
  {
    public CplxModelBlock()
      : base(BlockType.Cplx)
    {
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      return true;
    }
  }
}
