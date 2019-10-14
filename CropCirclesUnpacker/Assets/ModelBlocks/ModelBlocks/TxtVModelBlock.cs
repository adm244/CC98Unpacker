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
  }
}
