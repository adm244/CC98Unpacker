using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Storages;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpackerGUI
{
  public partial class MainForm : Form
  {
    private PreviewTypes PreviewType;
    private object PreviewObject;

    private List<MediaStorage> Archives;
    private List<Palette> Palettes;

    public MainForm()
    {
      InitializeComponent();

      PreviewType = PreviewTypes.None;
      PreviewObject = null;

      Archives = new List<MediaStorage>();
      Palettes = new List<Palette>();
      
      clearArchivesToolStripMenuItem.Enabled = false;
      extractToToolStripMenuItem.Enabled = false;
      btnImport.Enabled = false;
      btnExport.Enabled = false;
      ctrlPaletteSelector.Enabled = false;

      Console.SetOut(TextWriter.Null);
    }

    private void addArchiveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ctrlOpenFile.Title = "Select archive files";
      ctrlOpenFile.Filter = "Archive (*.dat)|*.dat";
      ctrlOpenFile.CheckFileExists = true;
      ctrlOpenFile.Multiselect = true;

      if (ctrlOpenFile.ShowDialog() == DialogResult.OK)
      {
        LoadArchives(ctrlOpenFile.FileNames);
      }
    }

    private void LoadArchives(string[] files)
    {
      ctrlStatusLabel.Text = "Loading...";

      //TODO(adm244): put into a separate thread
      for (int i = 0; i < files.Length; ++i)
      {
        MediaStorage storage = MediaStorage.ReadFromFile(files[i]);
        if (storage != null)
          Archives.Add(storage);
      }

      ctrlStatusLabel.Text = string.Format("{0} archives loaded.", Archives.Count);

      if (Archives.Count > 0)
        clearArchivesToolStripMenuItem.Enabled = true;

      FillTreeView();
      FillPaletteSelector();
    }

    private void FillPaletteSelector()
    {
      ctrlPaletteSelector.Items.Clear();
      for (int i = 0; i < Archives.Count; ++i)
      {
        Asset[] palettes = Archives[i].GetAssets(Asset.AssetType.Palette);
        ctrlPaletteSelector.Items.AddRange(palettes);
      }

      if (ctrlPaletteSelector.Items.Count > 0)
      {
        ctrlPaletteSelector.SelectedIndex = 0;
        ctrlPaletteSelector.Enabled = true;
      }
    }

    private void FillTreeView()
    {
      ctrlTreeView.Nodes.Clear();
      for (int archiveIndex = 0; archiveIndex < Archives.Count; ++archiveIndex)
      {
        string archiveName = Path.GetFileNameWithoutExtension(Archives[archiveIndex].LibraryPath);
        TreeNode archiveNode = new TreeNode(archiveName);

        MediaStorage.AssetInfo[] assets = Archives[archiveIndex].AssetsInfo;
        for (int i = 0; i < assets.Length; ++i)
        {
          TreeNode folderNode = CreateFolderNode(archiveNode, assets[i].Folder);
          TreeNode fileNode = folderNode.Nodes.Add(assets[i].FullFileName);

          Asset asset = Archives[archiveIndex].GetAsset(assets[i].Name);
          if (asset == null)
          {
            //Debug.Assert(false, "Cannot find an asset");
            continue;
          }

          fileNode.Tag = asset;
        }

        ctrlTreeView.Nodes.Add(archiveNode);
      }
    }

    private TreeNode CreateFolderNode(TreeNode root, string folder)
    {
      if (string.IsNullOrEmpty(folder))
        return root;

      char[] separators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
      TreeNode[] targets = root.Nodes.Find(folder, true);
      if (targets.Length == 0)
      {
        string[] folders = folder.Split(separators);

        TreeNode current = root;
        for (int i = 0; i < folders.Length; ++i)
        {
          if (string.IsNullOrEmpty(folders[i]))
            break;

          //FIX(adm244): hack
          string folderpath = string.Empty;
          for (int j = 0; j <= i; ++j)
            folderpath += folders[j] + "\\";

          current = current.Nodes.Add(folderpath, folders[i]);
        }
        return current;
      }

      return targets[0];
    }

    private void clearArchivesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Archives.Clear();
      ctrlTreeView.Nodes.Clear();
      
      clearArchivesToolStripMenuItem.Enabled = false;
      
      ctrlPaletteSelector.Items.Clear();
      ctrlPaletteSelector.Enabled = false;

      ctrlContentPanel.Controls.Clear();

      btnImport.Enabled = false;
      btnExport.Enabled = false;

      ctrlStatusLabel.Text = "Cleared.";
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void ctrlTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (!(e.Node.Tag is Asset))
      {
        Debug.Assert(false, "TreeNode is not associated with an Asset");
        return;
      }

      Asset asset = (Asset)e.Node.Tag;
      switch (asset.Type)
      {
        case Asset.AssetType.Sprite:
          DisplaySprite((Sprite)asset);
          break;

        default:
          Debug.Assert(false, "Asset type not implemented");
          break;
      }
    }

    private Palette GetSelectedPalette()
    {
      if ((ctrlPaletteSelector.Items.Count < 1) || (ctrlPaletteSelector.SelectedItem == null))
      {
        Debug.Assert(false, "No palettes to select from");
        return null;
      }

      if (!(ctrlPaletteSelector.SelectedItem is Palette))
      {
        Debug.Assert(false, "Selected palette is not implementing a Palette class");
        return null;
      }

      return (Palette)ctrlPaletteSelector.SelectedItem;
    }

    private void DisplaySprite(Sprite sprite)
    {
      Palette palette = GetSelectedPalette();
      Image image = sprite.CreateBitmap(palette);

      PictureBox pictureBox = new PictureBox();
      pictureBox.Dock = DockStyle.None;
      pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

      ctrlContentPanel.Controls.Clear();
      ctrlContentPanel.Controls.Add(pictureBox);

      btnImport.Enabled = true;
      btnExport.Enabled = true;

      pictureBox.Image = image;

      PreviewType = PreviewTypes.Image;
      PreviewObject = sprite;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      switch (PreviewType)
      {
        case PreviewTypes.Image:
          ImportImage();
          break;

        default:
          Debug.Assert(false, "Import type not implemented");
          break;
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      switch (PreviewType)
      {
        case PreviewTypes.Image:
          ExportImage();
          break;

        default:
          Debug.Assert(false, "Export type not implemented");
          break;
      }
    }

    private void ImportImage()
    {
      Debug.Assert(PreviewObject is Sprite);

      Sprite sprite = (Sprite)PreviewObject;

      ctrlOpenFile.Title = "Select image to import...";
      ctrlOpenFile.CheckFileExists = true;
      ctrlOpenFile.Multiselect = false;
      ctrlOpenFile.Filter = "Bitmap (*.bmp)|*.bmp";
      
      if (ctrlOpenFile.ShowDialog() == DialogResult.OK)
      {
        Bitmap bitmap = new Bitmap(ctrlOpenFile.FileName);
        if (bitmap == null)
        {
          MessageBox.Show(this, "Could not load an image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }

        if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
        {
          MessageBox.Show(this, "Bitmap is not in an 8-bit indexed format!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }

        Palette palette = GetSelectedPalette();
        if (!palette.Entries.AreEqual(bitmap.Palette.Entries))
        {
          if (MessageBox.Show(this, "Bitmap palette differs from currently selected. Continue to import?", "Palettes are not equal",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            return;
        }

        sprite.ChangeImage(bitmap);
        DisplaySprite(sprite);

        MessageBox.Show(this, "Image was successfully imported!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void ExportImage()
    {
      Debug.Assert(PreviewObject is Sprite);
      Debug.Assert(ctrlContentPanel.Controls.Count == 1);
      Debug.Assert(ctrlContentPanel.Controls[0] is PictureBox);

      Sprite sprite = (Sprite)PreviewObject;
      Image image = ((PictureBox)ctrlContentPanel.Controls[0]).Image;

      ctrlSaveFile.Title = "Select export destination";
      ctrlSaveFile.CheckFileExists = false;
      ctrlSaveFile.CheckPathExists = true;
      ctrlSaveFile.FileName = sprite.Name;
      ctrlSaveFile.Filter = "Bitmap (*.bmp)|*.bmp";

      if (ctrlSaveFile.ShowDialog() == DialogResult.OK)
      {
        image.Save(ctrlSaveFile.FileName, ImageFormat.Bmp);
        MessageBox.Show(this, "Image was successfully exported!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void ctrlPaletteSelector_SelectedValueChanged(object sender, EventArgs e)
    {
      switch (PreviewType)
      {
        case PreviewTypes.Image:
          {
            Debug.Assert(PreviewObject is Sprite);
            DisplaySprite((Sprite)PreviewObject);
          }
          break;

        default:
          Debug.Assert(false);
          break;
      }
    }

    private enum PreviewTypes
    {
      None = 0,
      Image,
      Text,
    }
  }
}
