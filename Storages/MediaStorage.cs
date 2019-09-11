using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CropCirclesUnpacker.Storages
{
  public class MediaStorage : BaseStorage
  {
    public static readonly string FolderName = "GMedia";

    private MediaStorage(string libraryPath)
      : base(libraryPath)
    {
    }

    protected override bool ParseSection(BinaryReader inputReader, SectionNames sectionName)
    {
      throw new NotImplementedException();
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
  }
}
