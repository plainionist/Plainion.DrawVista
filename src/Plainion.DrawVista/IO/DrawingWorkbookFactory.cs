namespace Plainion.DrawVista.IO;

public class DrawingWorkbookFactory(string RootFolder)
{
    public IDrawingWorkbook TryCreate(string name)
    {
        if (Path.GetExtension(name).Equals(".png", StringComparison.OrdinalIgnoreCase))
        {
            return new DrawIOPngWorkbook(RootFolder, name);
        }
        else if (Path.GetExtension(name).Equals(".drawio", StringComparison.OrdinalIgnoreCase))
        {
            return new DrawIOWorkbook(RootFolder, name);
        }
        else
        {
            Console.WriteLine($"Unsupported file type: {name}");
            return null;
        }
    }
}
