using System.IO;
using CropCirclesUnpacker.Assets.ModelBlocks.Base;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class CModModelBlock : ModelBlock
  {
    private string Name1;
    private string Name2;

    public CModModelBlock()
      : base(BlockType.CMod)
    {
      Name1 = string.Empty;
      Name2 = string.Empty;
    }

    public override bool Parse(BinaryReader inputReader)
    {
      Name1 = inputReader.ReadUInt32AsString();
      Name2 = inputReader.ReadUInt32AsString();

      return true;
    }
  }
}
