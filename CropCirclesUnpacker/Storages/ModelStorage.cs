using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Assets.ModelBlocks.Base;
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
      if (!base.Parse(inputReader))
        return false;

      //NOTE(adm244): check for duplicated header
      Int32 magic = inputReader.PeekInt32();
      if (magic == Signature)
      {
        if (!base.Parse(inputReader))
          return false;
      }

      while (!inputReader.EOF())
      {
        ModelBlock block = ModelBlock.ParseBlock(inputReader);
        if (block == null)
        {
          Debug.Assert(false, "Could not parse a block!");
          break;
        }

        Blocks.Add(block);
      }

      return (Blocks.Count > 0);
    }
  }
}
