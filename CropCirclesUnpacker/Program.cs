using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Storages.Resources;

namespace CropCirclesUnpacker
{
  class Program
  {
    public static void Main(string[] args)
    {
      string filePath = args[0];
      //string palettePath = args[1];
      /*string fontPath = args[1];*/

      //Model model = ModelStorage.LoadFromFile(filePath);

      /*List<Model> models = new List<Model>();
      string[] files = Directory.GetFiles(filePath);
      for (int i = 0; i < files.Length; ++i)
      {
        if (Path.GetExtension(files[i]) != ".mod")
          continue;

        Model model = ModelStorage.LoadFromFile(files[i]);
        if (model != null)
          models.Add(model);
      }

      //int end = 0;

      string folderPath = args[1];
      Directory.CreateDirectory(folderPath);

      for (int i = 0; i < models.Count; ++i)
      {
        string path = Path.Combine(folderPath, models[i].Name + ".mod");
        ModelStorage.SaveToFile(path, models[i]);
      }

      int end = 0;*/

      /*List<string> lines = new List<string>();
      for (int i = 0; i < models.Count; ++i)
      {
        ModelBlock[] blocks = models[i].Blocks;
        for (int j = 0; j < blocks.Length; ++j)
        {
          List<ModelBlock> subBlocks = blocks[j].SubBlocks;
          for (int k = 0; k < subBlocks.Count; ++k)
          {
            if (subBlocks[k].Type != ModelBlock.BlockType.TxtV)
              continue;

            string text = ((TxtVModelBlock)subBlocks[k]).Text;
            if (!string.IsNullOrEmpty(text))
            {
              text = text.Replace("\n", "\\n");
              lines.Add(text);
            }
          }
        }
      }

      File.WriteAllLines("extracted_text.txt", lines.ToArray());*/

      // menu.dat extraction
      /*MediaStorage media = MediaStorage.ReadFromFile(palettePath);
      media.ExtractTo(Environment.CurrentDirectory);*/

      // media.dat extraction
      /*MediaStorage media = MediaStorage.ReadFromFile(filePath);
      media.ExtractTo(Environment.CurrentDirectory);*/

      // clr parsing
      Palette palette = PaletteStorage.LoadFromFile(filePath);
      PaletteStorage.SaveToFile(filePath + ".new.clr", palette);

      // zft parsing
      //Font font = FontStorage.ReadFromFile(filePath);

      // zim parsing
      //Sprite sprite = ImageStorage.ReadFromFile(filePath);
      //System.Drawing.Bitmap spriteImage = sprite.CreateBitmap(palette);
      //spriteImage.Save("test.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

      /*for (int ii = 0; ii < (palette.Lookups.Length / palette.Entries.Length); ++ii)
      {
        ColorPalette bitmapPalette = font.Texture.Palette;
        for (int j = 0; j < bitmapPalette.Entries.Length; ++j)
        {
          int index = palette.Lookups[j + palette.Entries.Length * ii];
          int value = palette.Entries[index];

          int red = (value & 0xFF);
          int green = ((value >> 8) & 0xFF);
          int blue = ((value >> 16) & 0xFF);
          int alpha = ((value >> 32) & 0xFF) == 0 ? 0 : 255;

          System.Drawing.Color color = System.Drawing.Color.FromArgb(alpha, red, green, blue);
          bitmapPalette.Entries[j] = color;
        }
        font.Texture.Palette = bitmapPalette;

        Directory.CreateDirectory(string.Format("{0}", Path.GetFileNameWithoutExtension(fontPath)));
        font.Texture.Save(string.Format("{0}/pal{1}.png", Path.GetFileNameWithoutExtension(fontPath), ii), ImageFormat.Png);
      }*/

      // zim (sprite) parsing
      /*string[] sprites = Directory.GetFiles(Path.GetDirectoryName(filePath));
      for (int i = 0; i < sprites.Length; ++i)
      {
        if (Path.GetExtension(sprites[i]) != ".zim")
          continue;

        Sprite sprite = ImageStorage.ReadFromFile(sprites[i]);
        //Bitmap bitmap = sprite.CreateBitmap(palette);
        //bitmap.Save(string.Format("{0}.bmp", Path.GetFileNameWithoutExtension(sprites[i])), ImageFormat.Bmp);
      }*/
    }
  }
}
