using System.IO;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class CplxModelBlock : ModelBlock
  {
    public CplxModelBlock()
      : base(BlockType.Cplx)
    {
    }

    protected CplxModelBlock(BlockType type)
      : base(type)
    {
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      if (!ParseSubBlocks(inputReader))
        return false;

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      if (!WriteSubBlocks(outputWriter))
        return false;

      return true;
    }
  }
}
