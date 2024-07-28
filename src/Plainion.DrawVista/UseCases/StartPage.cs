using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;
using Rubjerg.Graphviz;

public class StartPage {

    private record StartPageNode(string Id, ICollection<string> Children);
    private ISvgCaptionParser myCaptionParser;
    private IDocumentStore myStore;
    private string myStartPageDir;

    public string Svg { get; private set; }

    public StartPage(IDocumentStore store, ISvgCaptionParser captionParser)
    {
        var appData = Path.Combine(Environment.GetEnvironmentVariable("ALLUSERSPROFILE"), GlobalConst.AppFolder);
        myStartPageDir = Path.Combine(appData, GlobalConst.InputDirName, "StartPage");
        myCaptionParser = captionParser;
        myStore = store;
        store.DocumentsChanged += OnDocumentsChanged;
        CreateGraph();
    }

    private void OnDocumentsChanged(object sender, EventArgs e)
    {
        CreateGraph();
    }

    private void CreateGraph() {
        List<ProcessedDocument> existingDocuments = myStore.GetPageNames()
            .Select(myStore.GetPage)
            .ToList();

        RootGraph startPageGraph = RootGraph.CreateNew(GraphType.Directed, "StartPage");

        foreach(ProcessedDocument document in existingDocuments)
        {
            XElement xml = XElement.Parse(document.Content);
            IReadOnlyCollection<Caption> captions = myCaptionParser.Parse(xml);
            string GetPageReference(string name) =>
                existingDocuments.FirstOrDefault(p => p.Name.Replace(" ", "").Equals(name, StringComparison.OrdinalIgnoreCase))?.Name;

            bool IsSelfReference(string name) =>
                name.Replace(" ", "").Equals(document.Name, StringComparison.OrdinalIgnoreCase);
            
            Node parent = startPageGraph.GetOrAddNode(document.Name);
            parent.SetAttribute("shape", "rect");

            foreach (Caption caption in captions)
            {
                string reference = GetPageReference(caption.DisplayText);

                if (reference is null || IsSelfReference(caption.DisplayText))
                {
                    continue;
                }

                Node child = startPageGraph.GetOrAddNode(reference);
                child.SetAttribute("shape", "rect");
                startPageGraph.GetOrAddEdge(parent, child, $"{parent} - {child}");
            }
        }

        Svg = LinkNodes(startPageGraph.ToSvgString());

        if (!Directory.Exists(myStartPageDir))
        {
            Directory.CreateDirectory(myStartPageDir);
        }
        startPageGraph.ToSvgFile(Path.Combine(myStartPageDir, "test.svg"));
    }

    private string LinkNodes(string startPageSvgString)
    {
        string replacement = @"$1 style=""display: inline-block;font-size: 12px;font-family: Helvetica;fill: blue;line-height: 1.2;pointer-events: all;white-space: normal;overflow-wrap: normal;text-decoration: underline;cursor: pointer"" onclick=""window.hook.navigate('$2')"">$2$3";
        string pattern = @"(<text.*)>(.*)(<\/text>)";
        string result = Regex.Replace(startPageSvgString, pattern, replacement);
        return result;
    }
}