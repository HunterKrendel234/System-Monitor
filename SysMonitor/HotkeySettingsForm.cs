using System;
using System.Windows.Forms;
using System.IO;

namespace MonitorTray
{
    public delegate string Translator(string key);

    public class HotkeySettingsForm : Form
    {
        private ComboBox cbKey;
        private ComboBox cbModifier;
        private Button btnOK;
        private Button btnCancel;

        public string SelectedKey { get; private set; }
        public string SelectedModifier { get; private set; }

        private Translator T;

        public HotkeySettingsForm(string currentKey, string currentModifier, Translator translator, string currentLanguage)
        {
            T = translator;
            this.Width = currentLanguage == "ru" ? 350 : 300;
            this.Height = 150;
            this.Text = T("hotkey_settings_form_title");
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "style", "monitor.ico"));

            Label lblKey = new Label() { Text = T("key_label"), Left = 10, Top = 10, Width = 100 };
            cbKey = new ComboBox() { Left = 110, Top = 10, Width = this.Width - 140, DropDownStyle = ComboBoxStyle.DropDownList };
            for (int i = 1; i <= 12; i++)
            {
                cbKey.Items.Add("F" + i);
            }
            cbKey.SelectedItem = currentKey;

            Label lblModifier = new Label() { Text = T("modifier_label"), Left = 10, Top = 40, Width = 100 };
            cbModifier = new ComboBox() { Left = 110, Top = 40, Width = this.Width - 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cbModifier.Items.AddRange(new string[] { T("modifier_none"), T("modifier_shift"), T("modifier_alt"), T("modifier_control"), T("modifier_win") });
            cbModifier.SelectedItem = currentModifier;

            btnOK = new Button() { Text = T("ok"), Left = 110, Width = 75, Top = 80, DialogResult = DialogResult.OK };
            btnCancel = new Button() { Text = T("cancel"), Left = 190, Width = 75, Top = 80, DialogResult = DialogResult.Cancel };

            btnOK.Click += (s, e) =>
            {
                SelectedKey = cbKey.SelectedItem.ToString();
                SelectedModifier = cbModifier.SelectedItem.ToString();
                this.Close();
            };
            btnCancel.Click += (s, e) => { this.Close(); };

            this.Controls.Add(lblKey);
            this.Controls.Add(cbKey);
            this.Controls.Add(lblModifier);
            this.Controls.Add(cbModifier);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }
}
