namespace ExportSVG.UseCases;

public interface IDrawingWorkbook
{
    IReadOnlyList<string> ReadPages();
    SvgDocument Export(int pageIndex, string pageName);
    void Save(SvgDocument svgDocument);
}