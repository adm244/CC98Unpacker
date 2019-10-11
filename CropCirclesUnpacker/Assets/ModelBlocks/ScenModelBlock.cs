using System;
using System.Diagnostics;
using System.IO;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class ScenModelBlock : CplxModelBlock
  {
    private Int32 Unk01;

    private ScenModelBlock()
    {
      throw new InvalidOperationException();
    }

    protected ScenModelBlock(BlockType type)
      : base(type)
    {
      Unk01 = default(Int32);
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;
      
      Unk01 = inputReader.ReadInt32();
      if (Unk01 != 0)
      {
        Trace.Assert(false, "Value of Unk01 in Scen block is not zero!\n"
          + "Filename: " + ((FileStream)inputReader.BaseStream).Name);
        return false;
      }

      //TODO(adm244): implement parsing properly if there's a file that use Unk01 != 0

      return true;
    }
  }
}
