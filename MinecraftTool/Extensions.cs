using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using MinecraftLibrary;

namespace MinecraftTool
{
    public static class Extensions
    {
        public static void addTagNodes(this TreeNodeCollection collection, Tag tag)
        {
            NBTTreeNode node = new NBTTreeNode(tag);
            switch (tag.Type)
            {
                case TagType.Compound:
                    foreach (Tag subTag in (List<Tag>)tag.Payload)
                    {
                        node.Nodes.addTagNodes(subTag);
                    }
                    break;
                case TagType.List:
                    foreach (object obj in (IList)tag.Payload)
                    {
                        if (obj.GetType() == typeof(List<Tag>))
                        {
                            TreeNode subNode = new TreeNode(string.Format("{0} - ", TagType.Compound));
                            subNode.Tag = obj;
                            foreach (Tag subTag in (List<Tag>)obj)
                            {
                                subNode.Nodes.addTagNodes(subTag);
                            }
                            node.Nodes.Add(subNode);
                        }
                        else
                        {
                            TreeNode subNode = new TreeNode(obj.ToString());
                            subNode.Tag = obj;
                            node.Nodes.Add(subNode);
                        }
                    }
                    break;
            }
            collection.Add(node);
        }
    }
}
