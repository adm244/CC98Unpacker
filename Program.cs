using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Storages;

namespace CropCirclesUnpacker
{
  class Program
  {
    public static void Main(string[] args)
    {
      string filePath = args[0];
      
      // media.dat extraction
      /*MediaStorage media = MediaStorage.ReadFromFile(filePath);
      media.ExtractTo(Environment.CurrentDirectory);*/

      // zft parsing
      Font font = FontStorage.ReadFromFile(filePath);
    }
  }
}
