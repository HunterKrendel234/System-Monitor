namespace SysMonitor
{
    partial class SettingsForm
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
            lblLanguage = new Label();
            lblHotkey = new Label();
            lblModifier = new Label();
            cbLanguage = new ComboBox();
            cbModifier = new ComboBox();
            cbHotkey = new ComboBox();
            btnStartup = new Button();
            btnOK = new Button();
            btnCancel = new Button();
            groupOverlayPosition = new GroupBox();
            lblOverlayX = new Label();
            lblOverlayY = new Label();
            tbOverlayX = new TextBox();
            tbOverlayY = new TextBox();
            groupOverlayPosition.SuspendLayout();
            SuspendLayout();
            // 
            // lblLanguage
            // 
            lblLanguage.Location = new Point(12, 36);
            lblLanguage.Name = "lblLanguage";
            lblLanguage.Size = new Size(100, 20);
            lblLanguage.TabIndex = 10;
            lblLanguage.Text = "Язык:";
            // 
            // lblHotkey
            // 
            lblHotkey.Location = new Point(224, 71);
            lblHotkey.Name = "lblHotkey";
            lblHotkey.Size = new Size(119, 20);
            lblHotkey.TabIndex = 11;
            lblHotkey.Text = "Горячая клавиша:";
            // 
            // lblModifier
            // 
            lblModifier.Location = new Point(12, 71);
            lblModifier.Name = "lblModifier";
            lblModifier.Size = new Size(100, 20);
            lblModifier.TabIndex = 12;
            lblModifier.Text = "Модификатор:";
            // 
            // cbLanguage
            // 
            cbLanguage.FormattingEnabled = true;
            cbLanguage.Location = new Point(118, 33);
            cbLanguage.Name = "cbLanguage";
            cbLanguage.Size = new Size(100, 23);
            cbLanguage.TabIndex = 0;
            // 
            // cbModifier
            // 
            cbModifier.FormattingEnabled = true;
            cbModifier.Location = new Point(118, 68);
            cbModifier.Name = "cbModifier";
            cbModifier.Size = new Size(100, 23);
            cbModifier.TabIndex = 1;
            // 
            // cbHotkey
            // 
            cbHotkey.FormattingEnabled = true;
            cbHotkey.Location = new Point(349, 68);
            cbHotkey.Name = "cbHotkey";
            cbHotkey.Size = new Size(100, 23);
            cbHotkey.TabIndex = 2;
            // 
            // btnStartup
            // 
            btnStartup.Location = new Point(12, 213);
            btnStartup.Name = "btnStartup";
            btnStartup.Size = new Size(437, 30);
            btnStartup.TabIndex = 3;
            btnStartup.Text = "Автозапуск";
            btnStartup.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(243, 249);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(100, 30);
            btnOK.TabIndex = 4;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(349, 249);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 30);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupOverlayPosition
            // 
            groupOverlayPosition.Controls.Add(lblOverlayX);
            groupOverlayPosition.Controls.Add(lblOverlayY);
            groupOverlayPosition.Controls.Add(tbOverlayX);
            groupOverlayPosition.Controls.Add(tbOverlayY);
            groupOverlayPosition.Location = new Point(12, 109);
            groupOverlayPosition.Name = "groupOverlayPosition";
            groupOverlayPosition.Size = new Size(437, 86);
            groupOverlayPosition.TabIndex = 20;
            groupOverlayPosition.TabStop = false;
            groupOverlayPosition.Text = "Положение оверлея";
            // 
            // lblOverlayX
            // 
            lblOverlayX.Location = new Point(10, 25);
            lblOverlayX.Name = "lblOverlayX";
            lblOverlayX.Size = new Size(196, 20);
            lblOverlayX.TabIndex = 21;
            lblOverlayX.Text = "X:";
            // 
            // lblOverlayY
            // 
            lblOverlayY.Location = new Point(10, 45);
            lblOverlayY.Name = "lblOverlayY";
            lblOverlayY.Size = new Size(196, 20);
            lblOverlayY.TabIndex = 23;
            lblOverlayY.Text = "Y:";
            // 
            // tbOverlayX
            // 
            tbOverlayX.Location = new Point(212, 22);
            tbOverlayX.Name = "tbOverlayX";
            tbOverlayX.Size = new Size(60, 23);
            tbOverlayX.TabIndex = 22;
            // 
            // tbOverlayY
            // 
            tbOverlayY.Location = new Point(212, 42);
            tbOverlayY.Name = "tbOverlayY";
            tbOverlayY.Size = new Size(60, 23);
            tbOverlayY.TabIndex = 24;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(461, 300);
            Controls.Add(lblLanguage);
            Controls.Add(cbLanguage);
            Controls.Add(lblHotkey);
            Controls.Add(lblModifier);
            Controls.Add(cbModifier);
            Controls.Add(cbHotkey);
            Controls.Add(btnStartup);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(groupOverlayPosition);
            Name = "SettingsForm";
            Text = "SettingsForm";
            groupOverlayPosition.ResumeLayout(false);
            groupOverlayPosition.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        public System.Windows.Forms.Label lblLanguage;
        public System.Windows.Forms.Label lblHotkey;
        public System.Windows.Forms.Label lblModifier;
        public System.Windows.Forms.ComboBox cbLanguage;
        public System.Windows.Forms.ComboBox cbModifier;
        public System.Windows.Forms.ComboBox cbHotkey;
        public System.Windows.Forms.Button btnStartup;
        public System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.GroupBox groupOverlayPosition;
        public System.Windows.Forms.Label lblOverlayX;
        public System.Windows.Forms.Label lblOverlayY;
        public System.Windows.Forms.TextBox tbOverlayX;
        public System.Windows.Forms.TextBox tbOverlayY;
    }
}