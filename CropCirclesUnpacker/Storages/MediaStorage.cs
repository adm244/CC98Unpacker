using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Extensions;
using CropCirclesUnpacker.Storages.Resources;
using CropCirclesUnpacker.Utils;

namespace CropCirclesUnpacker.Storages
{
  public class MediaStorage : BaseStorage
  {
    private List<Asset> Assets;

    private MediaStorage(string libraryPath)
      : base(libraryPath)
    {
      Assets = new List<Asset>();
      AssetsInfo = new AssetInfo[0];
    }

    public AssetInfo[] AssetsInfo
    {
      get;
      private set;
    }

    private bool Parse()
    {
      bool result = false;

      using (FileStream inputStream = new FileStream(LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, Encoding))
        {
          result = Parse(inputReader);
        }
      }

      return result;
    }

    protected override bool Parse(BinaryReader inputReader)
    {
      Console.WriteLine("Parsing {0}...", Path.GetFileName(LibraryPath));

      if (!base.Parse(inputReader))
      {
        Console.WriteLine("Failed. Invalid or corrupt file detected!");
        return false;
      }

      AssetInfoRaw[] assetsInfoRaw = ParseAssetsTable(inputReader);
      string[] folders = ParseStrings(inputReader);
      string[] extensions = ParseStrings(inputReader);

      PrepareAssets(assetsInfoRaw, folders, extensions);

      Console.WriteLine("Done!");

      return true;
    }

    private AssetInfoRaw[] ParseAssetsTable(BinaryReader inputReader)
    {
      FilesTableInfo tableInfo = GetFilesTableInfo(inputReader);
      inputReader.BaseStream.Seek(tableInfo.Offset, SeekOrigin.Begin);

      Console.WriteLine("\tReading assets table...");

      AssetInfoRaw[] assetsInfo = new AssetInfoRaw[tableInfo.Count];
      for (int i = 0; i < assetsInfo.Length; ++i)
      {
        assetsInfo[i].FolderIndex = inputReader.ReadInt16();
        assetsInfo[i].ExtensionIndex = inputReader.ReadInt16();
        assetsInfo[i].Offset = inputReader.ReadInt32();
        assetsInfo[i].Size = inputReader.ReadInt32();
        assetsInfo[i].Name = inputReader.ReadCString();

        Console.WriteLine("\t\tFound {0} asset", assetsInfo[i].Name);
      }

      Console.WriteLine("\tDone!");

      return assetsInfo;
    }

    private FilesTableInfo GetFilesTableInfo(BinaryReader inputReader)
    {
      FilesTableInfo tableInfo;

      Console.Write("\tLocating files table...");

      inputReader.BaseStream.Seek(-(2 * sizeof(Int32)), SeekOrigin.End);

      tableInfo.Offset = inputReader.ReadInt32();
      tableInfo.Count = inputReader.ReadInt32();

      Console.WriteLine(" Done!");

      return tableInfo;
    }

    private static string[] ParseStrings(BinaryReader inputReader)
    {
      Console.Write("\tParsing strings...");

      Int32 dataSize = inputReader.ReadInt32();
      byte[] rawData = inputReader.ReadBytes(dataSize);
      Int32 stringsCount = inputReader.ReadInt32();

      string[] names = StringUtils.ConvertNullTerminatedSequence(rawData);
      Debug.Assert(names.Length == stringsCount);

      Console.WriteLine(" Done!");

      return names;
    }

    private void PrepareAssets(AssetInfoRaw[] assetsInfoRaw, string[] folders, string[] extensions)
    {
      AssetsInfo = new AssetInfo[assetsInfoRaw.Length];
      for (int i = 0; i < AssetsInfo.Length; ++i)
      {
        AssetsInfo[i].Name = assetsInfoRaw[i].Name;
        AssetsInfo[i].Folder = folders[assetsInfoRaw[i].FolderIndex];
        AssetsInfo[i].Extension = extensions[assetsInfoRaw[i].ExtensionIndex];
        AssetsInfo[i].FullFileName = AssetsInfo[i].Name + AssetsInfo[i].Extension;
        AssetsInfo[i].Offset = assetsInfoRaw[i].Offset;
        AssetsInfo[i].Size = assetsInfoRaw[i].Size;
      }
    }

    public static MediaStorage ReadFromFile(string filePath)
    {
      MediaStorage mediaFile = new MediaStorage(filePath);
      if (!mediaFile.Parse())
      {
        Console.WriteLine("ERROR: Could not parse {0}", filePath);
        return null;
      }

      int filesCount = mediaFile.LoadContents();
      if (filesCount < 1)
        Debug.Assert(false, "Could not load archive contents");

      return mediaFile;
    }

