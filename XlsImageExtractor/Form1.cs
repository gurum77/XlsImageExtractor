using Ionic.Zip;
using Manina.Windows.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;
using static Manina.Windows.Forms.ImageListView;
using Manina.Windows.Forms.ImageListViewRenderers;

namespace XlsImageExtractor
{
    public partial class Form1 : Form
    {
        long totalBytes = 0;
        long totalTransferredBytes = 0;


        public Form1()
        {
            InitializeComponent();

            // 이걸 해줘야 광고가 나옴
            InternetExplorerBrowserEmulation.SetBrowserEmulationVersion();

            Translate();
        }

        void Translate()
        {
            this.Text = LanguageHelper.Tr("XLS Image Extractor");
            toolStripButtonOpen.Text = LanguageHelper.Tr("Open");
            toolStripButtonSave.Text = LanguageHelper.Tr("Save");
            toolStripLabelThumbnailSize.Text = LanguageHelper.Tr("Thumbnail Size");
            toolStripLabelStyle.Text = LanguageHelper.Tr("Style");
            toolStripLabelView.Text = LanguageHelper.Tr("View");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            int panel2Size = 310;
            //int panel2Size = 400;

            splitContainer1.Panel2MinSize = panel2Size;
            splitContainer1.SplitterDistance = splitContainer1.Size.Width - panel2Size;
            splitContainer1.FixedPanel = FixedPanel.Panel2;

            var uri = new Uri("https://hileejaeho.cafe24.com/xlsimageextractor-start-screen/");
            webBrowser1.Navigate(uri);
            
            InitSizeCombo();
            InitRendererCombo();

            SetRenderer();

            imageListView1.SelectionChanged += ImageListView1_SelectionChanged;
        }

        // 이미지 선택시 status 바 갱신
        private void ImageListView1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateMenu();
            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            toolStripStatusLabelSelectionInfo.Text = $"{imageListView1.SelectedItems.Count} " + LanguageHelper.Tr("selected");   
        }

        void UpdateMenu()
        {
            toolStripButtonSave.Enabled = imageListView1.SelectedItems.Count > 0;
        }

        private void InitRendererCombo()
        {
            toolStripComboBoxRenderer.Items.Clear();
            toolStripComboBoxRenderer.Items.Add(LanguageHelper.Tr("Default"));
            toolStripComboBoxRenderer.Items.Add(LanguageHelper.Tr("XP"));
            toolStripComboBoxRenderer.Items.Add(LanguageHelper.Tr("Tile"));
            toolStripComboBoxRenderer.Items.Add(LanguageHelper.Tr("Zooming"));
            toolStripComboBoxRenderer.Items.Add(LanguageHelper.Tr("Noir"));

            toolStripComboBoxRenderer.SelectedIndex = (int)Options.renderer;
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
                    System.IO.File.Copy(item.FileName, targetFileName);
                }

                MessageBox.Show(LanguageHelper.Tr("Completed!"));
            }
            else
            {
                MessageBox.Show(LanguageHelper.Tr("Cancelled!"));
            }

        }

        private void toolStripComboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int size = (int)toolStripComboBoxSize.SelectedItem;

            imageListView1.ThumbnailSize = new Size(size, size);
            Options.thumbnailSize = size;
        }

        private void toolStripComboBoxRenderer_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.renderer = (Options.Renderer)toolStripComboBoxRenderer.SelectedIndex;
            SetRenderer();
        }

        private void SetRenderer()
        {
            ImageListViewRenderer renderer = null;
            if (Options.renderer == Options.Renderer.defaultRenderer)
                renderer = new DefaultRenderer();
            else if (Options.renderer == Options.Renderer.xPRenderer)
                renderer = new XPRenderer();
            else if (Options.renderer == Options.Renderer.tileRenderer)
                renderer = new TilesRenderer();
            else if (Options.renderer == Options.Renderer.zoomingRenderer)
                renderer = new ZoomingRenderer();
            else if (Options.renderer == Options.Renderer.noirRenderer)
                renderer = new NoirRenderer();

            if (renderer == null)
                return;
            imageListView1.SetRenderer(renderer);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            toolStripButtonOpen_Click(null, null);
        }
        private void thumbnailsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = Manina.Windows.Forms.View.Thumbnails;
        }

        private void galleryToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = Manina.Windows.Forms.View.Gallery;
        }

        private void paneToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = Manina.Windows.Forms.View.Pane;
        }

        private void detailsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = Manina.Windows.Forms.View.Details;
            if (imageListView1.Columns.Count == 0)
            {
                imageListView1.Columns.Add(ColumnType.Name);
                imageListView1.Columns.Add(ColumnType.Dimensions);
                imageListView1.Columns.Add(ColumnType.FileSize);
                imageListView1.Columns.Add(ColumnType.FolderName);
                imageListView1.Columns.Add(ColumnType.DateModified);
                imageListView1.Columns.Add(ColumnType.FileType);
            }
        }

        private void toolStripButtonKorean_Click(object sender, EventArgs e)
        {
            Options.language = "ko-KR";
            LanguageHelper.Load(Options.language);
            Translate();
            InitRendererCombo();
        }

        private void toolStripButtonEnglish_Click(object sender, EventArgs e)
        {
            Options.language = "en-US";
            LanguageHelper.Load(Options.language);
            Translate();
            InitRendererCombo();
        }
    }
}
