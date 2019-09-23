using System.Collections.Generic;

namespace CropCirclesUnpacker.Utils
{
  public static class StringUtils
  {
    public static unsafe string[] ConvertNullTerminatedSequence(byte[] buffer)
    {
      List<string> strings = new List<string>();
      int startpos = 0;
      for (int i = 0; i < buffer.Length; ++i)
      {
        if (buffer[i] == 0)
        {
          fixed (byte* p = &buffer[startpos])
          {
            strings.Add(new string((sbyte*)p));
          }
          startpos = (i + 1);
        }
      }

      return strings.ToArray();
    }
  }
}
