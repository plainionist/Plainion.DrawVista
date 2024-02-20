using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

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
app.UseCors(builder =>
    builder.WithOrigins("http://localhost:8080")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

var outputFolder = Path.Combine(app.Environment.ContentRootPath, "store");
if (!Directory.Exists(outputFolder))
{
    Directory.CreateDirectory(outputFolder);
}

app.MapPost("/upload", async (IFormFileCollection files) =>
{
    foreach (var file in files)
    {
        Console.WriteLine($"IMPORT: {file.Name}");

        var tempFile = Path.GetTempFileName();
        try
        {
            using (var stream = File.OpenWrite(tempFile))
            {
                await file.CopyToAsync(stream);
            }

            var svgProcessor = new SvgProcessor(
                new SvgCaptionParser(),
                new SvgHyperlinkFormatter());

            var drawIoWorkbook = new DrawIOWorkbook(tempFile, outputFolder);
            svgProcessor.Process(drawIoWorkbook);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    return "OK";
})
.DisableAntiforgery();

app.MapGet("/svg", async (HttpContext context, string pageName) =>
{
    var fileName = Path.Combine(outputFolder, pageName + ".svg");
    context.Response.ContentType = "image/svg+xml";
    await context.Response.SendFileAsync(fileName);
});

app.MapGet("/allFiles", () =>
{
    return Directory.GetFiles(outputFolder, "*.svg")
        .Select(file => new
        {
            id = Path.GetFileNameWithoutExtension(file).ToLower(),
            content = File.ReadAllText(file)
        })
        .ToList();
});

app.Run();
