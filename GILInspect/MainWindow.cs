using Gurosi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GILInspect
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            
        }

        private void MenuOpenLibrary_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Library File|*.grsl";
            dialog.Title = "Open library";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(dialog.FileName))
                {
                    try
                    {
                        Library lib = Library.Load(dialog.FileName);

                        DisplayBox.Text = GILFormatter.Format(lib);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error loading the library.", "GILInspect");
                    }
                }
            }
        }

        private void MenuOpenRuntime_Click(object sender, EventArgs e)
        {

        }
    }
}
