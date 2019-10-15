using System;
using System.IO;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class GButModelBlock : GChkModelBlock
  {
    private Int32 Unk01;

    public GButModelBlock()
      : this(BlockType.GBut)
    {
    }

    protected GButModelBlock(BlockType type)
      : base(type)
    {
      Unk01 = default(Int32);
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Unk01 = inputReader.ReadInt32();

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      outputWriter.Write((Int32)Unk01);

      return true;
    }
  }
}
