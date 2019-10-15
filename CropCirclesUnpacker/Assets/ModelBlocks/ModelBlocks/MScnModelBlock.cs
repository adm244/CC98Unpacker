using System;
using System.Collections.Generic;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets.ModelBlocks.ModelBlocks
{
  public class MScnModelBlock : PScnModelBlock
  {
    private List<ActionData> Actions;
    private Int32 Unk01;

    public MScnModelBlock()
      : base(BlockType.MScn)
    {
      Actions = new List<ActionData>();
      Unk01 = default(Int32);
    }

    public override bool Parse(BinaryReader inputReader)
    {
      if (!base.Parse(inputReader))
        return false;

      Unk01 = inputReader.ReadInt32();

      while (true)
      {
        Int32 actionValue = inputReader.ReadInt32();
        if (actionValue == -1)
          break;

        inputReader.BaseStream.Seek(-sizeof(Int32), SeekOrigin.Current);

        ActionData action;

        action.ActionName = inputReader.ReadUInt32AsString();
        action.Unk02 = inputReader.ReadInt32();
        action.Unk03 = inputReader.ReadUInt64();
        action.Unk04 = inputReader.ReadUInt64();
        action.Name = inputReader.ReadUInt32AsString();
        action.Unk06 = inputReader.ReadInt32();
        action.Unk07 = inputReader.ReadInt32();
        action.Unk08 = inputReader.ReadInt32();
        action.Unk09 = inputReader.ReadInt32();

        Actions.Add(action);
      }

      return true;
    }

    public override bool Write(BinaryWriter outputWriter)
    {
      if (!base.Write(outputWriter))
        return false;

      outputWriter.Write((Int32)Unk01);

      for (int i = 0; i < Actions.Count; ++i)
      {
        outputWriter.WriteStringAsUInt32(Actions[i].ActionName);
        outputWriter.Write((Int32)Actions[i].Unk02);
        outputWriter.Write((UInt64)Actions[i].Unk03);
        outputWriter.Write((UInt64)Actions[i].Unk04);
        outputWriter.WriteStringAsUInt32(Actions[i].Name);
        outputWriter.Write((Int32)Actions[i].Unk06);
        outputWriter.Write((Int32)Actions[i].Unk07);
        outputWriter.Write((Int32)Actions[i].Unk08);
        outputWriter.Write((Int32)Actions[i].Unk09);
      }

      outputWriter.Write((Int32)(-1));

      return true;
    }

    public struct ActionData
    {
      public string ActionName;
      public Int32 Unk02;
      public UInt64 Unk03;
      public UInt64 Unk04;
      public string Name;
      public Int32 Unk06;
      public Int32 Unk07;
      public Int32 Unk08;
      public Int32 Unk09;
    }
  }
}
