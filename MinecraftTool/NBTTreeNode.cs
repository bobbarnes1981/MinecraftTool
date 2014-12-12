using System;
using System.Windows.Forms;
using MinecraftLibrary;

namespace MinecraftTool
{
    public class NBTTreeNode : TreeNode
    {
        private Tag m_nbtTag;

        public NBTTreeNode(Tag nbtTag)
        {
            m_nbtTag = nbtTag;
            m_nbtTag.PayloadChanged += tagPayloadChanged;
            setText();
        }

        private void tagPayloadChanged(object sender, EventArgs e)
        {
            setText();
        }

        public Tag NBTTag
        {
            get
            {
                return m_nbtTag;
            }
        }

        private void setText()
        {
            Text = m_nbtTag.ToString();
        }
    }
}
