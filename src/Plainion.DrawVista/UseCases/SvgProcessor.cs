using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

public class SvgProcessor(ISvgCaptionParser parser, ISvgHyperlinkFormatter formatter, IDocumentStore store)
{
    private readonly ISvgCaptionParser myParser = parser;
    private readonly ISvgHyperlinkFormatter myFormatter = formatter;
    private readonly IDocumentStore myStore = store;

    /// <summary>
    /// Processes existing and newly uploaded documents.
    /// </summary>
    public void Process(IReadOnlyCollection<RawDocument> documents)
    {
        var existingDocuments = myStore.GetPageNames()
            .Where(x => !documents.Any(y => y.Name.Equals(x, StringComparison.OrdinalIgnoreCase)))
            .Select(x => ParsedDocument.Create(myParser, myStore.GetPage(x)))
            .ToList();

        var knownPageNames = documents.Select(x => x.Name)
            .Concat(existingDocuments.Select(x => x.Name))
            .ToList();

        var parsedDocuments = documents
            .Select(x => ParsedDocument.Create(myParser, x))
            .Concat(existingDocuments);

        foreach (var doc in parsedDocuments)
        {
            AddLinks(knownPageNames, doc);
            ApplyStyleToExistingLinks(doc);
            myStore.Save(doc.ToProcessedDocument());
        }
    }

    private record ParsedDocument(string Name, XElement Content, IReadOnlyCollection<Caption> Captions)
    {
        public static ParsedDocument Create(ISvgCaptionParser parser, RawDocument document)
        {
            var xml = XElement.Parse(document.Content);
            var captions = parser.Parse(xml);
            return new(document.Name, xml, captions);
        }

        public static ParsedDocument Create(ISvgCaptionParser parser, ProcessedDocument document)
        {
            var xml = XElement.Parse(document.Content);
            var captions = parser.Parse(xml);
            return new(document.Name, xml, captions);
        }

        public ProcessedDocument ToProcessedDocument() =>
            new(Name, Content.ToString(), Captions.Select(x => x.DisplayText).ToList());
    }

    private void AddLinks(IReadOnlyCollection<string> pages, ParsedDocument document)
    {
        string GetPageReference(string name) =>
            pages.FirstOrDefault(p => p.Replace(" ", "").Equals(name, StringComparison.OrdinalIgnoreCase));

        bool IsSelfReference(string name) =>
            name.Replace(" ", "").Equals(document.Name, StringComparison.OrdinalIgnoreCase);

        foreach (var caption in document.Captions)
        {
            var reference = GetPageReference(caption.DisplayText);

            if (reference is null || IsSelfReference(caption.DisplayText))
            {
                continue;
            }

            Console.WriteLine($"Creating link for: {reference}");

            var onClickAttr = caption.Element.Attribute("onclick");
            if (onClickAttr == null)
            {
                onClickAttr = new XAttribute("onclick", string.Empty);
                caption.Element.Add(onClickAttr);
            }
            onClickAttr.Value = $"window.hook.navigate('{reference}')";

            myFormatter.ApplyStyle(caption.Element, isExternal: false);
        }

        document.Content.Attribute("width").Value = "100%";
    }

    // In DrawIO external links can be provided but those are neither in draw.io
    // nor in SVG visualized as links (e.g. blue and underlined) - so let's apply some style
    private void ApplyStyleToExistingLinks(ParsedDocument document)
    {
        var existingLinks = document.Content
            .Descendants()
            .Where(x => x.EqualsTagName("a"))
            .SelectMany(x => x.Descendants()
                .Where(x => x.IsMostInnerDiv()))
           .ToList();

        foreach (var link in existingLinks)
        {
            myFormatter.ApplyStyle(link, isExternal: true);
        }
    }
}