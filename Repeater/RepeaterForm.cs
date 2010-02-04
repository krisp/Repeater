using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace Repeater
{
    public partial class RepeaterForm : Form
    {
        private Repeater repeater;

        public RepeaterForm(Repeater r)
        {
            InitializeComponent();
            this.repeater = r;
        }

        private void RepeaterForm_Load(object sender, EventArgs e)
        {
            foreach (String port in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(port);
                comboBox2.Items.Add(port);
            }

            comboBox1.Sorted = true;
            comboBox2.Sorted = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String source = (String)comboBox1.SelectedItem;
            String dest = (String)comboBox2.SelectedItem;

            try
            {
                if (repeater.AddSerialPairing(source, dest))
                {
                    listBox1.Items.Add(source + " <->  " + dest);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;
            
            String item = (String)listBox1.SelectedItem;
            int index = listBox1.SelectedIndex;

            String src = item.Substring(0, item.IndexOf(' '));
            String dst = item.Substring(item.IndexOf('>') + 3, item.Length - (item.IndexOf('>') + 3));

            try
            {
                if (repeater.RemoveSerialPairing(src, dst))
                {
                    listBox1.Items.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
