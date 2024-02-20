namespace Plainion.DrawVista.IO;

public class DrawingWorkbookFactory(string RootFolder)
{
    public IDrawingWorkbook Create(string fileName)
    {
        if (Path.GetExtension(fileName).Equals(".png", StringComparison.OrdinalIgnoreCase))
        {
            return new DrawIOPngWorkbook(RootFolder);
        }
        else
        {
            return new DrawIOWorkbook(RootFolder);
        }
    }
}
