using System.IO;

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
  }
}
