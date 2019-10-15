using System;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class SeqVModelBlock : BaseModelBlock
  {
    private DataBlock[] Blocks;
    private string Name;
    private Int32 Unknown;

    public SeqVModelBlock()
      : base(BlockType.SeqV)
    {
      Blocks = new DataBlock[0];
      Name = string.Empty;
      Unknown = default(Int32);
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Blocks = DataBlock.ParseBlocks(inputReader);
      if (Blocks.Length <= 0)
        return false;

      if (!ParseSubBlocks(inputReader))
        return false;

      Name = inputReader.ReadUInt32AsString();
      Unknown = inputReader.ReadInt32();

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      if (!DataBlock.WriteBlocks(outputWriter, Blocks))
        return false;

      if (!WriteSubBlocks(outputWriter))
        return false;

      outputWriter.WriteStringAsUInt32(Name);
      outputWriter.Write((Int32)Unknown);

      return true;
    }
  }
}
