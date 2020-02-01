using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PatternTranslator
{
    public partial class PerformanceForm : Form
    {
        public PerformanceForm()
        {
            InitializeComponent();
        }

        public PerformanceForm(TranslateTestResult[] results) : this()
        {
            listView1.BeginUpdate();
            foreach(var result in results)
            {
                var row = listView1.Items.Add(result.Key);
                row.SubItems.Add(result.Time.ToString());
                row.SubItems.Add(result.Count.ToString());
            }
            listView1.EndUpdate();
        }

        private void Performance_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("Key");
            listView1.Columns.Add("Time");
            listView1.Columns.Add("Count");
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
        }
    }

}
