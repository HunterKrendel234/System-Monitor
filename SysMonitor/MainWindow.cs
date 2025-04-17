
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.IO;
using System.Linq;

namespace MonitorTray
{

    public class MainWindow : Form
    {
        private TabControl tabControl;
        private DataGridView cpuGridView;
        private Label cpuNameLabel;
        private DataGridView ramGridView;
        private Label ramNameLabel;
        private HardwareMonitor hardwareMonitor;
        private System.Windows.Forms.Timer updateTimer;
        private DataGridView gpuGridView;
        private DataGridView mbGridView;
        private Label gpuNameLabel;
        private Label mbNameLabel;
        private List<TabPage> diskTabs = new List<TabPage>();
        private List<DataGridView> diskGridViews = new List<DataGridView>();
        private List<Label> diskNameLabels = new List<Label>();
        private List<string[]> lastCpuRows = new List<string[]>();
        private List<string[]> lastRamRows = new List<string[]>();
        private List<string[]> lastGpuRows = new List<string[]>();
        private List<string[]> lastMbRows = new List<string[]>();
        private List<List<string[]>> lastDiskRows = new List<List<string[]>>();
        private DataGridView netGridView;
        private Label netNameLabel;
        private readonly MonitorApplicationContext appContext;

        public MainWindow(MonitorApplicationContext context)
        {
            appContext = context;
            this.Text = "System Monitor";
            this.Size = new Size(660, 750);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = SystemFonts.DefaultFont;
            this.Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "style", "monitor.ico"));
            tabControl = new TabControl() { Dock = DockStyle.Fill };
            hardwareMonitor = new HardwareMonitor();

