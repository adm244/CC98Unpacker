
namespace CropCirclesUnpacker.Assets
{
  public class Entity : Asset
  {
    public Texture Texture;
    public byte[] OFFI;
    public byte[] OFFS;

    public Entity(string name, Texture texture, byte[] offi, byte[] offs)
      : base(name, AssetType.Entity)
    {
      Texture = texture;

      OFFI = new byte[offi.Length];
      offi.CopyTo(OFFI, 0);

      OFFS = new byte[offs.Length];
      offs.CopyTo(OFFS, 0);
    }
  }
}
