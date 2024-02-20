using System.Xml.Linq;
using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.UseCases;
using Moq;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class SvgProcessorTests
{
    private const string SvgDocument = """
        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" width="204px" height="464px" viewBox="-0.5 -0.5 204 464">
        <defs />
        <g>
            <path d="M 101 281 L 101 347.9" fill="none" stroke="#000000" stroke-width="3" stroke-miterlimit="10" pointer-events="stroke" />
            <path d="M 101 357.65 L 94.5 344.65 L 101 347.9 L 107.5 344.65 Z" fill="#000000" stroke="#000000" stroke-width="3" stroke-miterlimit="10" pointer-events="all" />
            <rect x="1" y="181" width="200" height="100" rx="15" ry="15" fill="#fff2cc" stroke="#d6b656" stroke-width="3" pointer-events="all" />
            <g transform="translate(-0.5 -0.5)">
            <switch>
                <foreignObject pointer-events="none" width="100%" height="100%" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility" style="overflow: visible; text-align: left;">
                <div xmlns="http://www.w3.org/1999/xhtml" style="display: flex; align-items: unsafe center; justify-content: unsafe center; width: 198px; height: 1px; padding-top: 231px; margin-left: 2px;">
                    <div style="box-sizing: border-box; font-size: 0px; text-align: center;">
                    <div style="display: inline-block; font-size: 20px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; pointer-events: all; font-weight: bold; white-space: normal; overflow-wrap: normal;">Processor</div>
                    </div>
                </div>
                </foreignObject>
                <text x="101" y="237" fill="#000000" font-family="Helvetica" font-size="20px" text-anchor="middle" font-weight="bold">Processor</text>
            </switch>
            </g>
            <path d="M 101 101 L 101 170.9" fill="none" stroke="#000000" stroke-width="3" stroke-miterlimit="10" pointer-events="stroke" />
            <path d="M 101 177.65 L 96.5 168.65 L 101 170.9 L 105.5 168.65 Z" fill="#000000" stroke="#000000" stroke-width="3" stroke-miterlimit="10" pointer-events="all" />
            <rect x="1" y="1" width="200" height="100" rx="15" ry="15" fill="#dae8fc" stroke="#6c8ebf" stroke-width="3" pointer-events="all" />
            <g transform="translate(-0.5 -0.5)">
            <switch>
                <foreignObject pointer-events="none" width="100%" height="100%" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility" style="overflow: visible; text-align: left;">
                <div xmlns="http://www.w3.org/1999/xhtml" style="display: flex; align-items: unsafe center; justify-content: unsafe center; width: 198px; height: 1px; padding-top: 51px; margin-left: 2px;">
                    <div style="box-sizing: border-box; font-size: 0px; text-align: center;">
                    <div style="display: inline-block; font-size: 20px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; pointer-events: all; font-weight: bold; white-space: normal; overflow-wrap: normal;">Parser</div>
                    </div>
                </div>
                </foreignObject>
                <text x="101" y="57" fill="#000000" font-family="Helvetica" font-size="20px" text-anchor="middle" font-weight="bold">Parser</text>
            </switch>
            </g>
            <rect x="1" y="361" width="200" height="100" rx="15" ry="15" fill="#d5e8d4" stroke="#82b366" stroke-width="3" pointer-events="all" />
            <g transform="translate(-0.5 -0.5)">
            <switch>
                <foreignObject pointer-events="none" width="100%" height="100%" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility" style="overflow: visible; text-align: left;">
                <div xmlns="http://www.w3.org/1999/xhtml" style="display: flex; align-items: unsafe center; justify-content: unsafe center; width: 198px; height: 1px; padding-top: 411px; margin-left: 2px;">
                    <div style="box-sizing: border-box; font-size: 0px; text-align: center;">
                    <div style="display: inline-block; font-size: 20px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; pointer-events: all; font-weight: bold; white-space: normal; overflow-wrap: normal;">Processor</div>
                    </div>
                </div>
                </foreignObject>
                <text x="101" y="417" fill="#000000" font-family="Helvetica" font-size="20px" text-anchor="middle" font-weight="bold">Processor</text>
            </switch>
            </g>
        </g>
        <switch>
            <g requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility" />
            <a transform="translate(0,-5)" xlink:href="https://www.diagrams.net/doc/faq/svg-export-text-problems" target="_blank">
            <text text-anchor="middle" font-size="10px" x="50%" y="100%">Viewer does not support full SVG 1.1</text>
            </a>
        </switch>
        </svg>
    """;

    [Test]
    public void LinksShouldBeAddedForExistingPage()
    {
        var systemPage = new SvgDocument("System", XElement.Parse(SvgDocument));
        var parserPage = new SvgDocument("Parser", new XElement("doc", new XAttribute("width", "100%")));

        var svgProcessor = new SvgProcessor(new SvgCaptionParser(), new SvgHyperlinkFormatter(), Mock.Of<IDocumentStore>());

        svgProcessor.Process([systemPage, parserPage]);

        var parserElement = systemPage.Content.Descendants()
            .Single(x => x.Elements().Count() == 0 && x.Name.LocalName == "div" && x.Value == "Parser");
        Assert.That(parserElement.Attribute("onclick"), Is.Not.Null);
    }
}
