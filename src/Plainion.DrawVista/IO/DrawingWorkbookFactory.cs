namespace Plainion.DrawVista.IO;

public class DrawingWorkbookFactory(string RootFolder)
{
    public IDrawingWorkbook Create(string name)
    {
        if (Path.GetExtension(name).Equals(".png", StringComparison.OrdinalIgnoreCase))
        {
            return new DrawIOPngWorkbook(RootFolder, name);
        }
        else
        {
            return new DrawIOWorkbook(RootFolder, name);
        }
    }
}
