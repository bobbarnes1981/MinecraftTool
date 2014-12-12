using System;
using System.Windows.Forms;
using MinecraftLibrary;

namespace MinecraftTool
{
    public partial class EditForm : Form
    {
        private Tag m_tag;

        public EditForm(Tag tag)
        {
            InitializeComponent();

            m_tag = tag;

            Text = m_tag.Name;

            textBoxInput.Text = tag.Payload.ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (m_tag.ParsePayload(textBoxInput.Text))
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(this, "Invalid input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}
