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
    public partial class DictionaryEditor : Form
    {
        public PatternDictionary Dict { get; set; }
        private bool Changed { get; set; }

        public DictionaryEditor()
        {
            InitializeComponent();
        }

        public DictionaryEditor(PatternDictionary dict) : this()
        {
            Dict = dict;
            chkEnable.Checked = dict.Enable;
            chkRegex.Checked = dict.Regex;
            chkIgnoreCase.Checked = dict.IgnoreCase;

            BindingList<Pair> pairs = new BindingList<Pair>();
            foreach(var kv in dict.Entries)
            {
                pairs.Add(new Pair() {
                    Key = kv.Key,
                    Value = kv.Value,
                    Parent = pairs,
                    IsRegex = dict.Regex,
                });
            }

            pairs.AddingNew += (s, a) =>
            {
                a.NewObject = new Pair()
                {
                    Key = "",
                    Value = "",
                    Parent = pairs,
                    IsRegex = dict.Regex,
                };
            };

            dataGridView1.DataSource = pairs;
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        public new DialogResult ShowDialog()
        {

            return base.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void Save()
        {
            var dict = CreateDictionary(true);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(dict.Path, json);
            Dict = dict;
        }

        private PatternDictionary CreateDictionary(bool withError)
        {
            var dict = (PatternDictionary)Dict.Clone();
            IEnumerable<Pair> entries = ((BindingList<Pair>)dataGridView1.DataSource);

            if (!withError)
            {
                entries = entries.Where(x => !x.IsError);
            }

            dict.Entries = entries.ToDictionary(x => x.Key, x => x.Value);
            dict.Enable = chkEnable.Checked;
            dict.Regex = chkRegex.Checked;
            dict.IgnoreCase = chkIgnoreCase.Checked;
            return dict;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var pair = ((BindingList<Pair>)dataGridView1.DataSource)[e.RowIndex];
            pair.Check(dataGridView1.Columns[e.ColumnIndex].Name);

            Changed = true;
            if (richTextBox1.TextLength > 0)
            {
                var dict = CreateDictionary(false);
                dict.Enable = true;
                var translator = new Translator(new List<PatternDictionary>() { 
                    dict,
                });
                var result = translator.Translate(richTextBox1.Text);
                richTextBox2.Text = result.Output;
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Changed)
            {
                var ask = MessageBox.Show("Save?", "", MessageBoxButtons.YesNoCancel);
                if (ask == DialogResult.Yes)
                {
                    Save();
                    this.DialogResult = DialogResult.OK;
                }
                else if (ask == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private class Pair : IDataErrorInfo
        {
            internal IList<Pair> Parent { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            internal bool IsRegex { get; set; }

            public void Check(string columnName)
            {
                KeyError = string.Empty;
                ValueError = string.Empty;

                if (columnName == "Key")
                {
                    if (string.IsNullOrEmpty(Key))
                    {
                        KeyError = "Empty pattern";
                    }

                    if (Parent != null && Parent.Any(x => x.Key == this.Key && !ReferenceEquals(x, this)))
                    {
                        KeyError = "Duplicate key";
                    }

                    if (IsRegex)
                    {
                        try
                        {
                            _ = new Regex(Key);
                        }
                        catch (ArgumentException e)
                        {
                            KeyError = e.Message;
                        }
                    }
                }
            }

            private string KeyError { get; set; }
            private string ValueError { get; set; }
            internal bool IsError => !string.IsNullOrEmpty(KeyError) || !string.IsNullOrEmpty(ValueError);

            string IDataErrorInfo.Error => "";

            string IDataErrorInfo.this[string columnName] => columnName switch
            {
                "Key" => KeyError,
                "Value" => ValueError,
                _ => ""
            };

        }

    }
}