    private int LoadContents()
    {
      using (FileStream inputStream = new FileStream(LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, Encoding))
        {
          for (int i = 0; i < AssetsInfo.Length; ++i)
          {
            inputReader.BaseStream.Seek(AssetsInfo[i].Offset, SeekOrigin.Begin);

            byte[] buffer = inputReader.ReadBytes(AssetsInfo[i].Size);
            if (buffer.Length != AssetsInfo[i].Size)
            {
              Debug.Assert(false, "Possible data corruption");
              continue;
            }

            MemoryStream memoryStream = new MemoryStream(buffer);
            BinaryReader resourceStream = new BinaryReader(memoryStream, Encoding);

            switch (AssetsInfo[i].Extension)
            {
              case ".clr":
                {
                  Palette palette = PaletteStorage.LoadFromStream(resourceStream, AssetsInfo[i].Name);
                  if (palette == null)
                    Debug.Assert(false, "Could not read a palette data from a stream");
                  else
                    Assets.Add(palette);
                }
                break;

              case ".zim":
                {
                  //TODO(adm244): unify ImageResourceStorage loaders
                  //Texture sprite = TextureStorage.LoadFromStream(resourceStream, AssetsInfo[i].Name);
                  //if (sprite == null)
                  //  Debug.Assert(false, "Could not read a sprite data from a stream");
                  //else
                  //  Assets.Add(sprite);
                }
                break;
              case ".zft":
                {
                  Font font = FontStorage.LoadFromStream(resourceStream, AssetsInfo[i].Name);
                  if (font == null)
                    Debug.Assert(false, "Could not read a font data from a stream");
                  else
                    Assets.Add(font);
                }
                break;

              default:
                //Debug.Assert(false, "Not implemented file extension");
                break;
            }
          }
        }
      }

      return Assets.Count;
    }

    public Asset GetAsset(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        Debug.Assert(false, "Asset name is null or empty");
        return null;
      }

      for (int i = 0; i < Assets.Count; ++i)
      {
        if (Assets[i].Name == name)
          return Assets[i];
      }

      return null;
    }

    public Asset[] GetAssets(Asset.AssetType type)
    {
      List<Asset> assets = new List<Asset>();

      for (int i = 0; i < Assets.Count; ++i)
      {
        if (Assets[i].Type == type)
          assets.Add(Assets[i]);
      }

      return assets.ToArray();
    }

    public void ExtractTo(string targetFolder)
    {
      using (FileStream inputStream = new FileStream(LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, Encoding))
        {
          for (int i = 0; i < AssetsInfo.Length; ++i)
          {
            string fileFolder = AssetsInfo[i].Folder;
            string fileExtension = AssetsInfo[i].Extension;
            string fileFullName = AssetsInfo[i].FullFileName;

            string folderPath = Path.Combine(targetFolder, fileFolder);
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileFullName);
            using (FileStream outputStream = new FileStream(filePath, FileMode.Create))
            {
              using (BinaryWriter outputWriter = new BinaryWriter(outputStream, Encoding.GetEncoding(1252)))
              {
                inputReader.BaseStream.Seek(AssetsInfo[i].Offset, SeekOrigin.Begin);

                Console.Write("\tExtracting {0}...", fileFullName);

                // 1048576 bytes = 1 mb
                byte[] buffer = new byte[1048576];
                long bytesRead = 0;
                while (bytesRead < AssetsInfo[i].Size)
                {
                  long bytesLeftToRead = AssetsInfo[i].Size - bytesRead;
                  long bytesToRead = Math.Min(buffer.Length, bytesLeftToRead);

                  // make sure buffer length is within 32-bit boundary
                  buffer = inputReader.ReadBytes((int)bytesToRead);

                  //NOTE(adm244): check for end-of-stream
                  Debug.Assert(buffer.Length > 0);

                  outputWriter.Write(buffer);

                  bytesRead += bytesToRead;
                }

                Console.WriteLine(" Done!");
              }
            }
          }
        }
      }
    }

    private struct FilesTableInfo
    {
      public Int32 Offset;
      public Int32 Count;
    }

    private struct AssetInfoRaw
    {
      public Int16 FolderIndex;
      public Int16 ExtensionIndex;
      public Int32 Offset;
      public Int32 Size;
      public string Name;
    }
    
    //TODO(adm244): private set
    public struct AssetInfo
    {
      public string Folder;
      public string Name;
      public string Extension;
      public string FullFileName;
      public int Offset;
      public int Size;
    }
  }
}
