using System.Diagnostics;

namespace Plainion.DrawVista.IO;

internal class DrawIOApp(DrawIOModel Model) : IDisposable
{
    private static readonly string Executable = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
         "draw.io", "draw.io.exe");

    private string myFile;

    public void ExtractSvg(int pageIndex, string svgFile)
    {
        SaveModelOnDemand();

        // Console.WriteLine($"draw.io.exe -x {myFile} -o {svgFile} -p {pageIndex}");

        Process.Start(Executable, $"-x {myFile} -o \"{svgFile}\" -p {pageIndex}")
            .WaitForExit();
    }

    private void SaveModelOnDemand()
    {
        if (myFile != null)
        {
            return;
        }

        // extension ".drawio" is important so that draw.io.exe detects file contents properly
        myFile = Path.GetTempFileName() + ".drawio";
        
        Model.WriteTo(myFile);
    }

    public void Dispose()
    {
        if (File.Exists(myFile))
        {
            File.Delete(myFile);
        }
    }
}