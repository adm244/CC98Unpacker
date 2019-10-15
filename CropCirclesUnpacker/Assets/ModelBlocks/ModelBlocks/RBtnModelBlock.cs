using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class RBtnModelBlock : GChkModelBlock
  {
    private string ImageNormal;
    private string ImageOver;
    private string ImageClicked;

    public RBtnModelBlock()
      : base(BlockType.RBtn)
    {
      ImageNormal = string.Empty;
      ImageOver = string.Empty;
      ImageClicked = string.Empty;
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      ImageNormal = inputReader.ReadUInt32AsString();
      ImageOver = inputReader.ReadUInt32AsString();
      ImageClicked = inputReader.ReadUInt32AsString();

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      outputWriter.WriteStringAsUInt32(ImageNormal);
      outputWriter.WriteStringAsUInt32(ImageOver);
      outputWriter.WriteStringAsUInt32(ImageClicked);

      return true;
    }
  }
}
