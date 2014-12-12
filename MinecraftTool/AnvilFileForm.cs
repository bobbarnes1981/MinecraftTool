using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MinecraftLibrary;

namespace MinecraftTool
{
    public partial class AnvilFileForm : Form, IFileForm
    {
        private string m_path;

        private string m_tempPath;

        private bool m_dirty;

        private Region m_region;

        private ProgressForm m_progressForm;

        public AnvilFileForm(string path)
        {
            InitializeComponent();

            m_path = path;

            m_dirty = false;

            m_progressForm = new ProgressForm(m_path);

            setText();
        }

        private void setText()
        {
            Text = string.Format("Anvil File - {0}{1}", m_path, m_dirty ? "*" : "");
        }

        private void AnvilFileForm_Load(object sender, EventArgs e)
        {
            using (FileStream stream = new FileStream(m_path, FileMode.Open, FileAccess.Read))
            {
                AnvilStream anvil = new AnvilStream(stream);

                loadRegion(anvil);
            }
        }

        private void loadRegion(AnvilStream stream)
        {
            stream.ProgressChanged += m_progressForm.ProgressChangedEventHandler;

            new Thread(new ParameterizedThreadStart(loadRegionWorker)).Start(stream);

            m_progressForm.ShowDialog(this);
        }

        private void loadRegionWorker(object obj)
        {
            AnvilStream stream = (AnvilStream)obj;

            m_region = stream.Read();

            m_progressForm.Invoke((Action)delegate { m_progressForm.Close(); });

            treeView.Invoke((Action)delegate { updateTreeView(); });
        }

        private void updateTreeView()
        {
            for (int i = 0; i < m_region.Chunks.Length; i++)
            {
                TreeNode node = new TreeNode(string.Format("Chunk {0}", i));
                if (m_region.Chunks[i] != null)
                {
                    node.Nodes.addTagNodes(m_region.Chunks[i].Data);
                }
                treeView.Nodes.Add(node);
            }
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
            using (FileStream stream = new FileStream(m_tempPath, FileMode.CreateNew, FileAccess.Write))
            {
                AnvilStream anvil = new AnvilStream(stream);

                saveAnvil(anvil);
            }
        }

        private void saveAnvil(AnvilStream stream)
        {
            stream.ProgressChanged += m_progressForm.ProgressChangedEventHandler;

            new Thread(new ParameterizedThreadStart(saveAnvilWorker)).Start(stream);

            m_progressForm.ShowDialog(this);
        }

        private void saveAnvilWorker(object obj)
        {
            AnvilStream stream = (AnvilStream)obj;

            stream.Write(m_region);

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
