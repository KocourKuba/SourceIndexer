using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceIndexerNS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void PdbPathTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void PdbPath_Click(object sender, EventArgs e)
        {

        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            var indexer = CreateSourceIndexer();
            indexer.RunSourceIndexing();
            EvaluateResults(indexer);
        }

        private void EvaluateButton_Click(object sender, EventArgs e)
        {
            var indexer = CreateSourceIndexer();
            EvaluateResults(indexer);
        }

        private void EvaluateResults(SourceIndexer sourceIndexer)
        {
            this.EvaluatedRichTextBox.Text = this.NormalizeNewlines(sourceIndexer.EvaluateSourceIndexing());
            this.StreamRichTextBox.Text = this.NormalizeNewlines(sourceIndexer.GetSourceIndexingResults());
            this.UnindexedRichTextBox.Text = this.NormalizeNewlines(sourceIndexer.GetUnindexedList());
        }
        string NormalizeNewlines(string data)
        {
            return Regex.Replace(data, @"\r\n|\n\r|\n|\r", "\r\n");
        }

        private SourceIndexer CreateSourceIndexer()
        {
            SettingsBean Params = new SettingsBean
            {
                PdbFile = this.PdbPathTextBox.Text,
                SourcePath = this.SourceRootTextBox.Text,
                ToolsPath = this.SrcToolsPath.Text,
                SrcSrvIniPath = this.SrcSrvIniPath.Text,
                BackEndType = BackendComboBox.SelectedItem.ToString().ToUpper()
            };
            return new SourceIndexer(Params);
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            TextBox textBox = (TextBox)sender;
            textBox.Text = files[0];
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackendComboBox.Items.Add(new CmdBackEnd());
            this.BackendComboBox.Items.Add(new GitBackEnd());
            this.BackendComboBox.Items.Add(new GitHubBackEnd());
            this.BackendComboBox.SelectedIndex = 0;
        }

        private void SourceBrowseButton_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            this.folderBrowserDialog1.SelectedPath = this.SourceRootTextBox.Text;
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.SourceRootTextBox.Text = this.folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = this.folderBrowserDialog1.RootFolder;
            }
        }

        private void ToolsBrowseButton_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            this.folderBrowserDialog1.SelectedPath = this.SrcToolsPath.Text;
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.SrcToolsPath.Text = this.folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = this.folderBrowserDialog1.RootFolder;
            }
        }

        private void PdbBrowseDialog_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            this.openFileDialog1.FileName = this.PdbPathTextBox.Text;
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.PdbPathTextBox.Text = this.openFileDialog1.FileName;
            }
        }

        private void IniBrowseButton_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            this.folderBrowserDialog1.SelectedPath = this.SrcSrvIniPath.Text;
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.SrcSrvIniPath.Text = this.folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = this.folderBrowserDialog1.RootFolder;
            }
        }
    }
}
