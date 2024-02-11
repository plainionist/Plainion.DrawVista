using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.Environment.ContentRootPath = Path.GetDirectoryName(typeof(SvgProcessor).Assembly.Location);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

var outputFolder = Path.Combine(app.Environment.ContentRootPath, "store");
if (!Directory.Exists(outputFolder))
{
    Directory.CreateDirectory(outputFolder);
}

var drawIOFile = Path.Combine(app.Environment.ContentRootPath, "samples", "sample.drawio");

app.MapGet("/init", () =>
{
    var drawIoWorkbook = new DrawIOWorkbook(drawIOFile, outputFolder);

    var svgProcessor = new SvgProcessor(
        new SvgCaptionParser(),
        new SvgHyperlinkFormatter());

    svgProcessor.Process(drawIoWorkbook);

    return "OK";
});

app.MapGet("/svg", async (HttpContext context, string pageName) =>
{
    var fileName = Path.Combine(outputFolder, pageName + ".svg");
    context.Response.ContentType = "image/svg+xml";
    await context.Response.SendFileAsync(fileName);
});

app.Run();
