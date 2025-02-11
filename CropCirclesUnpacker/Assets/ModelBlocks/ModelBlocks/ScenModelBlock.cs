﻿using System;
using System.IO;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class ScenModelBlock : CplxModelBlock
  {
    private Int32 Unk01;
    private ExtraBlock[] ExtraBlocks;

    private ScenModelBlock()
    {
      throw new InvalidOperationException();
    }

    protected ScenModelBlock(BlockType type)
      : base(type)
    {
      Unk01 = default(Int32);
      ExtraBlocks = new ExtraBlock[0];
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Unk01 = inputReader.ReadInt32();
      ExtraBlocks = ExtraBlock.ParseExtraBlocks(inputReader);

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      outputWriter.Write((Int32)Unk01);

      if (!ExtraBlock.WriteExtraBlocks(outputWriter, ExtraBlocks))
        return false;

      return true;
    }
  }
}
