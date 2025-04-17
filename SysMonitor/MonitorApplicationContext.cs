using System.Text.Json;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using SysMonitor;

namespace MonitorTray
{
    public class MonitorApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private ToolStripMenuItem showOverlayMenuItem;
        private ToolStripMenuItem openMainWindowMenuItem;
        private ToolStripMenuItem hotkeySettingsMenuItem;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem langMenuItem;
        private HotkeyWindow hotkeyWindow;
        private const int HOTKEY_ID = 9000;
        private Icon iconDefault;

        private Config config;
        private readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/config.json");
        private Dictionary<string, object> translations;

        private readonly Dictionary<string, string> fallbackErrors = new Dictionary<string, string>
        {
            {"err_config_load", "Error loading config.json: {0}"},
            {"err_config_save", "Error saving config.json: {0}"},
            {"err_lang_load", "Error loading lang.json: {0}"},
            {"err_title", "Error"}
        };
        private string currentLanguage = "ru";

        private SystemOverlay systemOverlay;
        private MainWindow mainWindow;
        private readonly string githubUrl = "https://github.com/HunterKrendel234";
        private ToolStripMenuItem githubMenuItem;
        private bool wasOverlayActive = false;
        private System.Windows.Forms.Timer delayedHotkeyTimer;
        private bool canShowOverlay = false;
        private ToolStripMenuItem addToStartupMenuItem;
        private ToolStripMenuItem settingsMenuItem;
        private bool wasOverlayActiveOnSettings = false;
        public const string AppVersion = "1.0.0";
        private ToolStripMenuItem versionMenuItem;

        public MonitorApplicationContext()
        {
            LoadConfig();
            LoadTranslations();
            currentLanguage = config.Language;
            LoadIcons();
            InitializeTrayIcon();
            canShowOverlay = false;
            delayedHotkeyTimer = new System.Windows.Forms.Timer();
            delayedHotkeyTimer.Interval = 5000;
            delayedHotkeyTimer.Tick += (s, e) =>
            {
                canShowOverlay = true;
                delayedHotkeyTimer.Stop();
                delayedHotkeyTimer.Dispose();
                delayedHotkeyTimer = null;
            };
            delayedHotkeyTimer.Start();
            RegisterHotKey();

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            mainWindow = new MainWindow(this);
        }

