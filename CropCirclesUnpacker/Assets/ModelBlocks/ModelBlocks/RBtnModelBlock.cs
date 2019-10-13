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
  }
}
