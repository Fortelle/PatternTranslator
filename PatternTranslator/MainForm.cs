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
        public string DictionaryFolder => Path.Combine(Application.StartupPath, "Dictionaries");

        public MainForm()
        {
            InitializeComponent();
        }

        private void AddDictionary(PatternDictionary dict)
        {
            Dictionaries.Add(dict);
            var tsmi = new ToolStripMenuItem(dict.Name + "...")
            {
                Tag = dict.Path,
            };
            tsmi.Click += tsmiDictionary_Click;
            tsmiDictionaries.DropDownItems.Add(tsmi);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(DictionaryFolder, "*.json", SearchOption.AllDirectories);

            Dictionaries = new List<PatternDictionary>();

            foreach (var file in files) {
                var json = File.ReadAllText(file);
                var name = Path.GetFileNameWithoutExtension(file);
                var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<PatternDictionary>(json);
                dict.Name = name;
                dict.Path = file;
                AddDictionary(dict);
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
            tssiTimes.Text = $"{result.TotalTimes.TotalMilliseconds:0}ms";
        }

        private void tsmiDictionaries_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void performanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var translator = new Translator(Dictionaries);
            var results = translator.Test(richTextBox1.Text);
            using var frm = new PerformanceForm(results);
            frm.ShowDialog();
        }

        private void fromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    var filename = openFileDialog1.FileName;

            //}
        }

        private void fromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var text = Clipboard.GetText();
            var lines = Regex.Split(text, @"\r?\n|\r");
            var entries = lines
                    .Select(x => x.Split("\t"[0]))
                    .Where(x => x.Length >= 2)
                    .GroupBy(x => x[0])
                    .Select(x => x.First())
                    .ToDictionary(x => x[0], x => x[1])
                    ;

            if (entries.Count == 0)
            {
                MessageBox.Show("Not supported format.");
                return;
            }

            var sb = new StringBuilder();
            foreach( var kv in entries.Take(Math.Min(lines.Length, 5)))
            {
                sb.AppendLine($"{kv.Key}\t{kv.Value}");
            }
            if (entries.Count > 5)
            {
                sb.AppendLine("...");
            }
            if (MessageBox.Show(sb.ToString(),"Preview", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            var dict = new PatternDictionary
            {
                Enable = true,
                Entries = entries,
            };
            saveFileDialog1.InitialDirectory = DictionaryFolder;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                dict.Path = saveFileDialog1.FileName;
                dict.Name = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName);
                AddDictionary(dict);
            }

            using var frm = new DictionaryEditor(dict);
            var result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                Dictionaries.Remove(dict);
                Dictionaries.Add(frm.Dict);
            }

        }
    }
}
