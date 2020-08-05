// This file is part of the TA.ObjectOrientedAstronomy project
//
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
//
// File: SimpleFitsViewer.cs  Last modified: 2016-10-07@03:51 by Tim Long

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;
using TA.Utils.Core;
using TA.Utils.Core.Diagnostics;
using TA.Utils.Logging.NLog;

namespace FitsViewer
    {
    public partial class SimpleFitsViewer : Form
        {
        private static readonly List<string> FitsFileExtensions = new List<string> {".FITS", ".FIT", ".FTS"};
        private readonly ILog log = new LoggingService();

        public SimpleFitsViewer()
            {
            InitializeComponent();
            }

        private async void OpenButton_Click(object sender, EventArgs e)
            {
            var result = FitsFileOpener.ShowDialog();
            if (result == DialogResult.OK)
                {
                var selectedFile = FitsFileOpener.FileName;
                await OpenFitsFile(selectedFile);
                }
            }

        private async Task OpenFitsFile(string fullyQualifiedFileName)
            {
            Cursor.Current = Cursors.WaitCursor;
            FitsHeaderDataUnit primaryHdu;
            using (var stream = new FileStream(fullyQualifiedFileName, FileMode.Open, FileAccess.Read))
                {
                var reader = new FitsReader(stream);
                primaryHdu = await reader.ReadPrimaryHeaderDataUnit().ConfigureAwait(true);
                }
            var image = primaryHdu.ToWindowsBitmap();
            FitsImage.Image = image;
            HeaderRecords.BeginUpdate();
            HeaderRecords.Items.Clear();
            HeaderRecords.Items.AddRange(primaryHdu.Header.HeaderRecords.Select(p => p.Text).ToArray());
            HeaderRecords.EndUpdate();
            Cursor.Current = Cursors.Default;
            }

        private void SimpleFitsViewer_DragDrop(object sender, DragEventArgs e)
            {
            var maybeFitsFile = DraggedFitsFile(e);
            log.Info().Message("Drag Drop: {file}", maybeFitsFile).Write();
            if (maybeFitsFile.Any())
                OpenFitsFile(maybeFitsFile.Single());   // Completes asynchronously
            }

        private void SimpleFitsViewer_DragEnter(object sender, DragEventArgs e)
            {
            var maybeFitsFile = DraggedFitsFile(e);
            log.Info().Message("Drag Enter: {file}", maybeFitsFile).Write();
            if (maybeFitsFile.Any())
                {
                e.Effect = DragDropEffects.Copy;
                }
            else
                {
                e.Effect = DragDropEffects.None;
                Cursor.Current = Cursors.No;
                }
            }

        private Maybe<string> DraggedFitsFile(DragEventArgs e)
            {
            Maybe<string> maybeFitsFile = Maybe<string>.Empty;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Any())
                    {
                    string droppedFile = files.First();
                    var extension = Path.GetExtension(droppedFile).ToUpper();
                    if (FitsFileExtensions.Contains(extension))
                        maybeFitsFile = droppedFile.AsMaybe();
                    }
                }
            log.Debug().Message("Drag & drop FITS file: {file}", maybeFitsFile).Write();
            return maybeFitsFile;
            }
        }
    }