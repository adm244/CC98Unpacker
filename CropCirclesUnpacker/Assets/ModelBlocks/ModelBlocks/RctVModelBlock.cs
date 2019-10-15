using System;
using System.IO;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class RctVModelBlock : BaseModelBlock
  {
    public double Unk01;
    public double Unk02;
    public Int32 Unk03;
    public Int32 Unk04;
    public Int32 Unk05;

    public RctVModelBlock()
      : base(BlockType.RctV)
    {
      Unk01 = default(double);
      Unk02 = default(double);
      Unk03 = default(Int32);
      Unk04 = default(Int32);
      Unk05 = default(Int32);
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Unk01 = inputReader.ReadDouble();
      Unk02 = inputReader.ReadDouble();
      Unk03 = inputReader.ReadInt32();
      Unk04 = inputReader.ReadInt32();
      Unk05 = inputReader.ReadInt32();

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      outputWriter.Write((double)Unk01);
      outputWriter.Write((double)Unk02);
      outputWriter.Write((Int32)Unk03);
      outputWriter.Write((Int32)Unk04);
      outputWriter.Write((Int32)Unk05);

      return true;
    }
  }
}
