using LibreHardwareMonitor.Hardware;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Diagnostics;

public class HardwareMonitor
{
    private Computer computer;

    private CpuInfoDetailed cpuInfo = new();
    private GpuInfoDetailed gpuInfo = new();
    private RamInfoDetailed ramInfo = new();
    private MotherboardInfoDetailed motherboardInfo = new();
    private List<DiskInfoDetailed> disksInfo = new();
    private NetworkInfoDetailed networkInfo = new();
    private Dictionary<string, PerformanceCounter> netRecvCounters = new();
    private Dictionary<string, PerformanceCounter> netSentCounters = new();
    private DateTime lastNetworkUpdate = DateTime.MinValue;

    public HardwareMonitor()
    {
        computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMotherboardEnabled = true,
            IsStorageEnabled = true,
            IsMemoryEnabled = false,
            IsNetworkEnabled = false,
            IsControllerEnabled = false
        };
        computer.Open();
        InitNetworkCounters();
        UpdateAll();
    }

    private void InitNetworkCounters()
    {
        netRecvCounters.Clear();
        netSentCounters.Clear();
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                string name = nic.Name;
                netRecvCounters[name] = new PerformanceCounter("Network Interface", "Bytes Received/sec", nic.Description);
                netSentCounters[name] = new PerformanceCounter("Network Interface", "Bytes Sent/sec", nic.Description);
            }
        }
    }

    public void UpdateAll()
    {
        disksInfo.Clear();
        foreach (var hardware in computer.Hardware)
        {
            hardware.Update();
            switch (hardware.HardwareType)
            {
                case HardwareType.Cpu:
                    cpuInfo = ParseCpu(hardware);
                    break;
                case HardwareType.GpuNvidia:
                case HardwareType.GpuAmd:
                case HardwareType.GpuIntel:
                    gpuInfo = ParseGpu(hardware);
                    break;
                case HardwareType.Memory:
                    ramInfo = ParseRam(hardware);
                    break;
                case HardwareType.Motherboard:
                    motherboardInfo = ParseMotherboard(hardware);
                    break;
                case HardwareType.Storage:
                    var disk = ParseDisk(hardware);
                    if (disk != null)
                        disksInfo.Add(disk);
                    break;
            }
        }
        UpdateNetworkInfo();
    }

    private void UpdateNetworkInfo()
    {
        if ((DateTime.Now - lastNetworkUpdate).TotalMilliseconds < 500) return;
        lastNetworkUpdate = DateTime.Now;
        networkInfo.DownloadSpeed.Clear();
        networkInfo.UploadSpeed.Clear();
        foreach (var kvp in netRecvCounters)
        {
            try
            {
                float down = kvp.Value.NextValue();
                float up = netSentCounters[kvp.Key].NextValue();
                networkInfo.DownloadSpeed[kvp.Key] = down;
                networkInfo.UploadSpeed[kvp.Key] = up;
            }
            catch { }
        }
    }

    public class CpuInfoDetailed
    {
        public Dictionary<string, string> Temperatures { get; set; } = new();
        public Dictionary<string, string> Clocks { get; set; } = new();
        public Dictionary<string, string> Voltages { get; set; } = new();
        public Dictionary<string, string> Loads { get; set; } = new();
        public Dictionary<string, string> Powers { get; set; } = new();
    }
    private CpuInfoDetailed ParseCpu(IHardware hardware)
    {
        var info = new CpuInfoDetailed();
        foreach (var sensor in hardware.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Temperature:
                    info.Temperatures[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Clock:
                    info.Clocks[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Voltage:
                    info.Voltages[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Load:
                    info.Loads[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Power:
                    info.Powers[sensor.Name] = FormatValue(sensor.Value);
                    break;
            }
        }
        return info;
    }
    public CpuInfoDetailed GetCpuInfoDetailed()
    {
        return cpuInfo;
    }

    public class GpuInfoDetailed
    {
        public string Name { get; set; } = "";
        public Dictionary<string, string> Temperatures { get; set; } = new();
        public Dictionary<string, string> Clocks { get; set; } = new();
        public Dictionary<string, string> Loads { get; set; } = new();
        public Dictionary<string, string> Fans { get; set; } = new();
        public Dictionary<string, string> Powers { get; set; } = new();
        public Dictionary<string, string> Memory { get; set; } = new();
    }
    private GpuInfoDetailed ParseGpu(IHardware hardware)
    {
        var info = new GpuInfoDetailed();
        info.Name = hardware.Name;
        foreach (var sensor in hardware.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Temperature:
                    info.Temperatures[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Clock:
                    info.Clocks[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Load:
                    info.Loads[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Fan:
                    info.Fans[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Power:
                    info.Powers[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.SmallData:
                case SensorType.Data:
                    info.Memory[sensor.Name] = FormatValue(sensor.Value);
                    break;
            }
        }
        return info;
    }
    public GpuInfoDetailed GetGpuInfoDetailed()
    {
        return gpuInfo;
    }

    public class RamInfoDetailed
    {
        public Dictionary<string, string> Loads { get; set; } = new();
        public Dictionary<string, string> Clocks { get; set; } = new();
        public Dictionary<string, string> Data { get; set; } = new();
        public Dictionary<string, string> Temperatures { get; set; } = new();
    }
    private RamInfoDetailed ParseRam(IHardware hardware)
    {
        var info = new RamInfoDetailed();
        foreach (var sensor in hardware.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Load:
                    info.Loads[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Clock:
                    info.Clocks[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Data:
                case SensorType.SmallData:
                    info.Data[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Temperature:
                    info.Temperatures[sensor.Name] = FormatValue(sensor.Value);
                    break;
            }
        }
        return info;
    }
    public RamInfoDetailed GetRamInfoDetailed()
    {
        return ramInfo;
    }

    public class MotherboardInfoDetailed
    {
        public Dictionary<string, string> Temperatures { get; set; } = new();
        public Dictionary<string, string> Voltages { get; set; } = new();
        public Dictionary<string, string> Fans { get; set; } = new();
        public Dictionary<string, string> Others { get; set; } = new();
    }
    private MotherboardInfoDetailed ParseMotherboard(IHardware hardware)
    {
        var info = new MotherboardInfoDetailed();
        foreach (var sensor in hardware.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Temperature:
                    info.Temperatures[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Voltage:
                    info.Voltages[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Fan:
                    info.Fans[sensor.Name] = FormatValue(sensor.Value);
                    break;
                default:
                    info.Others[sensor.Name] = FormatValue(sensor.Value);
                    break;
            }
        }
        return info;
    }
    public MotherboardInfoDetailed GetMotherboardInfoDetailed()
    {
        return motherboardInfo;
    }

    public class DiskInfoDetailed
    {
        public string Name { get; set; } = "";
        public Dictionary<string, string> Temperatures { get; set; } = new();
        public Dictionary<string, string> Loads { get; set; } = new();
        public Dictionary<string, string> Data { get; set; } = new();
        public Dictionary<string, string> Others { get; set; } = new();
    }
    private DiskInfoDetailed ParseDisk(IHardware hardware)
    {
        var info = new DiskInfoDetailed();
        info.Name = hardware.Name;
        foreach (var sensor in hardware.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Temperature:
                    info.Temperatures[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Load:
                    info.Loads[sensor.Name] = FormatValue(sensor.Value);
                    break;
                case SensorType.Data:
                case SensorType.SmallData:
                    info.Data[sensor.Name] = FormatValue(sensor.Value);
                    break;
                default:
                    info.Others[sensor.Name] = FormatValue(sensor.Value);
                    break;
            }
        }
        return info;
    }
    public List<DiskInfoDetailed> GetAllDisksInfo()
    {
        return disksInfo;
    }

    private string FormatValue(float? value)
    {
        if (!value.HasValue || float.IsNaN(value.Value) || value.Value == 0)
            return "Нет данных";
        return value.Value.ToString("0.##");
    }

    public string GetCpuName()
    {
        foreach (var hardware in computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.Cpu)
            {
                if (!string.IsNullOrWhiteSpace(hardware.Name))
                    return hardware.Name;
            }
        }
        try
        {
            var searcher = new System.Management.ManagementObjectSearcher("select Name from Win32_Processor");
            foreach (var item in searcher.Get())
            {
                return item["Name"]?.ToString() ?? "Unknown CPU";
            }
        }
        catch { }
        return "Unknown CPU";
    }

    public string GetMotherboardName()
    {
        try
        {
            var searcher = new System.Management.ManagementObjectSearcher("SELECT Product, Manufacturer FROM Win32_BaseBoard");
            foreach (var item in searcher.Get())
            {
                string manufacturer = item["Manufacturer"]?.ToString() ?? "";
                string product = item["Product"]?.ToString() ?? "";
                return $"{manufacturer} {product}".Trim();
            }
        }
        catch { }
        return "Неизвестно";
    }

    public string GetRamModulesInfo()
    {
        try
        {
            var searcher = new System.Management.ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
            int count = 0;
            long totalPerModule = 0;
            foreach (var item in searcher.Get())
            {
                if (item["Capacity"] != null)
                {
                    long cap = Convert.ToInt64(item["Capacity"]);
                    totalPerModule = cap / (1024 * 1024 * 1024);
                    count++;
                }
            }
            if (count > 0 && totalPerModule > 0)
                return $"{totalPerModule}x{count} ГБ";
            if (count > 0)
                return $"{count} планок";
        }
        catch { }
        return "Неизвестно";
    }

    public List<Dictionary<string, string>> GetRamModulesFullInfo()
    {
        var modules = new List<Dictionary<string, string>>();
        try
        {
            var searcher = new System.Management.ManagementObjectSearcher("SELECT Manufacturer, PartNumber, SerialNumber, Capacity, Speed, ConfiguredClockSpeed, MemoryType FROM Win32_PhysicalMemory");
            foreach (var item in searcher.Get())
            {
                var dict = new Dictionary<string, string>();
                dict["Производитель"] = item["Manufacturer"]?.ToString() ?? "-";
                dict["Модель"] = item["PartNumber"]?.ToString() ?? "-";
                dict["Серийный номер"] = item["SerialNumber"]?.ToString() ?? "-";
                if (item["Capacity"] != null)
                {
                    long cap = Convert.ToInt64(item["Capacity"]);
                    dict["Объем"] = (cap / (1024 * 1024 * 1024)).ToString() + " ГБ";
                }
                else dict["Объем"] = "-";
                dict["Частота (Speed)"] = item["Speed"]?.ToString() ?? "-";
                dict["Настроенная частота (ConfiguredClockSpeed)"] = item["ConfiguredClockSpeed"]?.ToString() ?? "-";
                dict["Тип памяти"] = item["MemoryType"]?.ToString() ?? "-";
                modules.Add(dict);
            }
        }
        catch { }
        return modules;
    }

    public Dictionary<string, string> GetMotherboardFullInfo()
    {
        var info = new Dictionary<string, string>();
        try
        {
            var searcher = new System.Management.ManagementObjectSearcher("SELECT Manufacturer, Product, SerialNumber, Version FROM Win32_BaseBoard");
            foreach (var item in searcher.Get())
            {
                info["Производитель"] = item["Manufacturer"]?.ToString() ?? "-";
                info["Модель"] = item["Product"]?.ToString() ?? "-";
                info["Серийный номер"] = item["SerialNumber"]?.ToString() ?? "-";
                info["Версия"] = item["Version"]?.ToString() ?? "-";
            }
            var biosSearcher = new System.Management.ManagementObjectSearcher("SELECT SMBIOSBIOSVersion FROM Win32_BIOS");
            foreach (var item in biosSearcher.Get())
            {
                info["BIOS"] = item["SMBIOSBIOSVersion"]?.ToString() ?? "-";
            }
        }
        catch { }
        return info;
    }

    public class NetworkInfoDetailed
    {
        public Dictionary<string, float> DownloadSpeed { get; set; } = new();
        public Dictionary<string, float> UploadSpeed { get; set; } = new();
    }

    public NetworkInfoDetailed GetNetworkInfoDetailed()
    {
        return networkInfo;
    }
} 