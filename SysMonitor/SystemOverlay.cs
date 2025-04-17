using System;
using System.Threading;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using System.Windows.Forms;

namespace MonitorTray
{
    public class SystemOverlay : IDisposable
    {
        private readonly string FontName = "Segoe UI";
        private readonly float FontSize = 12f;
        private readonly bool FontBold = true;
        private readonly byte FontColorR = 255, FontColorG = 255, FontColorB = 255, FontColorA = 255;
        private readonly int OverlayWidth;
        private readonly int OverlayHeight;
        private readonly int TextStartX = 40;
        private readonly int TextStartY = 40;
        private readonly int LineHeight = 15;
        private readonly byte OutlineColorR = 0, OutlineColorG = 0, OutlineColorB = 0, OutlineColorA = 255;
        private readonly int OutlineThickness = 5;

        private readonly GraphicsWindow _window;
        private readonly HardwareMonitor _hardwareMonitor;
        private readonly MonitorApplicationContext _appContext;
        private System.Threading.Timer _updateTimer;
        private GameOverlay.Drawing.Font _font;
        private GameOverlay.Drawing.SolidBrush _brush;
        private GameOverlay.Drawing.SolidBrush _outlineBrush;
        private string[] _rows = Array.Empty<string>();
        private bool _disposed = false;

        public SystemOverlay(MonitorApplicationContext appContext, int updateIntervalMs = 1000)
        {
            _appContext = appContext;
            _hardwareMonitor = new HardwareMonitor();

            var screen = Screen.PrimaryScreen.Bounds;
            OverlayWidth = screen.Width;
            OverlayHeight = screen.Height;
            var gfx = new GameOverlay.Drawing.Graphics
            {
                MeasureFPS = false,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                VSync = false
            };

            _window = new GraphicsWindow(appContext.Config.OverlayX, appContext.Config.OverlayY, OverlayWidth, OverlayHeight, gfx)
            {
                FPS = 6,
                IsTopmost = true,
                IsVisible = true
            };

            _window.DrawGraphics += Window_DrawGraphics;
            _window.SetupGraphics += Window_SetupGraphics;
            _window.DestroyGraphics += Window_DestroyGraphics;

            _window.Create();

            _updateTimer = new System.Threading.Timer(UpdateData, null, 0, updateIntervalMs);
        }

        private void Window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            _font = e.Graphics.CreateFont(FontName, FontSize, FontBold);
            _brush = e.Graphics.CreateSolidBrush(FontColorA, FontColorR, FontColorG, FontColorB);
            _outlineBrush = e.Graphics.CreateSolidBrush(OutlineColorA, OutlineColorR, OutlineColorG, OutlineColorB);
        }

        private void Window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            _font.Dispose();
            _brush.Dispose();
            _outlineBrush.Dispose();
        }

        private void Window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            e.Graphics.ClearScene();
            int x = TextStartX, y = TextStartY;
            foreach (var row in _rows)
            {
                for (int dx = -OutlineThickness; dx <= OutlineThickness; dx++)
                for (int dy = -OutlineThickness; dy <= OutlineThickness; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    e.Graphics.DrawText(_font, _outlineBrush, x + dx, y + dy, row);
                }
                e.Graphics.DrawText(_font, _brush, x, y, row);
                y += LineHeight;
            }
        }

        private void UpdateData(object state)
        {
            _hardwareMonitor.UpdateAll();
            var cpuInfo = _hardwareMonitor.GetCpuInfoDetailed();
            float cpuTemp = -1, cpuLoad = -1;
            string noData = _appContext?.T("labels.no_data") ?? "--";
            foreach (var kvp in cpuInfo.Temperatures)
            {
                if (kvp.Key.ToLower().Contains("tctl"))
                {
                    if (float.TryParse(kvp.Value.Replace(noData, "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float val))
                    {
                        cpuTemp = val;
                        break;
                    }
                }
            }
            if (cpuTemp < 0)
            {
                foreach (var kvp in cpuInfo.Temperatures)
                {
                    if (kvp.Key.ToLower().Contains("core #1") || kvp.Key.ToLower().Contains("package") || kvp.Key.ToLower().Contains("cpu"))
                    {
                        if (float.TryParse(kvp.Value.Replace(noData, "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float val))
                        {
                            cpuTemp = val;
                            break;
                        }
                    }
                }
            }
            foreach (var kvp in cpuInfo.Loads)
            {
                if (kvp.Key.ToLower().Contains("total") || kvp.Key.ToLower().Contains("cpu"))
                {
                    if (float.TryParse(kvp.Value.Replace(noData, "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float val))
                    {
                        cpuLoad = val;
                        break;
                    }
                }
            }
            var gpuInfo = _hardwareMonitor.GetGpuInfoDetailed();
            float gpuTemp = -1, gpuLoad = -1;
            foreach (var kvp in gpuInfo.Temperatures)
            {
                if (kvp.Key.ToLower().Contains("core") || kvp.Key.ToLower().Contains("gpu"))
                {
                    if (float.TryParse(kvp.Value.Replace(noData, "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float val))
                    {
                        gpuTemp = val;
                        break;
                    }
                }
            }
            foreach (var kvp in gpuInfo.Loads)
            {
                if (kvp.Key.ToLower().Contains("core") || kvp.Key.ToLower().Contains("gpu"))
                {
                    if (float.TryParse(kvp.Value.Replace(noData, "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float val))
                    {
                        gpuLoad = val;
                        break;
                    }
                }
            }
            var netInfo = _hardwareMonitor.GetNetworkInfoDetailed();
            float totalDown = 0, totalUp = 0;
            foreach (var iface in netInfo.DownloadSpeed.Keys)
            {
                totalDown += netInfo.DownloadSpeed[iface];
                if (netInfo.UploadSpeed.ContainsKey(iface))
                    totalUp += netInfo.UploadSpeed[iface];
            }
            float downMbit = totalDown * 8 / 1_000_000f;
            float upMbit = totalUp * 8 / 1_000_000f;
            string gpuLabel = _appContext?.T("labels.gpu") ?? "GPU";
            string cpuLabel = _appContext?.T("labels.cpu") ?? "CPU";
            string tempLabel = _appContext?.T("labels.temp") ?? "Temp";
            string loadLabel = _appContext?.T("labels.load") ?? "Load";
            string celsius = _appContext?.T("units.celsius") ?? "°C";
            string percent = _appContext?.T("units.percent") ?? "%";
            string noValue = _appContext?.T("labels.no_value") ?? "--";
            string mbps = _appContext?.T("labels.mbps") ?? "Мбит/с";
            _rows = new[]
            {
                $"{gpuLabel}  {tempLabel}: {(gpuTemp >= 0 ? gpuTemp.ToString("F0") : noValue)} {celsius}  |  {loadLabel}: {(gpuLoad >= 0 ? gpuLoad.ToString("F0") : noValue)} {percent}",
                $"{cpuLabel}  {tempLabel}: {(cpuTemp >= 0 ? cpuTemp.ToString("F0") : noValue)} {celsius}  |  {loadLabel}: {(cpuLoad >= 0 ? cpuLoad.ToString("F0") : noValue)} {percent}",
                $"↓ {downMbit:F2} {mbps}   |   ↑ {upMbit:F2} {mbps}"
            };
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _updateTimer?.Dispose();
            _window?.Dispose();
        }
    }
} 