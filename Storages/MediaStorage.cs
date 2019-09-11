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
    public static readonly string FolderName = "GMedia";

    private static readonly Int32 Signature = 0x6F72657A; // "zero"

    private string LibraryPath;
    private Asset[] Assets;
    private string[] Folders;
    private string[] FileExtensions;

    private MediaStorage(string libraryPath)
    {
      LibraryPath = libraryPath;
      Assets = new Asset[0];
      Folders = new string[0];
      FileExtensions = new string[0];
    }

    public static MediaStorage ReadFromFile(string filePath)
    {
      MediaStorage mediaFile = new MediaStorage(filePath);

      using (FileStream inputStream = new FileStream(mediaFile.LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream))
        {
          // verify head signature
          Int32 signature = inputReader.ReadInt32();
          Debug.Assert(signature == Signature);

          // read files table info
          inputReader.BaseStream.Seek(-(2 * sizeof(Int32)), SeekOrigin.End);

          Int32 filesTableOffset = inputReader.ReadInt32();
          Int32 filesTableCount = inputReader.ReadInt32();

          // read files table
          inputReader.BaseStream.Seek(filesTableOffset, SeekOrigin.Begin);

          mediaFile.Assets = new Asset[filesTableCount];
          for (int i = 0; i < mediaFile.Assets.Length; ++i)
          {
            mediaFile.Assets[i].FolderIndex = inputReader.ReadInt16();
            mediaFile.Assets[i].ExtensionIndex = inputReader.ReadInt16();
            mediaFile.Assets[i].Offset = inputReader.ReadInt32();
            mediaFile.Assets[i].Size = inputReader.ReadInt32();
            mediaFile.Assets[i].Name = inputReader.ReadCString();
          }

          // read folder names
          mediaFile.Folders = ReadNames(inputReader);

          // read file extensions
          mediaFile.FileExtensions = ReadNames(inputReader);
        }
      }

      return mediaFile;
    }

    public void ExtractTo(string targetFolder)
    {
      targetFolder = Path.Combine(targetFolder, FolderName);

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

    private static string[] ReadNames(BinaryReader reader)
    {
      Int32 dataSize = reader.ReadInt32();
      byte[] rawData = reader.ReadBytes(dataSize);
      Int32 stringsCount = reader.ReadInt32();

      string[] names = StringUtils.ConvertNullTerminatedSequence(rawData);
      Debug.Assert(names.Length == stringsCount);

      return names;
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
