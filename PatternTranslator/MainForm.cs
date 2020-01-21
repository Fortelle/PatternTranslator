using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PatternTranslator
{
    public partial class MainForm : Form
    {
        public List<PatternDictionary> Dictionaries;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var dictFolder = Path.Combine(Application.StartupPath, "Dictionaries");
            var files = Directory.GetFiles(dictFolder, "*.json", SearchOption.AllDirectories);

            Dictionaries = new List<PatternDictionary>();

            foreach (var file in files) {
                var json = File.ReadAllText(file);
                var name = Path.GetFileNameWithoutExtension(file);
                var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<PatternDictionary>(json);
                dict.Path = file;

                Dictionaries.Add(dict);
                var tsmi = new ToolStripMenuItem(name + "...")
                {
                    Tag = file,
                };
                tsmi.Click += tsmiDictionary_Click;
                tsmiDictionaries.DropDownItems.Add(tsmi);
            }
        }

        private void tsmiDictionary_Click(object sender, EventArgs e)
        {
            var path = (string)((ToolStripMenuItem)sender).Tag;
            var dict = Dictionaries.First(x => x.Path == path);
            using var frm = new DictionaryEditor(dict);
            var result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                Dictionaries.Remove(dict);
                Dictionaries.Add(frm.Dict);
            }
        }

        private void tsbTranslate_Click(object sender, EventArgs e)
        {
            var translator = new Translator(Dictionaries);
            var result = translator.Translate(richTextBox1.Text);

            if (result.Success)
            {
            }
            else
            {

            }

            richTextBox2.Text = result.Output;
            tssiTimes.Text = $"{result.TotalTimes.TotalMilliseconds.ToString("0")}ms";
        }

        private void tsmiDictionaries_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
