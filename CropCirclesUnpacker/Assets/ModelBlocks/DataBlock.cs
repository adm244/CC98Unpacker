using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks
{
  public class DataBlock
  {
    public string Name
    {
      get;
      private set;
    }

    public List<DataValue> Values
    {
      get;
      private set;
    }

    public DataBlockType Type
    {
      get;
      private set;
    }

    private DataBlock(DataBlockType type)
    {
      Name = string.Empty;
      Values = new List<DataValue>();
      Type = type;
    }

    private bool Parse(BinaryReader inputReader)
    {
      Name = inputReader.ReadUInt32AsString();

      while (true)
      {
        Int32 unk01 = inputReader.ReadInt32();
        if (unk01 == -1)
          break;

        Int32 unk02 = default(int);
        string name = string.Empty;

        if (unk01 < 0)
          unk02 = inputReader.ReadInt32();
        else
          name = inputReader.ReadUInt32AsString();

        DataValue value = new DataValue(unk01, unk02, name);
        Values.Add(value);
      }

      return (Values.Count > 0);
    }

    private static DataBlockType GetType(string typeName)
    {
      try
      {
        return (DataBlockType)Enum.Parse(typeof(DataBlockType), typeName, true);
      }
      catch
      {
        Debug.Assert(false, "Undefined data block type encountered!");
        return DataBlockType.Undefined;
      }
    }

    public static DataBlock[] ParseBlocks(BinaryReader inputReader)
    {
      List<DataBlock> blocks = new List<DataBlock>();

      bool continueParsing = true;
      do
      {
        string typeName = inputReader.ReadUInt32AsString();
        DataBlockType type = GetType(typeName);
        switch (type)
        {
          case DataBlockType.Timr:
          case DataBlockType.Sttc:
            {
              DataBlock block = ParseBlock(inputReader, type);
              if (block == null)
                return new DataBlock[0];

              blocks.Add(block);
            }
            break;

          case DataBlockType.End_:
            continueParsing = false;
            break;

          default:
            Debug.Assert(false, "Unimplemented data block type!");
            break;
        }
      } while (continueParsing);

      return blocks.ToArray();
    }

    private static DataBlock ParseBlock(BinaryReader inputReader, DataBlockType type)
    {
      DataBlock block = new DataBlock(type);
      if (!block.Parse(inputReader))
      {
        Debug.Assert(false, "Could not parse a data block!");
        return null;
      }

      return block;
    }

    public struct DataValue
    {
      public int Unk01;
      public int Unk02;
      public string Name;

      public DataValue(int unk01, int unk02, string name)
      {
        Unk01 = unk01;
        Unk02 = unk02;
        Name = name;
      }
    }

    public enum DataBlockType
    {
      Undefined = 0,
      End_,
      Timr,
      Sttc,
    }
  }
}
