using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public interface IDrawingWorkbook
{
    IReadOnlyCollection<RawDocument> Load(Stream stream);
}
