using Ionic.Zip;
using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XlsImageExtractor
{
    public partial class Form1 : Form
    {
        long totalBytes = 0;
        long totalTransferredBytes = 0;
        enum Renderer
        {
            defaultRenderer,
            xPRenderer,
            tileRenderer,

        }
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitSizeCombo();
            InitRendererCombo();
        }

        private void InitRendererCombo()
        {
            toolStripComboBoxRenderer.Items.Add("Default");
        }

        private void InitSizeCombo()
        {
            toolStripComboBoxSize.Items.Add(16);
            toolStripComboBoxSize.Items.Add(24);
            toolStripComboBoxSize.Items.Add(32);
            toolStripComboBoxSize.Items.Add(48);
            toolStripComboBoxSize.Items.Add(64);
            toolStripComboBoxSize.Items.Add(96);
            toolStripComboBoxSize.Items.Add(128);
            toolStripComboBoxSize.Items.Add(192);
            toolStripComboBoxSize.Items.Add(256);
            toolStripComboBoxSize.Items.Add(384);
            toolStripComboBoxSize.Items.Add(512);

            toolStripComboBoxSize.SelectedItem = Options.thumbnailSize;
        }

        // 열기
        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel files|*.xlsx";
            dlg.DefaultExt = "xlsx";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var targetPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(dlg.FileName));
                ExtractZipContent(dlg.FileName, "", targetPath);

                var mediaPath = System.IO.Path.Combine(targetPath, "xl", "media");
                var files = System.IO.Directory.GetFiles(mediaPath);
                imageListView1.Items.Clear();
                foreach (var file in files)
                {
                    var item = new ImageListViewItem(file);
                    imageListView1.Items.Add(item);
                }
            }
        }

        private void DotNetZip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            long percent = 0;
            if (totalBytes > 0)
            {
                if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
                    totalTransferredBytes += e.CurrentEntry.UncompressedSize;
                else if (e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
                    percent = (long)(((totalTransferredBytes + e.BytesTransferred) * 100) / totalBytes);
                else if (e.EventType == ZipProgressEventType.Saving_Completed)
                    percent = 100;
                else if (e.EventType == ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite)
                    e.CurrentEntry.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
            }
        }

        /// <summary>
        /// Extracts the content from a .zip file inside an specific folder.
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="password"></param>
        /// <param name="outputFolder"></param>
        public void ExtractZipContent(string zipFilePath, string password, string outputFolder)
        {
            try
            {
                totalBytes = 0;
                totalTransferredBytes = 0;
                ZipFile zip = new ZipFile(zipFilePath);
                zip.AlternateEncodingUsage = ZipOption.Always;
                zip.AlternateEncoding = System.Text.Encoding.UTF8;

                zip.Password = string.IsNullOrEmpty(password) ? null : password;
                zip.ExtractProgress += DotNetZip_ExtractProgress;
                foreach (var ent in zip.Entries)
                    totalBytes += ent.UncompressedSize;
                zip.ExtractAll(outputFolder, ExtractExistingFileAction.InvokeExtractProgressEvent);
            }
            catch
            {
            }
        }

        // 선택한 파일 저장
        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (var item in imageListView1.SelectedItems)
                {
                    var fileNameOnly = System.IO.Path.GetFileName(item.FileName);
                    var targetFileName = System.IO.Path.Combine(dlg.SelectedPath, fileNameOnly);
                    System.IO.File.Copy(item .FileName, targetFileName);
                }

                MessageBox.Show("Completed!");
            }
            else
            {
                MessageBox.Show("Cancelled!");
            }

        }

        private void toolStripComboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int size = (int)toolStripComboBoxSize.SelectedItem;
            
            imageListView1.ThumbnailSize = new Size(size, size);
            Options.thumbnailSize = size;
        }
    }
}