            // CPU Tab
            var cpuTab = new TabPage("CPU");
            cpuNameLabel = new Label() { Dock = DockStyle.Top, Height = 36, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(32, 32, 32), Padding = new Padding(8, 0, 0, 0), BackColor = Color.FromArgb(245, 246, 250) };
            cpuNameLabel.Text = appContext.T("labels.cpu") + ":";
            cpuGridView = new DataGridView() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BorderStyle = BorderStyle.None, BackgroundColor = Color.FromArgb(255, 255, 255), Font = new Font("Segoe UI", 9F, FontStyle.Regular), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            cpuGridView.Columns.Add(appContext.T("column_param") + "1", appContext.T("column_param"));
            cpuGridView.Columns.Add(appContext.T("column_value") + "1", appContext.T("column_value"));
            cpuGridView.Columns.Add(appContext.T("column_unit") + "1", appContext.T("column_unit"));
            cpuGridView.Columns.Add(appContext.T("column_param") + "2", appContext.T("column_param"));
            cpuGridView.Columns.Add(appContext.T("column_value") + "2", appContext.T("column_value"));
            cpuGridView.Columns.Add(appContext.T("column_unit") + "2", appContext.T("column_unit"));
            cpuGridView.AllowUserToResizeColumns = false;
            cpuGridView.AllowUserToResizeRows = false;
            cpuGridView.SelectionChanged += (s, e) => { cpuGridView.ClearSelection(); };
            cpuGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            cpuGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(32, 32, 32);
            cpuGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            cpuGridView.DefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            cpuGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cpuGridView.GridColor = Color.FromArgb(230, 230, 230);
            cpuGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
            cpuGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            cpuGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            cpuGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cpuGridView.EnableHeadersVisualStyles = false;
            var cpuPanel = new Panel() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 246, 250) };
            cpuPanel.Controls.Add(cpuGridView);
            cpuPanel.Controls.Add(cpuNameLabel);
            cpuTab.Controls.Add(cpuPanel);
            tabControl.TabPages.Add(cpuTab);

            // RAM Tab
            var ramTab = new TabPage("RAM");
            ramNameLabel = new Label() { Dock = DockStyle.Top, Height = 36, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(32, 32, 32), Padding = new Padding(8, 0, 0, 0), BackColor = Color.FromArgb(245, 246, 250) };
            ramNameLabel.Text = appContext.T("labels.memory") + ":";

            ramGridView = new DataGridView() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BorderStyle = BorderStyle.None, BackgroundColor = Color.FromArgb(255, 255, 255), Font = new Font("Segoe UI", 9F, FontStyle.Regular) };
            ramGridView.Columns.Add(appContext.T("column_param"), appContext.T("column_param"));
            ramGridView.Columns.Add(appContext.T("column_value"), appContext.T("column_value"));
            ramGridView.Columns.Add(appContext.T("column_unit"), appContext.T("column_unit"));
            ramGridView.AllowUserToResizeColumns = false;
            ramGridView.AllowUserToResizeRows = false;
            ramGridView.SelectionChanged += (s, e) => { ramGridView.ClearSelection(); };
            ramGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            ramGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(32, 32, 32);
            ramGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            ramGridView.DefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            ramGridView.GridColor = Color.FromArgb(230, 230, 230);
            ramGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
            ramGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            ramGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            ramGridView.EnableHeadersVisualStyles = false;
            ramGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ramGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ramGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var ramPanel = new Panel() { Dock = DockStyle.Fill };
            ramPanel.Controls.Add(ramGridView);
            ramTab.Controls.Add(ramPanel);
            ramTab.Controls.Add(ramNameLabel);
            tabControl.TabPages.Add(ramTab);

            // GPU Tab
            var gpuTab = new TabPage("GPU");
            gpuNameLabel = new Label() { Dock = DockStyle.Top, Height = 36, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(32, 32, 32), Padding = new Padding(8, 0, 0, 0), BackColor = Color.FromArgb(245, 246, 250) };
            gpuNameLabel.Text = appContext.T("labels.gpu") + ":";
            gpuGridView = new DataGridView() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BorderStyle = BorderStyle.None, BackgroundColor = Color.FromArgb(255, 255, 255), Font = new Font("Segoe UI", 9F, FontStyle.Regular), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            gpuGridView.Columns.Add(appContext.T("column_param") + "1", appContext.T("column_param"));
            gpuGridView.Columns.Add(appContext.T("column_value") + "1", appContext.T("column_value"));
            gpuGridView.Columns.Add(appContext.T("column_unit") + "1", appContext.T("column_unit"));
            gpuGridView.Columns.Add(appContext.T("column_param") + "2", appContext.T("column_param"));
            gpuGridView.Columns.Add(appContext.T("column_value") + "2", appContext.T("column_value"));
            gpuGridView.Columns.Add(appContext.T("column_unit") + "2", appContext.T("column_unit"));
            gpuGridView.AllowUserToResizeColumns = false;
            gpuGridView.AllowUserToResizeRows = false;
            gpuGridView.SelectionChanged += (s, e) => { gpuGridView.ClearSelection(); };
            gpuGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            gpuGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(32, 32, 32);
            gpuGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            gpuGridView.DefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            gpuGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gpuGridView.GridColor = Color.FromArgb(230, 230, 230);
            gpuGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
            gpuGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            gpuGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            gpuGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gpuGridView.EnableHeadersVisualStyles = false;
            var gpuPanel = new Panel() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 246, 250) };
            gpuPanel.Controls.Add(gpuGridView);
            gpuPanel.Controls.Add(gpuNameLabel);
            gpuTab.Controls.Add(gpuPanel);
            tabControl.TabPages.Add(gpuTab);

            // Motherboard Tab
            var mbTab = new TabPage("Motherboard");
            mbNameLabel = new Label() { Dock = DockStyle.Top, Height = 36, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(32, 32, 32), Padding = new Padding(8, 0, 0, 0), BackColor = Color.FromArgb(245, 246, 250) };
            mbNameLabel.Text = appContext.T("labels.motherboard") + ":";

            mbGridView = new DataGridView() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BorderStyle = BorderStyle.None, BackgroundColor = Color.FromArgb(255, 255, 255), Font = new Font("Segoe UI", 9F, FontStyle.Regular) };
            mbGridView.Columns.Add(appContext.T("column_param"), appContext.T("column_param"));
            mbGridView.Columns.Add(appContext.T("column_value"), appContext.T("column_value"));
            mbGridView.Columns.Add(appContext.T("column_unit"), appContext.T("column_unit"));
            mbGridView.AllowUserToResizeColumns = false;
            mbGridView.AllowUserToResizeRows = false;
            mbGridView.SelectionChanged += (s, e) => { mbGridView.ClearSelection(); };
            mbGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            mbGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(32, 32, 32);
            mbGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            mbGridView.DefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            mbGridView.GridColor = Color.FromArgb(230, 230, 230);
            mbGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
            mbGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            mbGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            mbGridView.EnableHeadersVisualStyles = false;
            mbGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            mbGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            mbGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var mbPanel = new Panel() { Dock = DockStyle.Fill };
            mbPanel.Controls.Add(mbGridView);
            mbTab.Controls.Add(mbPanel);
            mbTab.Controls.Add(mbNameLabel);
            tabControl.TabPages.Add(mbTab);


            var netTab = new TabPage(appContext.T("labels.internet") ?? "Интернет");
            netNameLabel = new Label() { Dock = DockStyle.Top, Height = 36, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(32, 32, 32), Padding = new Padding(8, 0, 0, 0), BackColor = Color.FromArgb(245, 246, 250), Text = (appContext.T("labels.internet") ?? "Интернет") + ":" };
            netGridView = new DataGridView() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BorderStyle = BorderStyle.None, BackgroundColor = Color.FromArgb(255, 255, 255), Font = new Font("Segoe UI", 9F, FontStyle.Regular) };
            netGridView.Columns.Add("iface", appContext.T("labels.interface") ?? "Интерфейс");
            netGridView.Columns.Add("down", "↓ " + (appContext.T("labels.mbps") ?? "Мбит/с"));
            netGridView.Columns.Add("up", "↑ " + (appContext.T("labels.mbps") ?? "Мбит/с"));
            netGridView.AllowUserToResizeColumns = false;
            netGridView.AllowUserToResizeRows = false;
            netGridView.SelectionChanged += (s, e) => { netGridView.ClearSelection(); };
            netGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            netGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(32, 32, 32);
            netGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            netGridView.DefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            netGridView.GridColor = Color.FromArgb(230, 230, 230);
            netGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
            netGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            netGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            netGridView.EnableHeadersVisualStyles = false;
            netGridView.CellClick += NetGridView_CellClick;
            netGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            netGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            netGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            var netPanel = new Panel() { Dock = DockStyle.Fill };
            netPanel.Controls.Add(netGridView);
            netTab.Controls.Add(netPanel);
            netTab.Controls.Add(netNameLabel);
            tabControl.TabPages.Add(netTab);


            var allDisks = hardwareMonitor.GetAllDisksInfo()?.FindAll(d => d != null) ?? new List<HardwareMonitor.DiskInfoDetailed>();
            diskTabs.Clear();
            diskGridViews.Clear();
            diskNameLabels.Clear();
            for (int i = 0; i < allDisks.Count; i++)
            {
                var disk = allDisks[i];
                var diskTab = new TabPage($"{appContext.T("labels.disk")} {i} ({disk.Name})");
                var diskNameLabelDyn = new Label() { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Text = appContext.T("labels.disk") + $": {disk.Name}" };
                var diskGridViewDyn = new DataGridView()
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    RowHeadersVisible = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    BorderStyle = BorderStyle.None,
                    BackgroundColor = Color.FromArgb(255, 255, 255),
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ScrollBars = ScrollBars.Vertical
                };
                diskGridViewDyn.Columns.Add(appContext.T("column_param") + "1", appContext.T("column_param"));
                diskGridViewDyn.Columns.Add(appContext.T("column_value") + "1", appContext.T("column_value"));
                diskGridViewDyn.Columns.Add(appContext.T("column_unit") + "1", appContext.T("column_unit"));
                diskGridViewDyn.Columns.Add(appContext.T("column_param") + "2", appContext.T("column_param"));
                diskGridViewDyn.Columns.Add(appContext.T("column_value") + "2", appContext.T("column_value"));
                diskGridViewDyn.Columns.Add(appContext.T("column_unit") + "2", appContext.T("column_unit"));
                diskGridViewDyn.AllowUserToResizeColumns = false;
                diskGridViewDyn.AllowUserToResizeRows = false;
                diskGridViewDyn.SelectionChanged += (s, e) => { diskGridViewDyn.ClearSelection(); };
                diskGridViewDyn.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
                diskGridViewDyn.DefaultCellStyle.SelectionForeColor = Color.FromArgb(32, 32, 32);
                diskGridViewDyn.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
                diskGridViewDyn.DefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
                diskGridViewDyn.GridColor = Color.FromArgb(230, 230, 230);
                diskGridViewDyn.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
                diskGridViewDyn.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
                diskGridViewDyn.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                diskGridViewDyn.EnableHeadersVisualStyles = false;
                diskGridViewDyn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                diskGridViewDyn.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                var diskPanelDyn = new Panel() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 246, 250) };
                diskPanelDyn.Controls.Add(diskGridViewDyn);
                diskPanelDyn.Controls.Add(diskNameLabelDyn);
                diskTab.Controls.Add(diskPanelDyn);
                diskNameLabels.Add(diskNameLabelDyn);
                diskGridViews.Add(diskGridViewDyn);
                diskTabs.Add(diskTab);
                tabControl.TabPages.Add(diskTab);


                var typeBlocks = new List<(string title, List<(string, string, string)> rows)>()
                {
                    (appContext.T("labels.temperatures"), new List<(string, string, string)>()),
                    (appContext.T("labels.load"), new List<(string, string, string)>()),
                    (appContext.T("labels.data"), new List<(string, string, string)>()),
                    (appContext.T("labels.other"), new List<(string, string, string)>())
                };
                foreach (var item in disk.Temperatures)
                    typeBlocks[0].rows.Add((item.Key, item.Value, appContext.T("units.celsius")));
                foreach (var item in disk.Loads)
                    typeBlocks[1].rows.Add((item.Key, item.Value, appContext.T("units.percent")));
                foreach (var item in disk.Data)
                    typeBlocks[2].rows.Add((item.Key, item.Value, appContext.T("units.mb_gb")));
                foreach (var item in disk.Others)
                    typeBlocks[3].rows.Add((item.Key, item.Value, ""));

                var nonEmptyBlocks = typeBlocks.Where(b => b.rows.Count > 0).ToList();
                if (nonEmptyBlocks.Count == 0)
                {
                    diskGridViewDyn.Rows.Add(appContext.T("labels.no_data"), "", "", "", "", "");
                    continue;
                }
                int mid = (nonEmptyBlocks.Count + 1) / 2;
                var leftBlocks = nonEmptyBlocks.Take(mid).ToList();
                var rightBlocks = nonEmptyBlocks.Skip(mid).ToList();

                var leftRows = new List<(string, string, string)>();
                var rightRows = new List<(string, string, string)>();
                foreach (var block in leftBlocks)
                {
                    leftRows.Add((block.title, "", ""));
                    leftRows.AddRange(block.rows);
                }
                foreach (var block in rightBlocks)
                {
                    rightRows.Add((block.title, "", ""));
                    rightRows.AddRange(block.rows);
                }
                int maxRows = Math.Max(leftRows.Count, rightRows.Count);
                for (int r = 0; r < maxRows; r++)
                {
                    var l = r < leftRows.Count ? leftRows[r] : ("", "", "");
                    var rr = r < rightRows.Count ? rightRows[r] : ("", "", "");
                    diskGridViewDyn.Rows.Add(l.Item1, l.Item2, l.Item3, rr.Item1, rr.Item2, rr.Item3);
                }

                foreach (DataGridViewRow row in diskGridViewDyn.Rows)
                {
                    if (row.Cells.Count < 6) continue;
                    bool isLeftSep = row.Cells[1].Value?.ToString() == "" && row.Cells[2].Value?.ToString() == "" && !string.IsNullOrEmpty(row.Cells[0].Value?.ToString());
                    bool isRightSep = row.Cells[4].Value?.ToString() == "" && row.Cells[5].Value?.ToString() == "" && !string.IsNullOrEmpty(row.Cells[3].Value?.ToString());
                    if (isLeftSep)
                    {
                        row.Cells[0].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                        row.Cells[0].Style.BackColor = Color.LightGray;
                        row.Cells[1].Style.BackColor = Color.LightGray;
                        row.Cells[2].Style.BackColor = Color.LightGray;
                    }
                    if (isRightSep)
                    {
                        row.Cells[3].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                        row.Cells[3].Style.BackColor = Color.LightGray;
                        row.Cells[4].Style.BackColor = Color.LightGray;
                        row.Cells[5].Style.BackColor = Color.LightGray;
                    }
                }
            }

            this.Controls.Add(tabControl);
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateCpuInfo;
            updateTimer.Start();
        }

        private DataGridView CreateGridView()
        {
            var grid = new DataGridView()
            {
                Dock = DockStyle.Left,
                Width = 320,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.FromArgb(255, 255, 255),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };
            grid.Columns.Add(appContext.T("column_param"), appContext.T("column_param"));
            grid.Columns.Add(appContext.T("column_value"), appContext.T("column_value"));
            grid.Columns.Add(appContext.T("column_unit"), appContext.T("column_unit"));
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionChanged += (s, e) => { grid.ClearSelection(); };
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(32, 32, 32);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            grid.GridColor = Color.FromArgb(230, 230, 230);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(32, 32, 32);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;
            return grid;
        }

        private void UpdateCpuInfo(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.Visible ||
                cpuGridView == null || cpuGridView.IsDisposed ||
                ramGridView == null || ramGridView.IsDisposed ||
                gpuGridView == null || gpuGridView.IsDisposed ||
                mbGridView == null || mbGridView.IsDisposed ||
                netGridView == null || netGridView.IsDisposed)
                return;
            hardwareMonitor.UpdateAll();
            var cpuInfo = hardwareMonitor.GetCpuInfoDetailed();
            string cpuName = hardwareMonitor.GetCpuName();
            cpuNameLabel.Text = appContext.T("labels.cpu") + ": " + cpuName;


            var ramInfo = hardwareMonitor.GetRamInfoDetailed();
            ramNameLabel.Text = appContext.T("labels.memory") + ": " + hardwareMonitor.GetRamModulesInfo();
            var ramRows = new List<string[]>();
            void AddRamSeparator(DataGridView grid, string title)
            {
                ramRows.Add(new[] { title, "", "" });
            }
            bool ramHasData = false;
            if (ramInfo.Data.Count > 0)
            {
                AddRamSeparator(ramGridView, appContext.T("labels.volume_usage"));
                foreach (var item in ramInfo.Data)
                    ramRows.Add(new[] { item.Key, item.Value, appContext.T("units.mb_gb") });
                ramHasData = true;
            }
            if (ramInfo.Clocks.Count > 0)
            {
                AddRamSeparator(ramGridView, appContext.T("labels.clocks"));
                foreach (var item in ramInfo.Clocks)
                    ramRows.Add(new[] { item.Key, item.Value, appContext.T("units.mhz") });
                ramHasData = true;
            }
            var ramModules = hardwareMonitor.GetRamModulesFullInfo();
            if (ramModules.Count > 0)
            {
                AddRamSeparator(ramGridView, appContext.T("labels.ram_modules"));
                int idx = 1;
                foreach (var mod in ramModules)
                {
                    foreach (var kvp in mod)
                        ramRows.Add(new[] { $"{appContext.T("labels.ram_module")} {idx}: {kvp.Key}", kvp.Value, "" });
                    idx++;
                }
                ramHasData = true;
            }
            if (!ramHasData)
            {
                ramRows.Add(new[] { appContext.T("labels.no_data"), "", "" });
            }
            ramGridView.Rows.Clear();
            foreach (var row in ramRows)
                ramGridView.Rows.Add(row);

            foreach (DataGridViewRow row in ramGridView.Rows)
            {
                if (row.Cells.Count < 6) continue;
                if (row.Cells[1].Value?.ToString() == "" && row.Cells[2].Value?.ToString() == "")
                {
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }


            int scrollLeft = cpuGridView.FirstDisplayedScrollingRowIndex;
            var cpuTypeBlocks = new List<(string title, List<(string, string, string)> rows)>()
            {
                (appContext.T("labels.temperatures"), new List<(string, string, string)>()),
                (appContext.T("labels.clocks"), new List<(string, string, string)>()),
                (appContext.T("labels.voltages"), new List<(string, string, string)>()),
                (appContext.T("labels.load"), new List<(string, string, string)>()),
                (appContext.T("labels.power_consumption"), new List<(string, string, string)>())
            };
            foreach (var item in cpuInfo.Temperatures)
                cpuTypeBlocks[0].rows.Add((item.Key, item.Value, appContext.T("units.celsius")));
            foreach (var item in cpuInfo.Clocks)
                cpuTypeBlocks[1].rows.Add((item.Key, item.Value, appContext.T("units.mhz")));
            foreach (var item in cpuInfo.Voltages)
                cpuTypeBlocks[2].rows.Add((item.Key, item.Value, appContext.T("units.volts")));
            foreach (var item in cpuInfo.Loads)
                cpuTypeBlocks[3].rows.Add((item.Key, item.Value, appContext.T("units.percent")));
            foreach (var item in cpuInfo.Powers)
                cpuTypeBlocks[4].rows.Add((item.Key, item.Value, appContext.T("units.watts")));
            var cpuNonEmptyBlocks = cpuTypeBlocks.Where(b => b.rows.Count > 0).ToList();
            if (cpuNonEmptyBlocks.Count == 0)
            {
                cpuGridView.Rows.Clear();
                cpuGridView.Rows.Add(appContext.T("labels.no_data"), "", "", "", "", "");
            } else {
                int cpuMid = (cpuNonEmptyBlocks.Count + 1) / 2;
                var cpuLeftBlocks = cpuNonEmptyBlocks.Take(cpuMid).ToList();
                var cpuRightBlocks = cpuNonEmptyBlocks.Skip(cpuMid).ToList();
                var cpuLeftRows = new List<(string, string, string)>();
                var cpuRightRows = new List<(string, string, string)>();
                foreach (var block in cpuLeftBlocks)
                {
                    cpuLeftRows.Add((block.title, "", ""));
                    cpuLeftRows.AddRange(block.rows);
                }
                foreach (var block in cpuRightBlocks)
                {
                    cpuRightRows.Add((block.title, "", ""));
                    cpuRightRows.AddRange(block.rows);
                }
                int cpuMaxRows = Math.Max(cpuLeftRows.Count, cpuRightRows.Count);
                cpuGridView.Rows.Clear();
                for (int r = 0; r < cpuMaxRows; r++)
                {
                    var l = r < cpuLeftRows.Count ? cpuLeftRows[r] : ("", "", "");
                    var rr = r < cpuRightRows.Count ? cpuRightRows[r] : ("", "", "");
                    cpuGridView.Rows.Add(l.Item1, l.Item2, l.Item3, rr.Item1, rr.Item2, rr.Item3);
                }

                foreach (DataGridViewRow row in cpuGridView.Rows)
                {
                    if (row.Cells.Count < 6) continue;
                    bool isLeftSep = row.Cells[1].Value?.ToString() == "" && row.Cells[2].Value?.ToString() == "" && !string.IsNullOrEmpty(row.Cells[0].Value?.ToString());
                    bool isRightSep = row.Cells[4].Value?.ToString() == "" && row.Cells[5].Value?.ToString() == "" && !string.IsNullOrEmpty(row.Cells[3].Value?.ToString());
                    if (isLeftSep)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            row.Cells[c].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                            row.Cells[c].Style.BackColor = Color.LightGray;
                        }
                    }
                    if (isRightSep)
                    {
                        for (int c = 3; c < 6; c++)
                        {
                            row.Cells[c].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                            row.Cells[c].Style.BackColor = Color.LightGray;
                        }
                    }
                }
            }
            if (scrollLeft >= 0 && scrollLeft < cpuGridView.RowCount)
                cpuGridView.FirstDisplayedScrollingRowIndex = scrollLeft;

            var gpuInfo = hardwareMonitor.GetGpuInfoDetailed();
            gpuNameLabel.Text = appContext.T("labels.gpu") + ": " + (gpuInfo.Name ?? "");
            int gpuScrollLeft = gpuGridView.FirstDisplayedScrollingRowIndex;
            var gpuTypeBlocks = new List<(string title, List<(string, string, string)> rows)>()
            {
                (appContext.T("labels.temperatures"), new List<(string, string, string)>()),
                (appContext.T("labels.clocks"), new List<(string, string, string)>()),
                (appContext.T("labels.fans"), new List<(string, string, string)>()),
                (appContext.T("labels.memory"), new List<(string, string, string)>()),
                (appContext.T("labels.load"), new List<(string, string, string)>()),
                (appContext.T("labels.power_consumption"), new List<(string, string, string)>())
            };
            foreach (var item in gpuInfo.Temperatures)
                gpuTypeBlocks[0].rows.Add((item.Key, item.Value, appContext.T("units.celsius")));
            foreach (var item in gpuInfo.Clocks)
                gpuTypeBlocks[1].rows.Add((item.Key, item.Value, appContext.T("units.mhz")));
            foreach (var item in gpuInfo.Fans)
                gpuTypeBlocks[2].rows.Add((item.Key, item.Value, appContext.T("units.rpm")));
            foreach (var item in gpuInfo.Memory)
                gpuTypeBlocks[3].rows.Add((item.Key, item.Value, appContext.T("units.mb")));
            foreach (var item in gpuInfo.Loads)
                gpuTypeBlocks[4].rows.Add((item.Key, item.Value, appContext.T("units.percent")));
            foreach (var item in gpuInfo.Powers)
                gpuTypeBlocks[5].rows.Add((item.Key, item.Value, appContext.T("units.watts")));
            var gpuNonEmptyBlocks = gpuTypeBlocks.Where(b => b.rows.Count > 0).ToList();
            if (gpuNonEmptyBlocks.Count == 0)
            {
                gpuGridView.Rows.Clear();
                gpuGridView.Rows.Add(appContext.T("labels.no_data"), "", "", "", "", "");
            } else {
                int gpuMid = (gpuNonEmptyBlocks.Count + 1) / 2;
                var gpuLeftBlocks = gpuNonEmptyBlocks.Take(gpuMid).ToList();
                var gpuRightBlocks = gpuNonEmptyBlocks.Skip(gpuMid).ToList();
                var gpuLeftRows = new List<(string, string, string)>();
                var gpuRightRows = new List<(string, string, string)>();
                foreach (var block in gpuLeftBlocks)
                {
                    gpuLeftRows.Add((block.title, "", ""));
                    gpuLeftRows.AddRange(block.rows);
                }
                foreach (var block in gpuRightBlocks)
                {
                    gpuRightRows.Add((block.title, "", ""));
                    gpuRightRows.AddRange(block.rows);
                }
                int gpuMaxRows = Math.Max(gpuLeftRows.Count, gpuRightRows.Count);
                gpuGridView.Rows.Clear();
                for (int r = 0; r < gpuMaxRows; r++)
                {
                    var l = r < gpuLeftRows.Count ? gpuLeftRows[r] : ("", "", "");
                    var rr = r < gpuRightRows.Count ? gpuRightRows[r] : ("", "", "");
                    gpuGridView.Rows.Add(l.Item1, l.Item2, l.Item3, rr.Item1, rr.Item2, rr.Item3);
                }

                foreach (DataGridViewRow row in gpuGridView.Rows)
                {
                    if (row.Cells.Count < 6) continue;
                    bool isLeftSep = row.Cells[1].Value?.ToString() == "" && row.Cells[2].Value?.ToString() == "" && !string.IsNullOrEmpty(row.Cells[0].Value?.ToString());
                    bool isRightSep = row.Cells[4].Value?.ToString() == "" && row.Cells[5].Value?.ToString() == "" && !string.IsNullOrEmpty(row.Cells[3].Value?.ToString());
                    if (isLeftSep)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            row.Cells[c].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                            row.Cells[c].Style.BackColor = Color.LightGray;
                        }
                    }
                    if (isRightSep)
                    {
                        for (int c = 3; c < 6; c++)
                        {
                            row.Cells[c].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                            row.Cells[c].Style.BackColor = Color.LightGray;
                        }
                    }
                }
            }
            if (gpuScrollLeft >= 0 && gpuScrollLeft < gpuGridView.RowCount)
                gpuGridView.FirstDisplayedScrollingRowIndex = gpuScrollLeft;


            var mbInfo = hardwareMonitor.GetMotherboardInfoDetailed();
            mbNameLabel.Text = appContext.T("labels.motherboard") + ": " + hardwareMonitor.GetMotherboardName();
            var mbRows = new List<string[]>();
            void AddMbSeparator(DataGridView grid, string title)
            {
                mbRows.Add(new[] { title, "", "" });
            }
            bool mbHasData = false;
            if (mbInfo.Temperatures.Count > 0)
            {
                AddMbSeparator(mbGridView, appContext.T("labels.temperatures"));
                foreach (var item in mbInfo.Temperatures)
                    mbRows.Add(new[] { item.Key, item.Value, appContext.T("units.celsius") });
                mbHasData = true;
            }
            if (mbInfo.Voltages.Count > 0)
            {
                AddMbSeparator(mbGridView, appContext.T("labels.voltages"));
                foreach (var item in mbInfo.Voltages)
                    mbRows.Add(new[] { item.Key, item.Value, appContext.T("units.volts") });
                mbHasData = true;
            }
            if (mbInfo.Fans.Count > 0)
            {
                AddMbSeparator(mbGridView, appContext.T("labels.fans"));
                foreach (var item in mbInfo.Fans)
                    mbRows.Add(new[] { item.Key, item.Value, appContext.T("units.rpm") });
                mbHasData = true;
            }
            var mbWmi = hardwareMonitor.GetMotherboardFullInfo();
            if (mbWmi.Count > 0)
            {
                AddMbSeparator(mbGridView, appContext.T("labels.motherboard_info"));
                foreach (var kvp in mbWmi)
                    mbRows.Add(new[] { kvp.Key, kvp.Value, "" });
                mbHasData = true;
            }
            if (!mbHasData)
            {
                mbRows.Add(new[] { appContext.T("labels.no_data"), "", "" });
            }
            if (!RowsEqual(mbRows, lastMbRows))
            {
                mbGridView.Rows.Clear();
                foreach (var row in mbRows)
                    mbGridView.Rows.Add(row);
                foreach (DataGridViewRow row in mbGridView.Rows)
                {
                    if (row.Cells.Count < 6) continue;
                    if (row.Cells[1].Value?.ToString() == "" && row.Cells[2].Value?.ToString() == "")
                    {
                        row.DefaultCellStyle.Font = new Font(mbGridView.Font, FontStyle.Bold);
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
                lastMbRows = mbRows;
            }


            var netInfo = hardwareMonitor.GetNetworkInfoDetailed();
            netNameLabel.Text = (appContext.T("labels.internet") ?? "Интернет") + ":";
            netGridView.Rows.Clear();
            bool netHasData = false;
            foreach (var iface in netInfo.DownloadSpeed.Keys)
            {
                float down = netInfo.DownloadSpeed[iface] * 8 / 1_000_000f; 
                float up = netInfo.UploadSpeed.ContainsKey(iface) ? netInfo.UploadSpeed[iface] * 8 / 1_000_000f : 0f;
                netGridView.Rows.Add(iface, down.ToString("F2"), up.ToString("F2"));
                netHasData = true;
            }
            if (!netHasData)
                netGridView.Rows.Add("Нет данных", "-", "-");
        }

        private bool RowsEqual(List<string[]> a, List<string[]> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                for (int j = 0; j < a[i].Length; j++)
                {
                    if (a[i][j] != b[i][j]) return false;
                }
            }
            return true;
        }


        private void NetGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= netGridView.Rows.Count) return;
            var row = netGridView.Rows[e.RowIndex];
            if (row.Cells[0].Value != null && row.Cells[0].Value.ToString().StartsWith("IP: "))
                return;
            if (e.RowIndex + 1 < netGridView.Rows.Count && netGridView.Rows[e.RowIndex + 1].Cells[0].Value != null &&
                netGridView.Rows[e.RowIndex + 1].Cells[0].Value.ToString().StartsWith("IP: "))
            {
                netGridView.Rows.RemoveAt(e.RowIndex + 1);
                return;
            }
            string iface = row.Cells[0].Value?.ToString();
            if (string.IsNullOrWhiteSpace(iface) || iface == "Нет данных") return;

            string ip = GetInterfaceIPv4(iface);

            int insertIdx = e.RowIndex + 1;
            netGridView.Rows.Insert(insertIdx, 1);
            var ipRow = netGridView.Rows[insertIdx];
            ipRow.Cells[0].Value = $"IP: {ip}";
            ipRow.Cells[1].Value = "";
            ipRow.Cells[2].Value = "";
            ipRow.DefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            ipRow.DefaultCellStyle.ForeColor = Color.DimGray;
            ipRow.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private string GetInterfaceIPv4(string ifaceName)
        {
            try
            {
                foreach (var nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.Name == ifaceName)
                    {
                        var props = nic.GetIPProperties();
                        foreach (var ip in props.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }
            }
            catch { }
            return "-";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            updateTimer.Stop();
            updateTimer.Tick -= UpdateCpuInfo; 
            lastCpuRows.Clear();
            lastGpuRows.Clear();
            lastMbRows.Clear();
            base.OnFormClosing(e);
        }
    }
}
