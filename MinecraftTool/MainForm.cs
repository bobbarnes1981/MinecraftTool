using System;
using System.IO;
using System.Windows.Forms;

namespace MinecraftTool
{
    public partial class MainForm : Form
    {
        private OpenFileDialog m_openFileDialog;

        public MainForm()
        {
            InitializeComponent();

            m_openFileDialog = new OpenFileDialog();
        }

        private void addNewNBTFileForm(string path)
        {
            NBTFileForm form = new NBTFileForm(path);
            form.MdiParent = this;
            form.Show();
        }

        private void addNewAnvilFileForm(string path)
        {
            AnvilFileForm form = new AnvilFileForm(path);
            form.MdiParent = this;
            form.Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(m_openFileDialog.FileName);
                switch (fileInfo.Extension)
                {
                    case ".dat":
                        addNewNBTFileForm(m_openFileDialog.FileName);
                        break;
                    case ".mca":
                        addNewAnvilFileForm(m_openFileDialog.FileName);
                        break;
                    default:
                        MessageBox.Show(string.Format("Unhandled file extension: {0}", fileInfo.Extension));
                        break;
                }
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            Form activeChild = ActiveMdiChild;
            if (activeChild != null)
            {
                if (activeChild is NBTFileForm || activeChild is AnvilFileForm)
                {
                    saveToolStripMenuItem.Enabled = true;
                    saveAsToolStripMenuItem.Enabled = true;
                }
                else
                {
                    saveToolStripMenuItem.Enabled = false;
                    saveAsToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form activeChild = ActiveMdiChild;
            if (activeChild != null)
            {
                if (typeof(IFileForm).IsAssignableFrom(activeChild.GetType()))
                {
                    ((IFileForm)activeChild).Save();
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form activeChild = ActiveMdiChild;
            if (activeChild != null)
            {
                if (typeof(IFileForm).IsAssignableFrom(activeChild.GetType()))
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        ((IFileForm)activeChild).SaveAs(dialog.FileName);
                    }
                }
            }
        }
    }
}
