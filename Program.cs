using CropCirclesUnpacker.Storages;
using System;

namespace CropCirclesUnpacker
{
  class Program
  {
    public static void Main(string[] args)
    {
      string filePath = args[0];
      MediaStorage media = MediaStorage.ReadFromFile(filePath);
      media.ExtractTo(Environment.CurrentDirectory);
    }
  }
}
