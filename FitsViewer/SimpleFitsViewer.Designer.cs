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
            this.FitsHeader = new System.Windows.Forms.ListView();
            this.OpenButton = new System.Windows.Forms.Button();
            this.FitsFileOpener = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.FitsImage)).BeginInit();
            this.SuspendLayout();
            // 
            // FitsImage
            // 
            this.FitsImage.Location = new System.Drawing.Point(12, 12);
            this.FitsImage.Name = "FitsImage";
            this.FitsImage.Size = new System.Drawing.Size(687, 502);
            this.FitsImage.TabIndex = 0;
            this.FitsImage.TabStop = false;
            // 
            // FitsHeader
            // 
            this.FitsHeader.Location = new System.Drawing.Point(705, 12);
            this.FitsHeader.Name = "FitsHeader";
            this.FitsHeader.Size = new System.Drawing.Size(395, 647);
            this.FitsHeader.TabIndex = 1;
            this.FitsHeader.UseCompatibleStateImageBehavior = false;
            // 
            // OpenButton
            // 
            this.OpenButton.Location = new System.Drawing.Point(172, 597);
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
            this.FitsFileOpener.Filter = "FITS files|*.fits|All files|*.*";
            this.FitsFileOpener.ReadOnlyChecked = true;
            this.FitsFileOpener.SupportMultiDottedExtensions = true;
            this.FitsFileOpener.Title = "Browse for FITS image file";
            // 
            // SimpleFitsViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1112, 671);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.FitsHeader);
            this.Controls.Add(this.FitsImage);
            this.Name = "SimpleFitsViewer";
            this.Text = "Simple Fits Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.FitsImage)).EndInit();
            this.ResumeLayout(false);

            }

        #endregion
        private System.Windows.Forms.PictureBox FitsImage;
        private System.Windows.Forms.ListView FitsHeader;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.OpenFileDialog FitsFileOpener;
        }
    }

