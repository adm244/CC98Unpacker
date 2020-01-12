
namespace CropCirclesUnpacker.Assets
{
  public abstract class Asset
  {
    public Asset(string name, AssetType type)
    {
      Name = name;
      Type = type;
    }

    public override string ToString()
    {
      return Name;
    }

    public string Name
    {
      get;
      private set;
    }

    public AssetType Type
    {
      get;
      private set;
    }

    public enum AssetType
    {
      Unknown = 0,
      Font,
      Palette,
      Texture,
      Entity,
      Model,
    }
  }
}
