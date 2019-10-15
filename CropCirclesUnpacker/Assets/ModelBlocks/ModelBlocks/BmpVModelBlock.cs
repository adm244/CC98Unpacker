using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
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

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      outputWriter.WriteStringAsUInt32(ImageName);

      return true;
    }
  }
}
