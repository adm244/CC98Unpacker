using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ExtraBlocks
{
  public class TVExtraBlock : ExtraBlock
  {
    private double Unk01;
    private double Unk02;
    private string Name;

    public TVExtraBlock()
      : base(ExtraBlockType.TV__)
    {
      Unk01 = default(double);
      Unk02 = default(double);
      Name = string.Empty;
    }

    public override bool Parse(BinaryReader inputReader)
    {
      Unk01 = inputReader.ReadDouble();
      Unk02 = inputReader.ReadDouble();
      Name = inputReader.ReadUInt32AsString();

      return true;
    }
  }
}
