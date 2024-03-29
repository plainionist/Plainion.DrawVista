using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

internal class DocumentBuilder
{
    public static ProcessedDocument Create(string pageName, params string[] captions) =>
        new(pageName,
            $"""
            <svg xmlns="http://www.w3.org/2000/svg">
            <g>
                {string.Join(Environment.NewLine, captions.Select(CreateElement))}
            </g>
            </svg>
            """,
            captions);

    private static string CreateElement(string caption) =>
        $"""
        <rect x="10" y="10" width="100" height="100" />
        <g transform="translate(-0.5 -0.5)">
            <switch>
                <foreignObject requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility">
                    <div xmlns="http://www.w3.org/1999/xhtml">
                        <div>
                            <div>{caption}</div>
                        </div>
                    </div>
                </foreignObject>
                <text>{caption}</text>
            </switch>
        </g>
        """;
}