        #region Config
        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    config = JsonSerializer.Deserialize<Config>(json);
                }
                else
                {
                    config = new Config();
                    SaveConfig();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(T("err_config_load"), ex.Message), T("err_title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                config = new Config();
            }
        }

        public void SaveConfig()
        {
            try
            {
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(T("err_config_save"), ex.Message), T("err_title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Lang
        private void LoadTranslations()
        {
            try
            {
                string langPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/lang.json");
                string json = File.ReadAllText(langPath);
                translations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(T("err_lang_load"), ex.Message), T("err_title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                translations = new Dictionary<string, object>();
            }
        }


        public string T(string key)
        {
            if (translations == null || !translations.ContainsKey(currentLanguage))
                return key;
            object section = translations[currentLanguage];
            string[] parts = key.Split('.');
            foreach (var part in parts)
            {
                if (section is Dictionary<string, object> dictObj && dictObj.ContainsKey(part))
                    section = dictObj[part];
                else if (section is System.Text.Json.JsonElement elem && elem.ValueKind == System.Text.Json.JsonValueKind.Object && elem.TryGetProperty(part, out var subElem))
                    section = subElem;
                else
                    return key;
            }
            if (section is string str)
                return str;
            if (section is System.Text.Json.JsonElement elem2 && elem2.ValueKind == System.Text.Json.JsonValueKind.String)
                return elem2.GetString();
            return key;
        }
        #endregion

        #region icons and UI
        private void LoadIcons()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                iconDefault = new Icon(Path.Combine(baseDir, "style", "monitor.ico"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(T("err_icons_load"), ex.Message), T("err_title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                iconDefault = SystemIcons.Application;
            }
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = iconDefault,
                Visible = true,
                Text = T("app_name")
            };

            var contextMenu = new ContextMenuStrip();


            showOverlayMenuItem = new ToolStripMenuItem(T("show_overlay"), null, (s, e) =>
            {
                if (!canShowOverlay) return;
                if (systemOverlay != null)
                {
                    systemOverlay.Dispose();
                    systemOverlay = null;
                }
                else
                {
                    systemOverlay = new SystemOverlay(this, config.UpdateInterval);
                }
            });


            openMainWindowMenuItem = new ToolStripMenuItem(T("open_main_window"), null, (s, e) =>
            {
                if (mainWindow == null || mainWindow.IsDisposed)
                {
                    mainWindow = new MainWindow(this);
                }
                mainWindow.Show();
                mainWindow.BringToFront();
            });


            settingsMenuItem = new ToolStripMenuItem(T("settings"), null, (s, e) =>
            {
                using (var form = new SettingsForm(this, config, T, currentLanguage))
                {
                    form.ShowDialog();
                }
            });


            githubMenuItem = new ToolStripMenuItem(T("github_author"), null, (s, e) =>
            {
                var result = MessageBox.Show(T("github_open_question"), T("github_author"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = githubUrl,
                        UseShellExecute = true
                    });
                }
            });


            exitMenuItem = new ToolStripMenuItem(T("exit"), null, (s, e) => ExitApplication());

            versionMenuItem = new ToolStripMenuItem($"{T("version")}: {AppVersion}");
            versionMenuItem.Enabled = false;

            contextMenu.Items.Add(showOverlayMenuItem);
            contextMenu.Items.Add(openMainWindowMenuItem);
            contextMenu.Items.Add(settingsMenuItem);
            contextMenu.Items.Add(githubMenuItem);
            contextMenu.Items.Add(exitMenuItem);
            contextMenu.Items.Add(versionMenuItem);

            trayIcon.ContextMenuStrip = contextMenu;
            trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (!canShowOverlay) return;
                    if (systemOverlay != null)
                    {
                        systemOverlay.Dispose();
                        systemOverlay = null;
                    }
                    else
                    {
                        systemOverlay = new SystemOverlay(this, config.UpdateInterval);
                    }
                }
            };
        }

        private void UpdateContextMenuText()
        {
            showOverlayMenuItem.Text = T("show_overlay");
            openMainWindowMenuItem.Text = T("open_main_window");
            settingsMenuItem.Text = T("settings");
            githubMenuItem.Text = T("github_author");
            exitMenuItem.Text = T("exit");
            versionMenuItem.Text = $"{T("version")}: {AppVersion}";
            trayIcon.Text = T("app_name");
        }
        #endregion

        #region monitoring
        public void RegisterHotKey()
        {
            if (hotkeyWindow != null)
                return;

            hotkeyWindow = new HotkeyWindow();
            Keys key;
            try
            {
                key = (Keys)Enum.Parse(typeof(Keys), config.Hotkey, true);
            }
            catch
            {
                key = Keys.F9;
            }
            uint modifier = ConvertModifier(config.Modifier);
            if (!hotkeyWindow.RegisterHotKey(HOTKEY_ID, modifier, key))
            {
                MessageBox.Show(string.Format(T("err_hotkey_register"), config.Hotkey), T("err_title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                hotkeyWindow.Dispose();
                hotkeyWindow = null;
            }
            else
            {
                hotkeyWindow.HotkeyPressed += ToggleOverlay;
            }
        }

        private uint ConvertModifier(string mod)
        {
            switch (mod.ToLower())
            {
                case "shift": return 4;
                case "alt": return 1;
                case "control": return 2;
                case "win": return 8;
                default: return 0;
            }
        }

        public void UnregisterHotKey()
        {
            if (hotkeyWindow != null)
            {
                hotkeyWindow.Dispose();
                hotkeyWindow = null;
            }
        }


        private void ToggleOverlay()
        {
            if (!canShowOverlay) return;
            if (systemOverlay != null)
            {
                systemOverlay.Dispose();
                systemOverlay = null;
            }
            else
            {
                systemOverlay = new SystemOverlay(this, config.UpdateInterval);
            }
        }
        #endregion

        public void SwitchLanguage(string lang)
        {
            currentLanguage = lang;
            config.Language = lang;
            SaveConfig();
            UpdateContextMenuText();
            bool wasOpen = mainWindow != null && !mainWindow.IsDisposed && mainWindow.Visible;
            if (mainWindow != null && !mainWindow.IsDisposed)
            {
                mainWindow.Close();
            }
            mainWindow = new MainWindow(this);
            if (wasOpen)
                mainWindow.Show();
        }

        private void ShowHotkeySettings()
        {

            using (var form = new HotkeySettingsForm(config.Hotkey, config.Modifier, T, currentLanguage))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    config.Hotkey = form.SelectedKey;
                    config.Modifier = form.SelectedModifier;
                    SaveConfig();
                    UnregisterHotKey();
                    RegisterHotKey();
                    UpdateContextMenuText();
                }
            }
        }

        private void ExitApplication()
        {
            trayIcon.Visible = false;
            UnregisterHotKey();
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            Application.Exit();
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                if (systemOverlay != null)
                {
                    wasOverlayActive = true;
                    systemOverlay.Dispose();
                    systemOverlay = null;
                }
                else
                {
                    wasOverlayActive = false;
                }
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                if (wasOverlayActive && systemOverlay == null)
                {
                    systemOverlay = new SystemOverlay(this, config.UpdateInterval);
                }
            }
        }

        public bool IsInStartup()
        {
            using (var ts = new Microsoft.Win32.TaskScheduler.TaskService())
            {
                var task = ts.FindTask("SysMonitor_AutoStart", false);
                return task != null;
            }
        }

        private void UpdateAddToStartupMenuText()
        {
            if (addToStartupMenuItem == null) return;
            addToStartupMenuItem.Text = IsInStartup() ? T("remove_from_startup") : T("add_to_startup");
        }

        private void ToggleStartupMenu()
        {
            if (IsInStartup())
            {
                RemoveFromStartupTaskScheduler();
                MessageBox.Show(T("removed_from_startup"), T("remove_from_startup"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                AddToStartupTaskScheduler();
                MessageBox.Show(T("added_to_startup"), T("add_to_startup"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            UpdateAddToStartupMenuText();
        }

        public void AddToStartupTaskScheduler()
        {
            string exePath = Application.ExecutablePath;
            using (var ts = new Microsoft.Win32.TaskScheduler.TaskService())
            {
                try { ts.RootFolder.DeleteTask("SysMonitor_AutoStart", false); } catch { }
                var td = ts.NewTask();
                td.RegistrationInfo.Description = "SysMonitor автозапуск с правами администратора";
                td.Principal.UserId = Environment.UserName;
                td.Principal.LogonType = Microsoft.Win32.TaskScheduler.TaskLogonType.InteractiveToken;
                td.Principal.RunLevel = Microsoft.Win32.TaskScheduler.TaskRunLevel.Highest;
                td.Triggers.Add(new Microsoft.Win32.TaskScheduler.LogonTrigger());
                td.Actions.Add(new Microsoft.Win32.TaskScheduler.ExecAction(exePath, null, null));
                ts.RootFolder.RegisterTaskDefinition("SysMonitor_AutoStart", td);
            }
        }

        public void RemoveFromStartupTaskScheduler()
        {
            using (var ts = new Microsoft.Win32.TaskScheduler.TaskService())
            {
                ts.RootFolder.DeleteTask("SysMonitor_AutoStart", false);
            }
        }

        public void RestartOverlayIfActive()
        {
            if (systemOverlay != null)
            {
                wasOverlayActiveOnSettings = true;
                systemOverlay.Dispose();
                systemOverlay = null;
            }
            else
            {
                wasOverlayActiveOnSettings = false;
            }
            if (wasOverlayActiveOnSettings)
            {
                systemOverlay = new SystemOverlay(this, config.UpdateInterval);
            }
        }

        public Config Config => config;
    }
}
