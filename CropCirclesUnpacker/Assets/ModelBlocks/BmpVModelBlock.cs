using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class BmpVModelBlock : BaseModelBlock
  {
    public string ImageName
    {
      get;
      private set;
    }

    public BmpVModelBlock()
      : base(BlockType.BmpV)
    {
      ImageName = string.Empty;
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      ImageName = inputReader.ReadUInt32AsString();

      return true;
    }
  }
}
