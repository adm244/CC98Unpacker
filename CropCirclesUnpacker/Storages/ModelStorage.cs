using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Assets.ModelBlocks;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Storages
{
  public class ModelStorage : BaseStorage
  {
    private List<ModelBlock> Blocks;

    private ModelStorage()
      : this(string.Empty)
    {
    }

    private ModelStorage(string fileName)
      : base(fileName)
    {
      Blocks = new List<ModelBlock>();
    }

    public static Model LoadFromFile(string fileName)
    {
      ModelStorage storage = new ModelStorage(fileName);

      using (FileStream stream = new FileStream(storage.LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(stream, storage.Encoding))
        {
          if (!storage.Parse(inputReader))
          {
            Debug.Assert(false, "Cannot parse a model file!");
            return null;
          }
        }
      }

      string name = Path.GetFileNameWithoutExtension(storage.LibraryPath);
      return new Model(name, storage.Blocks.ToArray());
    }

    public static Model LoadFromStream(BinaryReader inputReader, string name)
    {
      ModelStorage storage = new ModelStorage();
      if (!storage.Parse(inputReader))
      {
        Debug.Assert(false, "Cannot parse a model file!");
        return null;
      }

      return new Model(name, storage.Blocks.ToArray());
    }

    protected override bool Parse(BinaryReader inputReader)
    {
      if (!ReadHeader(inputReader))
        return false;

      //NOTE(adm244): check for duplicated header
      Int32 magic = inputReader.ReadInt32();
      if (magic == Signature)
      {
        inputReader.BaseStream.Seek(-sizeof(Int32), SeekOrigin.Current);
        if (!ReadHeader(inputReader))
          return false;
      }

      while (!inputReader.EOF())
      {
        string blockTypeName = inputReader.ReadUInt32AsString();
        if (Enum.IsDefined(typeof(ModelBlock.BlockType), blockTypeName))
        {
          ModelBlock.BlockType blockType = 
            (ModelBlock.BlockType)Enum.Parse(typeof(ModelBlock.BlockType), blockTypeName, true);
          if (!ParseBlock(inputReader, blockType))
            Debug.Assert(false, "Could not parse a block!");
        }
        else
        {
          Debug.Assert(false, "Undefined block encountered!");
          break;
        }
      }

      return (Blocks.Count > 0);
    }

    private bool ParseBlock(BinaryReader inputReader, ModelBlock.BlockType type)
    {
      ModelBlock block = ModelBlock.Create(type);
      if (block == null)
      {
        Debug.Assert(false, "ModelBlock.Create returned null!");
        return false;
      }

      if (!block.Parse(inputReader))
        return false;

      Blocks.Add(block);
      return true;
    }
  }
}
