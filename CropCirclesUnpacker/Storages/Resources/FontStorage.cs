using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using CropCirclesUnpacker.Assets;

namespace CropCirclesUnpacker.Storages.Resources
{
  public class FontStorage : ResourceStorage
  {
    private GlythOffset[] GlythOffsets;

    private FontStorage(string libraryPath)
      : base(libraryPath)
    {
      GlythOffsets = new GlythOffset[0];
    }

    public static Font ReadFromFile(string filePath, Palette palette)
    {
      FontStorage storage = new FontStorage(filePath);
      storage.ParseFile();

      //TODO(adm244): convert parsed data into a Font object
      int width = storage.Width;
      int height = storage.Height;
      PixelFormat format = PixelFormat.Format8bppIndexed;
      int bytesPerPixel = 1;
      
      System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, format);

      BitmapData lockData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, format);

      IntPtr p = lockData.Scan0;
      for (int row = 0; row < height; ++row)
      {
        Marshal.Copy(storage.Pixels, row * width * bytesPerPixel, p, width * bytesPerPixel);
        p = new IntPtr(p.ToInt64() + lockData.Stride);
      }

      bitmap.UnlockBits(lockData);

      return new Font(bitmap);
    }

    protected override bool ParseSection(BinaryReader inputReader, Section section)
    {
      bool result = false;

      switch (section.Type)
      {
        case SectionType.NUMO:
          result = ParseNUMOSection(inputReader);
          break;
        case SectionType.OFFS:
          result = ParseOFFSSection(inputReader);
          break;

        default:
          Debug.Assert(false, "Section is not implemented");
          break;
      }

      return result;
    }

    private bool ParseNUMOSection(BinaryReader inputReader)
    {
      Int32 offsetsCount = inputReader.ReadInt32();
      GlythOffsets = new GlythOffset[offsetsCount];

      return true;
    }

    private bool ParseOFFSSection(BinaryReader inputReader)
    {
      for (int i = 0; i < GlythOffsets.Length; ++i)
      {
        GlythOffsets[i].Left = inputReader.ReadInt32();
        GlythOffsets[i].Right = inputReader.ReadInt32();
      }

      return true;
    }

    private struct GlythOffset
    {
      public Int32 Left;
      public Int32 Right;
    }
  }
}
