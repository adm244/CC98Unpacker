using System;
using System.IO;
using System.Text;

namespace CropCirclesUnpacker.Storages
{
  public abstract class BaseStorage
  {
    public static readonly Int32 Signature = 0x6F72657A; // "zero"
    public readonly Encoding Encoding = Encoding.GetEncoding(1252);

    public readonly string LibraryPath;

    protected BaseStorage(string libraryPath)
    {
      LibraryPath = libraryPath;
    }

    //TODO(adm244): make this virtual
    protected abstract bool Parse(BinaryReader inputReader);

    protected bool ReadHeader(BinaryReader inputReader)
    {
      if (!IsValidFile(inputReader))
        return false;

      //NOTE(adm244): do we care about attributes?
      // First character specifies file type: binary ('b') or text ('a')
      // Second character specifies file endianess: little ('l') or big ('b')
      // The rest characters are set to '_' and are ignored.
      char[] attributes = inputReader.ReadChars(4);

      return true;
    }

    protected bool IsValidFile(BinaryReader inputReader)
    {
      Int32 signature = inputReader.ReadInt32();
      if (signature != Signature)
        return false;

      return true;
    }
  }
}
