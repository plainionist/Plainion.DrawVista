using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var contentRootPath = Path.GetDirectoryName(typeof(SvgProcessor).Assembly.Location); ;

var inputFolder = Path.Combine(contentRootPath, "input");
if (!Directory.Exists(inputFolder))
{
    Directory.CreateDirectory(inputFolder);
}
var outputFolder = Path.Combine(contentRootPath, "store");
if (!Directory.Exists(outputFolder))
{
    Directory.CreateDirectory(outputFolder);
}

builder.Services.AddSingleton<IDocumentStore>(new DocumentStore(outputFolder));
builder.Services.AddSingleton(new DrawingWorkbookFactory(inputFolder));
builder.Services.AddSingleton<ISvgCaptionParser, SvgCaptionParser>();
builder.Services.AddSingleton<ISvgHyperlinkFormatter, SvgHyperlinkFormatter>();
builder.Services.AddSingleton<SvgProcessor>();

var app = builder.Build();
app.Environment.ContentRootPath = contentRootPath;

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

// concept: one upload considered as one scope so one workbook
app.MapPost("/upload", (DrawingWorkbookFactory factory, SvgProcessor processor, IFormFileCollection files) =>
{
    var allDocuments = new List<SvgDocument>();

    foreach (var file in files)
    {
        using var stream = file.OpenReadStream();

        var workbook = factory.Create(file.Name);
        var documents = workbook.Load(file.Name, stream);

        allDocuments.AddRange(documents);
    }

    processor.Process(allDocuments);

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
