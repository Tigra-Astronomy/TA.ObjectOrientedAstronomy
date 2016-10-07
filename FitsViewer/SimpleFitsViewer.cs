// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: SimpleFitsViewer.cs  Last modified: 2016-10-07@03:51 by Tim Long

using System;
using System.Linq;
using System.Windows.Forms;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;

namespace FitsViewer
    {
    public partial class SimpleFitsViewer : Form
        {
        public SimpleFitsViewer()
            {
            InitializeComponent();
            }

        private async void OpenButton_Click(object sender, EventArgs e)
            {
            var result = FitsFileOpener.ShowDialog();
            if (result == DialogResult.OK)
                {
                FitsHeaderDataUnit primaryHdu;
                using (var stream = FitsFileOpener.OpenFile())
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
                }
            }
        }
    }