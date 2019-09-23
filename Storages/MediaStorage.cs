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

    private string LibraryPath;
    private Asset[] Assets;
    private string[] Folders;
    private string[] FileExtensions;

    private Encoding Encoding = Encoding.GetEncoding(1252);

    private MediaStorage(string libraryPath)
    {
      LibraryPath = libraryPath;
      Assets = new Asset[0];
      Folders = new string[0];
      FileExtensions = new string[0];
    }

    private bool ParseArchive()
    {
      bool result = false;

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

          result = ParseAssetsTable(inputReader);
          Folders = ParseStrings(inputReader);
          FileExtensions = ParseStrings(inputReader);

          Console.WriteLine("Done!");
        }
      }

      return result;
    }

    private bool IsValidFile(BinaryReader inputReader)
    {
      Int32 signature = inputReader.ReadInt32();
      if (signature != Signature)
        return false;

      return true;
    }

    private bool ParseAssetsTable(BinaryReader inputReader)
    {
      FilesTableInfo tableInfo = GetFilesTableInfo(inputReader);
      inputReader.BaseStream.Seek(tableInfo.Offset, SeekOrigin.Begin);

      Console.WriteLine("\tReading assets table...");

      Assets = new Asset[tableInfo.Count];
      for (int i = 0; i < Assets.Length; ++i)
      {
        Assets[i].FolderIndex = inputReader.ReadInt16();
        Assets[i].ExtensionIndex = inputReader.ReadInt16();
        Assets[i].Offset = inputReader.ReadInt32();
        Assets[i].Size = inputReader.ReadInt32();
        Assets[i].Name = inputReader.ReadCString();

        Console.WriteLine("\t\tFound {0} asset", Assets[i].Name);
      }

      Console.WriteLine("\tDone!");

      return true;
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
            string fileFolder = Folders[Assets[i].FolderIndex];
            string fileExtension = FileExtensions[Assets[i].ExtensionIndex];
            string fileFullName = Assets[i].Name + fileExtension;

            string folderPath = Path.Combine(targetFolder, fileFolder);
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileFullName);
            using (FileStream outputStream = new FileStream(filePath, FileMode.Create))
            {
              using (BinaryWriter outputWriter = new BinaryWriter(outputStream, Encoding.GetEncoding(1252)))
              {
                inputReader.BaseStream.Seek(Assets[i].Offset, SeekOrigin.Begin);

                Console.Write("\tExtracting {0}...", fileFullName);

                // 1048576 bytes = 1 mb
                byte[] buffer = new byte[1048576];
                long bytesRead = 0;
                while (bytesRead < Assets[i].Size)
                {
                  long bytesLeftToRead = Assets[i].Size - bytesRead;
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

    private struct Asset
    {
      public Int16 FolderIndex;
      public Int16 ExtensionIndex;
      public Int32 Offset;
      public Int32 Size;
      public string Name;
    }
  }
}
