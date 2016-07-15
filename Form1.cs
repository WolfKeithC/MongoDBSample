using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MongoDBTester
{
    public partial class Form1 : Form
    {
        private MongoDBHelper helper;
        public Form1()
        {
            InitializeComponent();
            helper = new MongoDBHelper();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "IsConnected=" + helper.IsConnected.ToString();

            List<string> dbs = new List<string>();
            try
            {
                dbs = await helper.Databases();
            }
            catch (System.TimeoutException ext)
            {
                MessageBox.Show(ext.Message, this.Text);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text);
            }
            
            if (dbs.Count() > 0)
                listBox1.Items.AddRange(dbs.ToArray());
            
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string dbName = listBox1.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(dbName))
            {
                this.helper.SetDB(dbName);
                List<string> items = new List<string>();
                try
                {
                    items = await helper.Search("mycol");
                }
                catch (System.TimeoutException ext)
                {
                    MessageBox.Show(ext.Message, this.Text);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text);
                }
                if (items.Count() > 0)
                    listBox2.Items.AddRange(items.ToArray());
            }
        }
    }
}
