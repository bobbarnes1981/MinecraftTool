using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MinecraftLibrary;

namespace MinecraftTool
{
    public partial class NBTFileForm : Form, IFileForm
    {
        private string m_path;

        private string m_tempPath;

        private bool m_dirty;

        private Tag m_tag;

        private ProgressForm m_progressForm;

        public NBTFileForm(string path)
        {
            InitializeComponent();

            m_path = path;

            m_dirty = false;

            m_progressForm = new ProgressForm(m_path);

            setText();
        }

        private void setText()
        {
            Text = string.Format("NBT File - {0}{1}", m_path, m_dirty ? "*" : "");
        }

        private void NBTFileForm_Load(object sender, EventArgs e)
        {
            using (GZipStream stream = new GZipStream(new FileStream(m_path, FileMode.Open, FileAccess.Read), CompressionMode.Decompress))
            {
                NBTStream nbt = new NBTStream(stream);

                loadNBT(nbt);
            }
        }

        private void loadNBT(NBTStream stream)
        {
            stream.ProgressChanged += m_progressForm.ProgressChangedEventHandler;

            new Thread(new ParameterizedThreadStart(loadNBTWorker)).Start(stream);

            m_progressForm.ShowDialog(this);
        }

        private void loadNBTWorker(object obj)
        {
            NBTStream stream = (NBTStream)obj;

            m_tag = stream.Read();

            m_progressForm.Invoke((Action)delegate { m_progressForm.Close(); });

            treeView.Invoke((Action)delegate { updateTreeView(); });
        }

        private void updateTreeView()
        {
            treeView.Nodes.addTagNodes(m_tag);
        }

        public void Save()
        {
            SaveAs(m_path);
        }

        public void SaveAs(string path)
        {
            m_path = path;
            m_tempPath = string.Format("{0}.{1}", path, Guid.NewGuid());
            if (File.Exists(m_tempPath))
            {
                File.Delete(m_tempPath);
            }
            using (GZipStream stream = new GZipStream(new FileStream(m_tempPath, FileMode.CreateNew, FileAccess.Write), CompressionMode.Compress))
            {
                NBTStream nbt = new NBTStream(stream);

                saveNBT(nbt);
            }
        }

        private void saveNBT(NBTStream stream)
        {
            stream.ProgressChanged += m_progressForm.ProgressChangedEventHandler;

            new Thread(new ParameterizedThreadStart(saveNBTWorker)).Start(stream);

            m_progressForm.ShowDialog(this);
        }

        private void saveNBTWorker(object obj)
        {
            NBTStream stream = (NBTStream)obj;

            stream.Write(m_tag);

            m_progressForm.Invoke((Action)delegate { m_progressForm.Close(); });

            Invoke((Action)delegate { setText(); });

            if (File.Exists(m_path))
            {
                File.Delete(m_path);
            }
            File.Move(m_tempPath, m_path);

            m_dirty = false;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                editTreeViewNode(treeView.SelectedNode);
            }
        }

        private void editTreeViewNode(TreeNode node)
        {
            if (node is NBTTreeNode)
            {
                NBTTreeNode treeNode = (NBTTreeNode)node;
                if (!treeNode.NBTTag.IsCollection)
                {
                    if (new EditForm(treeNode.NBTTag).ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        m_dirty = true;
                        setText();
                    }
                }
            }
        }
    }
}
