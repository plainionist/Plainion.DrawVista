using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public interface IDrawingWorkbook
{
    Task<IReadOnlyCollection<SvgDocument>> LoadAsync(Stream stream);
}
