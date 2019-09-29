using System;

namespace CropCirclesUnpacker.Extensions
{
  public static class ArrayExtension
  {
    public static bool AreEqual(this Array left, Array right)
    {
      if (left == right)
        return true;

      if (left.Length != right.Length)
        return false;

      for (int i = 0; i < left.Length; ++i)
      {
        object leftValue = left.GetValue(i);
        object rightValue = right.GetValue(i);
        if (!leftValue.Equals(rightValue))
          return false;
      }

      return true;
    }
  }
}
