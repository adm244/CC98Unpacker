using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.Base
{
  public class ModelSubBlock : ModelBlock
  {
    protected List<ModelBlock> SubBlocks;

    protected ModelSubBlock(BlockType type)
      : base(type)
    {
      SubBlocks = new List<ModelBlock>();
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      bool continueParsing = false;
      do
      {
        string subBlockTypeName = inputReader.ReadUInt32AsString();
        if (!Enum.IsDefined(typeof(SubBlockType), subBlockTypeName))
        {
          Debug.Assert(false, "Undefined sub block type encountered!");
          return false;
        }

        SubBlockType subBlockType = (SubBlockType)Enum.Parse(typeof(SubBlockType), subBlockTypeName);
        switch (subBlockType)
        {
          case SubBlockType.CInt:
          case SubBlockType.CMod:
            continueParsing = ParseBlock(inputReader, subBlockType);
            break;

          case SubBlockType.CEnd:
            continueParsing = false;
            break;

          default:
            Debug.Assert(false, "Unimplemented sub block type!");
            break;
        }
      } while (continueParsing);

      return (SubBlocks.Count > 0);
    }

    private bool ParseBlock(BinaryReader inputReader, SubBlockType type)
    {
      ModelBlock block = null;
      if (type == SubBlockType.CMod)
        block = ModelBlock.ParseBlock(inputReader, BlockType.CMod);
      else
        block = ModelBlock.ParseBlock(inputReader);

      if (block == null)
        return false;

      SubBlocks.Add(block);
      return true;
    }

    private enum SubBlockType
    {
      CInt,
      CMod,
      CEnd,
    }
  }
}
