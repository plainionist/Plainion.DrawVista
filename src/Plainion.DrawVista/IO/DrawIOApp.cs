using System.Diagnostics;

namespace Plainion.DrawVista.IO;

internal class DrawIOApp
{
    private static readonly string Executable = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
         "draw.io", "draw.io.exe");

    internal static void ExtractSvg(string file, string svgFile, int pageIndex)
    {
        Console.WriteLine($"draw.io.exe -x {file} -o {svgFile} -p {pageIndex}");
        
        Process.Start(Executable, $"-x {file} -o {svgFile} -p {pageIndex}")
            .WaitForExit();
   }
}