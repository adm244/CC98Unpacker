using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using CropCirclesUnpacker.Extensions;
using CropCirclesUnpacker.Utils;

namespace CropCirclesUnpacker.Storages
{
  public class MediaStorage
  {
    private static readonly Int32 Signature = 0x6F72657A; // "zero"

    public string LibraryPath;
    private Asset[] Assets;
    private AssetInfo[] AssetsInfo;
    private string[] Folders;
    private string[] FileExtensions;

    private Encoding Encoding = Encoding.GetEncoding(1252);

    public Asset[] Contents
    {
      get { return Assets; }
    }

    private MediaStorage(string libraryPath)
    {
      LibraryPath = libraryPath;
      Assets = new Asset[0];
      AssetsInfo = new AssetInfo[0];
      Folders = new string[0];
      FileExtensions = new string[0];
    }

    private bool ParseArchive()
    {
      using (FileStream inputStream = new FileStream(LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, Encoding))
        {
          Console.WriteLine("Parsing {0}...", Path.GetFileName(LibraryPath));

          if (!IsValidFile(inputReader))
          {
            Console.WriteLine("Failed. Invalid or corrupt file detected!");
            return false;
          }

          //NOTE(adm244): do we care about attributes?
          // First character specifies file type: binary ('b') or text ('a')
          // Second character specifies file endianess: little ('l') or big ('b')
          // The rest characters are set to '_' and are ignored.
          char[] attributes = inputReader.ReadChars(4);

          AssetsInfo = ParseAssetsTable(inputReader);
          Folders = ParseStrings(inputReader);
          FileExtensions = ParseStrings(inputReader);

          PrepareAssets();

          Console.WriteLine("Done!");
        }
      }

      return true;
    }

    private bool IsValidFile(BinaryReader inputReader)
    {
      Int32 signature = inputReader.ReadInt32();
      if (signature != Signature)
        return false;

      return true;
    }

    private AssetInfo[] ParseAssetsTable(BinaryReader inputReader)
    {
      FilesTableInfo tableInfo = GetFilesTableInfo(inputReader);
      inputReader.BaseStream.Seek(tableInfo.Offset, SeekOrigin.Begin);

      Console.WriteLine("\tReading assets table...");

      AssetInfo[] assetsInfo = new AssetInfo[tableInfo.Count];
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

    private void PrepareAssets()
    {
      Assets = new Asset[AssetsInfo.Length];
      for (int i = 0; i < Assets.Length; ++i)
      {
        Assets[i].FileName = AssetsInfo[i].Name;
        Assets[i].Folder = Folders[AssetsInfo[i].FolderIndex];
        Assets[i].Extension = FileExtensions[AssetsInfo[i].ExtensionIndex];
        Assets[i].FullFileName = Assets[i].FileName + Assets[i].Extension;
      }
    }

    public static MediaStorage ReadFromFile(string filePath)
    {
      MediaStorage mediaFile = new MediaStorage(filePath);
      if (!mediaFile.ParseArchive())
      {
        Console.WriteLine("ERROR: Could not parse {0}", filePath);
        return null;
      }

      return mediaFile;
    }

    public void ExtractTo(string targetFolder)
    {
      using (FileStream inputStream = new FileStream(LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream))
        {
          for (int i = 0; i < Assets.Length; ++i)
          {
            string fileFolder = Folders[AssetsInfo[i].FolderIndex];
            string fileExtension = FileExtensions[AssetsInfo[i].ExtensionIndex];
            string fileFullName = AssetsInfo[i].Name + fileExtension;

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

    private struct AssetInfo
    {
      public Int16 FolderIndex;
      public Int16 ExtensionIndex;
      public Int32 Offset;
      public Int32 Size;
      public string Name;
    }
    
    //TODO(adm244): private set
    public struct Asset
    {
      public string Folder;
      public string FileName;
      public string Extension;
      public string FullFileName;
    }
  }
}
