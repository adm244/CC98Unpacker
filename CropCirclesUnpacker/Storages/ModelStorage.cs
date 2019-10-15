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

    private ModelStorage(string filePath)
      : base(filePath)
    {
      Blocks = new List<ModelBlock>();
    }

    private ModelStorage(string filePath, Model model)
      : this(filePath)
    {
      Blocks.AddRange(model.Blocks);
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

    public static bool SaveToFile(string filePath, Model model)
    {
      if (string.IsNullOrEmpty(filePath))
        return false;

      ModelStorage storage = new ModelStorage(filePath, model);
      using (FileStream stream = new FileStream(storage.LibraryPath, FileMode.Create))
      {
        using (BinaryWriter outputWriter = new BinaryWriter(stream, storage.Encoding))
        {
          if (!storage.Write(outputWriter))
          {
            Debug.Assert(false, "Cannot write a model file!");
            return false;
          }
        }
      }

      return true;
    }

    public static bool SaveToStream(BinaryWriter outputWriter, Model model)
    {
      ModelStorage storage = new ModelStorage(string.Empty, model);
      if (!storage.Write(outputWriter))
      {
        Debug.Assert(false, "Cannot write a model file!");
        return false;
      }

      return true;
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

    protected override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      //NOTE(adm244): do we need to duplicate a header?
      if (!base.Write(outputWriter))
        return false;

      for (int i = 0; i < Blocks.Count; ++i)
      {
        if (!Blocks[i].Write(outputWriter))
        {
          Debug.Assert(false, "Could not write a block!");
          return false;
        }
      }

      return true;
    }
  }
}
