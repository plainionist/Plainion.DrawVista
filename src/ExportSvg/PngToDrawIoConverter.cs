
using System.Web;
using MetadataExtractor;

internal class PngToDrawIoConverter
{
    internal string Convert(string drawIOFile)
    {
        var tag = ImageMetadataReader.ReadMetadata(drawIOFile)
            .SelectMany(x => x.Tags)
            .Where(x => x.DirectoryName == "PNG-tEXt" && x.Name == "Textual Data")
            .SingleOrDefault();

        var mxFileContent = HttpUtility.UrlDecode(tag.Description).Substring("mxfile: ".Length);
        var file = Path.GetTempFileName() + ".drawio";
        File.WriteAllText(file, mxFileContent);
        return file;
    }
}
