using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var contentRootPath = Path.GetDirectoryName(typeof(SvgProcessor).Assembly.Location);

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

app.MapPost("/upload", (SvgProcessor processor, IFormFileCollection files) =>
{
    foreach (var file in files)
    {
        Console.WriteLine($"IMPORT: {file.Name}");

        var workbook = new DrawIOWorkbook(inputFolder);

        using var stream = file.OpenReadStream();
        var documents = workbook.Load(stream);
        processor.Process(documents);
    }

    return "OK";
})
.DisableAntiforgery();

app.MapGet("/svg", async (HttpContext context, IDocumentStore store, string pageName) =>
{
    context.Response.ContentType = "image/svg+xml";
    await context.Response.SendFileAsync(store.GetFileName(pageName));
});

app.MapGet("/allFiles", (IDocumentStore store) =>
{
    return store.GetAllFiles()
        .Select(doc => new
        {
            id = doc.Name.ToLower(),
            content = doc.Content
        })
        .ToList();
});

app.Run();
