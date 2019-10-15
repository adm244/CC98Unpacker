using System;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class GEdtModelBlock : BaseModelBlock
  {
    private UInt64 Unk01;
    protected double Unk02;
    private Int32 Unk03;
    private string Name;

    public GEdtModelBlock()
      : this(BlockType.GEdt)
    {
    }

    protected GEdtModelBlock(BlockType type)
      : base(type)
    {
      Unk01 = default(UInt64);
      Unk02 = default(double);
      Unk03 = default(Int32);
      Name = string.Empty;
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Unk01 = inputReader.ReadUInt64();
      Unk02 = inputReader.ReadDouble();
      Unk03 = inputReader.ReadInt32();
      Name = inputReader.ReadCString();

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      outputWriter.Write((UInt64)Unk01);
      outputWriter.Write((double)Unk02);
      outputWriter.Write((Int32)Unk03);
      outputWriter.WriteCString(Name, Encoding);

      return true;
    }
  }
}
