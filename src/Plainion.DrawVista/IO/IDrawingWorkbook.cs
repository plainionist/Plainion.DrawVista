using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public interface IDrawingWorkbook
{
    IReadOnlyCollection<SvgDocument> Load(Stream stream);
}