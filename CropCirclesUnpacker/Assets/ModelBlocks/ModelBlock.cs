﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class ModelBlock
  {
    public static Encoding Encoding = Encoding.GetEncoding(1251);

    protected string ContentName;
    protected uint Flags;

    public List<ModelBlock> SubBlocks
    {
      get;
      private set;
    }

    public BlockType Type
    {
      get;
      private set;
    }

    protected ModelBlock()
    {
      throw new InvalidOperationException();
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

    public virtual bool Write(BinaryWriter outputWriter)
    {
      outputWriter.WriteStringAsUInt32(Type.ToString());
      outputWriter.WriteStringAsUInt32(ContentName);
      outputWriter.Write((UInt32)Flags);

      return true;
    }

    public static ModelBlock ParseBlock(BinaryReader inputReader)
    {
      string typeName = inputReader.ReadUInt32AsString();
      if (!Enum.IsDefined(typeof(BlockType), typeName))
      {
        Trace.Assert(false, "Undefined block encountered!");
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

    protected bool ParseSubBlocks(BinaryReader inputReader)
    {
      bool continueParsing = false;
      do
      {
        string subBlockTypeName = inputReader.ReadUInt32AsString();
        if (!Enum.IsDefined(typeof(SubBlockType), subBlockTypeName))
        {
          Trace.Assert(false, "Undefined sub block type encountered!");
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
            Trace.Assert(false, "Unimplemented sub block type!");
            return false;
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

    protected bool WriteSubBlocks(BinaryWriter outputWriter)
    {
      for (int i = 0; i < SubBlocks.Count; ++i)
      {
        if (SubBlocks[i].Type != BlockType.CMod)
          outputWriter.WriteStringAsUInt32(SubBlockType.CInt.ToString());

        if (!SubBlocks[i].Write(outputWriter))
          return false;
      }

      outputWriter.WriteStringAsUInt32(SubBlockType.CEnd.ToString());

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
        case BlockType.PScn:
          return new PScnModelBlock();
        case BlockType.RBtn:
          return new RBtnModelBlock();
        case BlockType.MScn:
          return new MScnModelBlock();
        case BlockType.RctV:
          return new RctVModelBlock();
        case BlockType.GEdt:
          return new GEdtModelBlock();
        case BlockType.GBut:
          return new GButModelBlock();
        case BlockType.PshB:
          return new PshBModelBlock();
        case BlockType.GLst:
          return new GLstModelBlock();

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
      PScn,
      RBtn,
      MScn,
      RctV,
      GEdt,
      GBut,
      PshB,
      GLst,
    }
  }
}
