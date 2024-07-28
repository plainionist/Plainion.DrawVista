using Newtonsoft.Json;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class FileSystemDocumentStore : IDocumentStore
{
    private readonly string myRootFolder;
    private readonly object myLock = new();

    public event EventHandler DocumentsChanged;

    public FileSystemDocumentStore(string appData) {
        myRootFolder = Path.Combine(appData, GlobalConst.StoreDirName);
        if (!Directory.Exists(myRootFolder))
        {
            Directory.CreateDirectory(myRootFolder);
        }
    }

    public IReadOnlyCollection<string> GetPageNames()
    {
        lock (myLock)
        {
            return Directory.GetFiles(myRootFolder, "*.svg")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
        }
    }

    public ProcessedDocument GetPage(string pageName)
    {
        lock (myLock)
        {
            var content = File.ReadAllText(ContentFile(pageName));
            dynamic meta = JsonConvert.DeserializeObject<MetaContent>(File.ReadAllText(MetaFile(pageName)));
            return new ProcessedDocument(pageName, content, meta.Captions);
        }
    }

    private record MetaContent(List<string> Captions);

    private string MetaFile(string pageName) => Path.Combine(myRootFolder, pageName + ".svg.meta");
    private string ContentFile(string pageName) => Path.Combine(myRootFolder, pageName + ".svg");

    public void Save(ProcessedDocument document)
    {
        lock (myLock)
        {
            File.WriteAllText(ContentFile(document.Name), document.Content);
            var meta = new MetaContent(document.Captions.ToList());
            File.WriteAllText(MetaFile(document.Name), JsonConvert.SerializeObject(meta));
        }

        DocumentsChanged?.Invoke(this, null);
    }

    public void Clear()
    {
        lock (myLock)
        {
            foreach (var file in Directory.GetFiles(myRootFolder))
            {
                File.Delete(file);
            }
        }
    }

    public void Init()
    {
        if (!Directory.Exists(myRootFolder))
        {
            Directory.CreateDirectory(myRootFolder);
        }
    }
}
