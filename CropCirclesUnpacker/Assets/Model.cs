using CropCirclesUnpacker.Assets.ModelBlocks;

namespace CropCirclesUnpacker.Assets
{
  public class Model : Asset
  {
    public ModelBlock[] Blocks
    {
      get;
      private set;
    }

    public Model(string name, ModelBlock[] blocks)
      : base(name, AssetType.Model)
    {
      Blocks = new ModelBlock[blocks.Length];
      blocks.CopyTo(Blocks, 0);
    }
  }
}
