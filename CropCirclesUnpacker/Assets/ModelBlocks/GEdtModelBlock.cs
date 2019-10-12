using System;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class GEdtModelBlock : BaseModelBlock
  {
    private UInt64 Unk01;
    private UInt64 Unk02;
    private Int32 Unk03;
    private string Name;

    protected GEdtModelBlock()
    {
      throw new InvalidOperationException();
    }

    protected GEdtModelBlock(BlockType type)
      : base(type)
    {
      Unk01 = default(UInt64);
      Unk02 = default(UInt64);
      Unk03 = default(Int32);
      Name = string.Empty;
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Unk01 = inputReader.ReadUInt64();
      Unk02 = inputReader.ReadUInt64();
      Unk03 = inputReader.ReadInt32();
      Name = inputReader.ReadCString();

      return true;
    }
  }
}
