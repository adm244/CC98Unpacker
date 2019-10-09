using System;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks
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
      string typeName = inputReader.ReadUInt32AsString();
      if (!Enum.IsDefined(typeof(BlockContentType), typeName))
      {
        Debug.Assert(false, "Undefined base block type encountered!");
        return false;
      }

      ContentType = (BlockContentType)Enum.Parse(typeof(BlockContentType), typeName, true);
      Flags = inputReader.ReadUInt32();

      return true;
    }

    public static ModelBlock Create(BlockType type)
    {
      switch (type)
      {
        case BlockType.BmpV:
          return new BmpVModelBlock();

        default:
          Debug.Assert(false, "Attempting to create unimplemented model block!");
          return null;
      }
    }

    public enum BlockContentType
    {
      Unknown = 0,
      View,
    }

    public enum BlockType
    {
      BmpV,
    }
  }
}
