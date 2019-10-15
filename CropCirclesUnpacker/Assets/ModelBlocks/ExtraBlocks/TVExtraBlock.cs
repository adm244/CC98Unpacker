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

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      outputWriter.Write((double)Unk01);
      outputWriter.Write((double)Unk02);
      outputWriter.WriteStringAsUInt32(Name);

      return true;
    }
  }
}
