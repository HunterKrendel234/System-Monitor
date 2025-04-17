using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysMonitor
{
    public partial class SettingsForm : Form
    {
        private readonly MonitorTray.MonitorApplicationContext appContext;
        private readonly MonitorTray.Config config;
        private readonly Func<string, string> T;
        private readonly string currentLanguage;
        private bool isInStartup;


        public SettingsForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public SettingsForm(MonitorTray.MonitorApplicationContext context, MonitorTray.Config config, Func<string, string> translator, string lang)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.appContext = context;
            this.config = config;
            this.T = translator;
            this.currentLanguage = lang;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            try {
                this.Icon = new Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "style", "monitor.ico"));
            } catch { this.Icon = SystemIcons.Application; }
            this.Text = T("settings_title");
            lblLanguage.Text = T("language_label");
            lblHotkey.Text = T("key_label");
            lblModifier.Text = T("modifier_label");
            groupOverlayPosition.Text = T("overlay_position_group");
            lblOverlayX.Text = T("overlay_position_x");
            lblOverlayY.Text = T("overlay_position_y");
            tbOverlayX.Text = config.OverlayX.ToString();
            tbOverlayY.Text = config.OverlayY.ToString();
            tbOverlayX.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            tbOverlayY.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            cbLanguage.Items.Clear();
            cbLanguage.Items.AddRange(new object[] { "RU", "EN" });
            cbLanguage.SelectedIndex = config.Language.ToLower() == "en" ? 1 : 0;
            cbModifier.Items.Clear();
            cbModifier.Items.AddRange(new object[] { T("modifier_none"), T("modifier_shift"), T("modifier_alt"), T("modifier_control"), T("modifier_win") });
            int modIdx = 0;
            switch (config.Modifier.ToLower())
            {
                case "shift": modIdx = 1; break;
                case "alt": modIdx = 2; break;
                case "control": modIdx = 3; break;
                case "win": modIdx = 4; break;
            }
            cbModifier.SelectedIndex = modIdx;
            cbHotkey.Items.Clear();
            for (int i = 1; i <= 12; i++)
                cbHotkey.Items.Add("F" + i);
            cbHotkey.SelectedItem = config.Hotkey;
            isInStartup = appContext.IsInStartup();
            btnStartup.Text = isInStartup ? T("startup_button_remove") : T("startup_button_add");
            btnStartup.Click += (s, e) =>
            {
                if (isInStartup)
                {
                    appContext.RemoveFromStartupTaskScheduler();
                    isInStartup = false;
                    btnStartup.Text = T("startup_button_add");
                    MessageBox.Show(T("removed_from_startup"), T("settings_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    appContext.AddToStartupTaskScheduler();
                    isInStartup = true;
                    btnStartup.Text = T("startup_button_remove");
                    MessageBox.Show(T("added_to_startup"), T("settings_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            btnOK.Click += (s, e) =>
            {
                string selectedLang = cbLanguage.SelectedIndex == 1 ? "en" : "ru";
                if (selectedLang != config.Language)
                {
                    appContext.SwitchLanguage(selectedLang);
                }
                string selectedHotkey = cbHotkey.SelectedItem?.ToString() ?? "F9";
                string selectedModifier = "None";
                switch (cbModifier.SelectedIndex)
                {
                    case 1: selectedModifier = "Shift"; break;
                    case 2: selectedModifier = "Alt"; break;
                    case 3: selectedModifier = "Control"; break;
                    case 4: selectedModifier = "Win"; break;
                }
                config.Hotkey = selectedHotkey;
                config.Modifier = selectedModifier;
                if (int.TryParse(tbOverlayX.Text, out int overlayX))
                    config.OverlayX = overlayX;
                if (int.TryParse(tbOverlayY.Text, out int overlayY))
                    config.OverlayY = overlayY;
                appContext.SaveConfig();
                appContext.UnregisterHotKey();
                appContext.RegisterHotKey();
                appContext.RestartOverlayIfActive();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            btnCancel.Click += (s, e) => { this.Close(); };
        }
    }
}
