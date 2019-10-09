using System;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.Base
{
  public class ModelBlock
  {
    protected BlockContentType ContentType;
    protected uint Flags;

    public BlockType Type
    {
      get;
      private set;
    }

    protected ModelBlock(BlockType type)
    {
      ContentType = BlockContentType.Unknown;
      Flags = 0x80000001;
      Type = type;
    }

    public virtual bool Parse(BinaryReader inputReader)
    {
      BlockContentType type = BlockContentType.Unknown;

      string typeName = inputReader.ReadUInt32AsString();
      if (!string.IsNullOrEmpty(typeName))
      {
        if (!Enum.IsDefined(typeof(BlockContentType), typeName))
        {
          Debug.Assert(false, "Undefined base block type encountered!");
          return false;
        }
        type = (BlockContentType)Enum.Parse(typeof(BlockContentType), typeName, true);
      }

      ContentType = type;
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

        default:
          Debug.Assert(false, "Attempting to create unimplemented model block!");
          return null;
      }
    }

    public enum BlockContentType
    {
      Unknown = 0,
      View,
      Brn_,
    }

    public enum BlockType
    {
      BmpV,
      Cplx,
      CMod,
      Brn_,
    }
  }
}
