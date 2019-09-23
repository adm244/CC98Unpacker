using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CropCirclesUnpacker.Storages;

namespace CropCirclesUnpackerGUI
{
  public partial class MainForm : Form
  {
    private List<MediaStorage> Archives;

    public MainForm()
    {
      InitializeComponent();

      Archives = new List<MediaStorage>();
      clearArchivesToolStripMenuItem.Enabled = false;
      extractToToolStripMenuItem.Enabled = false;

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
        ctrlStatusLabel.Text = "Loading...";

        //TODO(adm244): put into a separate thread
        string[] files = ctrlOpenFile.FileNames;
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
      }
    }

    private void FillTreeView()
    {
      ctrlTreeView.Nodes.Clear();
      for (int archiveIndex = 0; archiveIndex < Archives.Count; ++archiveIndex)
      {
        string archiveName = Path.GetFileNameWithoutExtension(Archives[archiveIndex].LibraryPath);
        TreeNode archiveNode = new TreeNode(archiveName);

        MediaStorage.Asset[] assets = Archives[archiveIndex].Contents;
        for (int i = 0; i < assets.Length; ++i)
        {
          TreeNode folderNode = CreateFolderNode(archiveNode, assets[i].Folder);
          folderNode.Nodes.Add(assets[i].FullFileName);
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
      ctrlStatusLabel.Text = "Cleared.";
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }
  }
}
