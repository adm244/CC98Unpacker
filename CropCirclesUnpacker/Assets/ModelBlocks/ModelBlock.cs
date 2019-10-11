using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class ModelBlock
  {
    protected List<ModelBlock> SubBlocks;
    protected string ContentName;
    protected uint Flags;

    public BlockType Type
    {
      get;
      private set;
    }

    protected ModelBlock(BlockType type)
    {
      SubBlocks = new List<ModelBlock>();
      ContentName = string.Empty;
      Flags = 0x80000001;
      Type = type;
    }

    public virtual bool Parse(BinaryReader inputReader)
    {
      ContentName = inputReader.ReadUInt32AsString();
      Flags = inputReader.ReadUInt32();

      return true;
    }

    public static ModelBlock ParseBlock(BinaryReader inputReader)
    {
      string typeName = inputReader.ReadUInt32AsString();
      if (!Enum.IsDefined(typeof(BlockType), typeName))
      {
        Debug.Assert(false, "Undefined block encountered!");
        return null;
      }

      BlockType type = (BlockType)Enum.Parse(typeof(BlockType), typeName, true);
      return ParseBlock(inputReader, type);
    }

    public static ModelBlock ParseBlock(BinaryReader inputReader, BlockType type)
    {
      ModelBlock block = ModelBlock.Create(type);
      if (block == null)
      {
        Debug.Assert(false, "ModelBlock.Create returned null!");
        return null;
      }

      if (!block.Parse(inputReader))
        return null;

      return block;
    }

    public bool ParseSubBlock(BinaryReader inputReader)
    {
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
            continueParsing = ParseSubBlock(inputReader, subBlockType);
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

    private bool ParseSubBlock(BinaryReader inputReader, SubBlockType type)
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

    private static ModelBlock Create(BlockType type)
    {
      switch (type)
      {
        case BlockType.BmpV:
          return new BmpVModelBlock();
        case BlockType.Cplx:
          return new CplxModelBlock();
        case BlockType.CMod:
          return new CModModelBlock();
        case BlockType.Brn_:
          return new BrnModelBlock();
        case BlockType.SeqV:
          return new SeqVModelBlock();
        case BlockType.TxtV:
          return new TxtVModelBlock();

        default:
          Debug.Assert(false, "Attempting to create unimplemented model block!");
          return null;
      }
    }

    private enum SubBlockType
    {
      CInt,
      CMod,
      CEnd,
    }

    public enum BlockType
    {
      BmpV,
      Cplx,
      CMod,
      Brn_,
      SeqV,
      TxtV,
    }
  }
}
