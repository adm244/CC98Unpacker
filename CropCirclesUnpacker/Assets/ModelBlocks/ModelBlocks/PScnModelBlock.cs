using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class PScnModelBlock : ScenModelBlock
  {
    private string Name;

    public PScnModelBlock()
      : this(BlockType.PScn)
    {
    }

    protected PScnModelBlock(BlockType type)
      : base(type)
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
