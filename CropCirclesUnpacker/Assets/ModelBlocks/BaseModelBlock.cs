using System;
using System.IO;
using CropCirclesUnpacker.Assets.ModelBlocks;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class BaseModelBlock : ModelBlock
  {
    protected Int64 Unk01;
    protected Int64 Unk02;
    protected Int64 Unk03;
    protected Int64 Unk04;

    protected BaseModelBlock()
      : base()
    {
      throw new InvalidOperationException();
    }

    protected BaseModelBlock(BlockType type)
      : base(type)
    {
      Unk01 = 0;
      Unk02 = 0;
      Unk03 = 0;
      Unk04 = 0;
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Unk01 = inputReader.ReadInt64();
      Unk02 = inputReader.ReadInt64();
      Unk03 = inputReader.ReadInt64();
      Unk04 = inputReader.ReadInt64();

      return true;
    }
  }
}
