using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets.ModelBlocks.ExtraBlocks;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public abstract class ExtraBlock
  {
    public ExtraBlockType Type
    {
      get;
      private set;
    }

    protected ExtraBlock(ExtraBlockType type)
    {
      Type = type;
    }

    public abstract bool Parse(BinaryReader inputReader);

    public static ExtraBlock[] ParseExtraBlocks(BinaryReader inputReader)
    {
      List<ExtraBlock> blocks = new List<ExtraBlock>();

      while (true)
      {
        Int32 typeId = inputReader.ReadInt32();
        if (typeId == 0)
          break;

        inputReader.BaseStream.Seek(-sizeof(Int32), SeekOrigin.Current);

        string typeName = inputReader.ReadUInt32AsString();
        if (!Enum.IsDefined(typeof(ExtraBlockType), typeName))
        {
          Trace.Assert(false, "Undefined extra block type encountered!");
          return null;
        }

        ExtraBlockType type = (ExtraBlockType)Enum.Parse(typeof(ExtraBlockType), typeName);
        switch (type)
        {
          case ExtraBlockType.ConM:
          case ExtraBlockType.TV__:
            {
              ExtraBlock block = ParseExtraBlock(inputReader, type);
              if (block == null)
                return new ExtraBlock[0];

              if (type == ExtraBlockType.ConM)
                blocks.AddRange(((ConMExtraBlock)block).Blocks);
              else
                blocks.Add(block);
            }
            break;

          default:
            Trace.Assert(false, "Unimplemented extra block type!");
            break;
        }
      }

      return blocks.ToArray();
    }

    private static ExtraBlock ParseExtraBlock(BinaryReader inputReader, ExtraBlockType type)
    {
      ExtraBlock block = CreateExtraBlock(type);
      if (block == null)
      {
        Debug.Assert(false, "Extra block cannot be created!");
        return null;
      }

      if (!block.Parse(inputReader))
        return null;

      return block;
    }

    private static ExtraBlock CreateExtraBlock(ExtraBlockType type)
    {
      switch (type)
      {
        case ExtraBlockType.ConM:
          return new ConMExtraBlock();
        case ExtraBlockType.TV__:
          return new TVExtraBlock();

        default:
          Debug.Assert(false, "Attempting to create an unimplemented extra block!");
          return null;
      }
    }

    public enum ExtraBlockType
    {
      ConM,
      TV__,
    }
  }
}
