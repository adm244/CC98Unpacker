using System;
using System.IO;
using System.Text;

namespace CropCirclesUnpacker.Extensions
{
  public static class BinaryWriterExtension
  {
    public static void WriteBytes(this BinaryWriter writer, byte value, int count)
    {
      for (int i = 0; i < count; ++i)
      {
        writer.Write(value);
      }
    }

    public static void WriteStringAsUInt32(this BinaryWriter writer, object value)
    {
      string str = (value is string) ? (string)value : value.ToString();

      if (string.IsNullOrEmpty(str))
      {
        writer.Write((Int32)0);
      }
      else
      {
        byte[] stringBytes = Encoding.ASCII.GetBytes(str);
        if (stringBytes.Length != 4)
          throw new InvalidDataException();

        byte[] outputBytes = new byte[4];
        outputBytes[0] = stringBytes[3];
        outputBytes[1] = stringBytes[2];
        outputBytes[2] = stringBytes[1];
        outputBytes[3] = stringBytes[0];

        writer.Write(outputBytes);
      }
    }

    public static void WriteFixedString(this BinaryWriter writer, string value, Encoding encoding)
    {
      byte[] stringBytes = encoding.GetBytes(value);
      writer.Write(stringBytes);
    }

    public static void WriteCString(this BinaryWriter writer, string value, Encoding encoding)
    {
      writer.WriteFixedString(value, encoding);
      writer.Write((byte)0);
    }
  }
}
