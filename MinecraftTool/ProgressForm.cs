using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MinecraftTool
{
    public partial class ProgressForm : Form
    {
        public ProgressForm(string title)
        {
            InitializeComponent();

            Text = title;
        }

        internal void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Invoke((Action)delegate { progressBar.Value = e.ProgressPercentage; });
        }
    }
}
