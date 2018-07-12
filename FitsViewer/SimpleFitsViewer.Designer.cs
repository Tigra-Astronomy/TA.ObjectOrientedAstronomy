namespace FitsViewer
    {
    partial class SimpleFitsViewer
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
            {
            if (disposing && (components != null))
                {
                components.Dispose();
                }
            base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            this.FitsImage = new System.Windows.Forms.PictureBox();
            this.OpenButton = new System.Windows.Forms.Button();
            this.FitsFileOpener = new System.Windows.Forms.OpenFileDialog();
            this.HeaderRecords = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.FitsImage)).BeginInit();
            this.SuspendLayout();
            // 
            // FitsImage
            // 
            this.FitsImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FitsImage.Location = new System.Drawing.Point(12, 12);
            this.FitsImage.Name = "FitsImage";
            this.FitsImage.Size = new System.Drawing.Size(687, 618);
            this.FitsImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.FitsImage.TabIndex = 0;
            this.FitsImage.TabStop = false;
            // 
            // OpenButton
            // 
            this.OpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OpenButton.Location = new System.Drawing.Point(12, 636);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(75, 23);
            this.OpenButton.TabIndex = 2;
            this.OpenButton.Text = "Open...";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // FitsFileOpener
            // 
            this.FitsFileOpener.DefaultExt = "fits";
            this.FitsFileOpener.Filter = "FITS files|*.fits;*.fts;*.fit|All files|*.*";
            this.FitsFileOpener.ReadOnlyChecked = true;
            this.FitsFileOpener.SupportMultiDottedExtensions = true;
            this.FitsFileOpener.Title = "Browse for FITS image file";
            // 
            // HeaderRecords
            // 
            this.HeaderRecords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HeaderRecords.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeaderRecords.FormattingEnabled = true;
            this.HeaderRecords.Location = new System.Drawing.Point(705, 12);
            this.HeaderRecords.Name = "HeaderRecords";
            this.HeaderRecords.Size = new System.Drawing.Size(504, 654);
            this.HeaderRecords.TabIndex = 3;
            // 
            // SimpleFitsViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1221, 671);
            this.Controls.Add(this.HeaderRecords);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.FitsImage);
            this.Name = "SimpleFitsViewer";
            this.Text = "Simple Fits Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.FitsImage)).EndInit();
            this.ResumeLayout(false);

            }

        #endregion
        private System.Windows.Forms.PictureBox FitsImage;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.OpenFileDialog FitsFileOpener;
        private System.Windows.Forms.ListBox HeaderRecords;
        }
    }

