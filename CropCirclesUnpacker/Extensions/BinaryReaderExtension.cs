using System;
using System.IO;
using System.Text;

namespace CropCirclesUnpacker.Extensions
{
  public static class BinaryReaderExtension
  {
    public static bool EOF(this BinaryReader reader)
    {
      return (reader.BaseStream.Position >= reader.BaseStream.Length);
    }

    public static string ReadCString(this BinaryReader reader)
    {
      StringBuilder sb = new StringBuilder();
      while (!reader.EOF())
      {
        char symbol = reader.ReadChar();
        if (symbol == 0) break;
        sb.Append(symbol);
      }

      return sb.ToString();
    }

    public static string ReadFixedString(this BinaryReader reader, int length)
    {
      char[] buffer = reader.ReadChars(length);

      int i;
      for (i = 0; i < buffer.Length; ++i)
      {
        if (buffer[i] == 0)
          break;
      }

      return new string(buffer, 0, i);
    }

    public static string ReadUInt32AsString(this BinaryReader reader)
    {
      UInt32 value = reader.ReadUInt32();
      if (value == 0)
        return string.Empty;
      
      byte[] buffer = new byte[4];
      buffer[0] = (byte)((value >> 24) & 0xFF);
      buffer[1] = (byte)((value >> 16) & 0xFF);
      buffer[2] = (byte)((value >> 8) & 0xFF);
      buffer[3] = (byte)((value) & 0xFF);

      return Encoding.ASCII.GetString(buffer);
    }

    public static Int32 PeekInt32(this BinaryReader reader)
    {
      Int32 value = reader.ReadInt32();
      reader.BaseStream.Seek(-sizeof(Int32), SeekOrigin.Current);
      
      return value;
    }
  }
}
