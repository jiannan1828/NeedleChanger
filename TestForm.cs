using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InjectorInspector
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            this.Text = "TestForm_L=123";
        }

        private void TestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Text = "TestForm_L=456";
        }
    }
}
