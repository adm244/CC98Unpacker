using System;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class TxtVModelBlock : BaseModelBlock
  {
    private string Name;
    private UInt64 Unk01;
    private UInt64 Unk02;
    private Int32 Unk03;
    private Int32 Unk04;
    private Int32 Unk05;

    public string Text
    {
      get;
      private set;
    }

    public TxtVModelBlock()
      : base(BlockType.TxtV)
    {
      Name = string.Empty;
      Unk01 = default(UInt64);
      Unk02 = default(UInt64);
      Unk03 = default(Int32);
      Unk04 = default(Int32);
      Unk05 = default(Int32);
      Text = string.Empty;
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Name = inputReader.ReadUInt32AsString();
      Unk01 = inputReader.ReadUInt64();
      Unk02 = inputReader.ReadUInt64();
      Unk03 = inputReader.ReadInt32();
      Unk04 = inputReader.ReadInt32();
      Unk05 = inputReader.ReadInt32();
      Text = inputReader.ReadCString();

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      outputWriter.WriteStringAsUInt32(Name);
      outputWriter.Write((UInt64)Unk01);
      outputWriter.Write((UInt64)Unk02);
      outputWriter.Write((Int32)Unk03);
      outputWriter.Write((Int32)Unk04);
      outputWriter.Write((Int32)Unk05);
      outputWriter.WriteCString(Text, Encoding);

      return true;
    }
  }
}
