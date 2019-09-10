using System.IO;
using System.Text;

namespace CropCirclesUnpacker.Extensions
{
  public static class BinaryReaderExtension
  {
    public static string ReadCString(this BinaryReader reader)
    {
      StringBuilder sb = new StringBuilder();
      while (reader.BaseStream.Position != reader.BaseStream.Length)
      {
        char symbol = reader.ReadChar();
        if (symbol == 0) break;
        sb.Append(symbol);
      }

      return sb.ToString();
    }
  }
}
