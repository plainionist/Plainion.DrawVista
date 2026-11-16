using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

namespace Plainion.DrawVista.IO;

internal class DrawIOApp(DrawIOModel Model, string RootFolder) : IDisposable
{
    private static readonly string Executable = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
         "draw.io", "draw.io.exe");

    private string myFile;

    public string ExtractSvg(int pageIndex)
    {
        SaveModelOnDemand();
        var svgFile = Path.Combine(RootFolder, $"{Guid.NewGuid():N}.svg");
        try
        {
            var startInfo = new ProcessStartInfo(Executable)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            foreach (var argument in new[] { "-x", myFile, "-o", svgFile, "-p", pageIndex.ToString(CultureInfo.InvariantCulture) })
            {
                startInfo.ArgumentList.Add(argument);
            }

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Could not start the draw.io exporter.");
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            var outputText = output.GetAwaiter().GetResult();
            var errorText = error.GetAwaiter().GetResult();
            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"Draw.io SVG export failed (exit {process.ExitCode}): {errorText} {outputText}");
            }
            if (!File.Exists(svgFile))
            {
                throw new InvalidDataException("Draw.io SVG export produced no output.");
            }
            return Model.AddLayerMetadata(pageIndex, XElement.Load(svgFile));
        }
        finally
        {
            if (File.Exists(svgFile))
            {
                File.Delete(svgFile);
            }
        }
    }

    private void SaveModelOnDemand()
    {
        if (myFile != null)
        {
            return;
        }

        // extension ".drawio" is important so that draw.io.exe detects file contents properly
        myFile = Path.Combine(RootFolder, $"{Guid.NewGuid():N}.drawio");
        
        Model.ForExport().WriteTo(myFile);
    }

    public void Dispose()
    {
        if (File.Exists(myFile))
        {
            File.Delete(myFile);
        }
    }
}