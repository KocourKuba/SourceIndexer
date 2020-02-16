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
            EvaluatedRichTextBox.Text = NormalizeNewlines(sourceIndexer.EvaluateSourceIndexing());
            StreamRichTextBox.Text = NormalizeNewlines(sourceIndexer.GetSourceIndexingResults());
            UnindexedRichTextBox.Text = NormalizeNewlines(sourceIndexer.GetUnindexedList());
        }
        string NormalizeNewlines(string data)
        {
            return Regex.Replace(data, @"\r\n|\n\r|\n|\r", "\r\n");
        }

        private SourceIndexer CreateSourceIndexer()
        {
            SettingsBean Params = new SettingsBean
            {
                PdbFile = PdbPathTextBox.Text,
                SourcePath = SourceRootTextBox.Text,
                ToolsPath = SrcToolsPath.Text,
                SrcSrvIniPath = SrcSrvIniPath.Text,
                BackEndType = BackendComboBox.SelectedItem.ToString().ToUpper(),
                CustomCommand = customCommandText.Text
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
            BackendComboBox.Items.Add(new CmdBackEnd());
            BackendComboBox.Items.Add(new GitBackEnd());
            BackendComboBox.Items.Add(new GitHubBackEnd());
            BackendComboBox.SelectedIndex = 0;
        }

        private void SourceBrowseButton_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            folderBrowserDialog1.SelectedPath = SourceRootTextBox.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                SourceRootTextBox.Text = folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = folderBrowserDialog1.RootFolder;
            }
        }

        private void ToolsBrowseButton_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            folderBrowserDialog1.SelectedPath = SrcToolsPath.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                SrcToolsPath.Text = folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = folderBrowserDialog1.RootFolder;
            }
        }

        private void PdbBrowseDialog_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            openFileDialog1.FileName = PdbPathTextBox.Text;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                PdbPathTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void IniBrowseButton_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            folderBrowserDialog1.SelectedPath = SrcSrvIniPath.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                SrcSrvIniPath.Text = folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = folderBrowserDialog1.RootFolder;
            }
        }

        private void BackendComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(BackendComboBox.SelectedIndex != 0)
            {
                customCommandText.Visible = false;
            }
        }
    }
}
