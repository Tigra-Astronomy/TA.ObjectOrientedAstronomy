// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: SimpleFitsViewer.cs  Last modified: 2016-10-04@01:00 by Tim Long

using System;
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
                using (var stream = FitsFileOpener.OpenFile())
                    {
                    var reader = new FitsReader(stream);
                    var primaryHdu = await reader.ReadPrimaryHeaderDataUnit().ConfigureAwait(true);
                    var image = primaryHdu.ToWindowsBitmap();
                    FitsImage.Image = image;
                    //ToDo - populate headers
                    }
                }
            }
        }
    }