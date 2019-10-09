using System.IO;
using CropCirclesUnpacker.Assets.ModelBlocks.Base;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class BrnModelBlock : ModelBlock
  {
    private string Name;

    public BrnModelBlock()
      : base(BlockType.Brn_)
    {
      Name = string.Empty;
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Name = inputReader.ReadUInt32AsString();

      return true;
    }
  }
}
