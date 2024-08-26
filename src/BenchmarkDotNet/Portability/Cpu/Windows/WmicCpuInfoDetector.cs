﻿using System.IO;
using BenchmarkDotNet.Helpers;

namespace BenchmarkDotNet.Portability.Cpu.Windows;

/// <summary>
/// CPU information from output of the `wmic cpu get Name, NumberOfCores, NumberOfLogicalProcessors /Format:List` command.
/// Windows only.
/// </summary>
internal class WmicCpuInfoDetector : ICpuInfoDetector
{
    private const string DefaultWmicPath = @"C:\Windows\System32\wbem\WMIC.exe";

    public bool IsApplicable() => RuntimeInformation.IsWindows();

    public CpuInfo? Detect()
    {
        if (!IsApplicable()) return null;

        const string argList = $"{WmicCpuInfoKeyNames.Name}, " +
                               $"{WmicCpuInfoKeyNames.NumberOfCores}, " +
                               $"{WmicCpuInfoKeyNames.NumberOfLogicalProcessors}, " +
                               $"{WmicCpuInfoKeyNames.MaxClockSpeed}";
        string wmicPath = File.Exists(DefaultWmicPath) ? DefaultWmicPath : "wmic";
        string wmicOutput = ProcessHelper.RunAndReadOutput(wmicPath, $"cpu get {argList} /Format:List");
        return WmicCpuInfoParser.Parse(wmicOutput);
    }
}