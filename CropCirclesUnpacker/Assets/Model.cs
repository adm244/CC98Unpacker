using CropCirclesUnpacker.Assets.ModelBlocks;

namespace CropCirclesUnpacker.Assets
{
  public class Model : Asset
  {
    private ModelBlock[] Blocks = null;

    public Model(string name, ModelBlock[] blocks)
      : base(name, AssetType.Model)
    {
      Blocks = new ModelBlock[blocks.Length];
      blocks.CopyTo(Blocks, 0);
    }
  }
}
