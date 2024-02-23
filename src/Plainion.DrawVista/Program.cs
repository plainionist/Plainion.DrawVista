using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

var port = 5236;
if (args.Length > 0)
{
    if (args[0] == "-p")
    {
        port = Convert.ToInt32(args[1]);
    }
}

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var appData = Path.Combine(Environment.GetEnvironmentVariable("ALLUSERSPROFILE"), "Plainion.DrawVista");

var inputFolder = Path.Combine(appData, "input");
if (!Directory.Exists(inputFolder))
{
    Directory.CreateDirectory(inputFolder);
}

var outputFolder = Path.Combine(appData, "store");
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

app.MapPost("/upload", (DrawingWorkbookFactory factory, SvgProcessor processor, IFormFileCollection files) =>
{
    var allDocuments = new List<SvgDocument>();

    foreach (var file in files)
    {
        Console.WriteLine($"Processing '{file.Name}' ...");

        using var stream = file.OpenReadStream();

        var workbook = factory.Create(file.Name);
        var documents = workbook.Load(stream);

        allDocuments.AddRange(documents);
    }

    processor.Process(allDocuments);

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

app.MapPost("/clear", (IDocumentStore store) =>
{
    store.Clear();

    return "OK";
});

app.Run();
