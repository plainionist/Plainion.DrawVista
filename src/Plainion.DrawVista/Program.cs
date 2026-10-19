using Microsoft.Data.Sqlite;
using Plainion.DrawVista;
using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

InstanceOptions options;
try
{
    options = InstanceOptions.Parse(args);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    Console.Error.WriteLine("Usage: Plainion.DrawVista.exe -name <name> [-p <port>]");
    Environment.ExitCode = 1;
    return;
}

var builder = WebApplication.CreateBuilder(options.HostArguments);

var uriBuilder = new UriBuilder(builder.Configuration["HostUri"]);
if (options.Port is int port)
{
    uriBuilder.Port = port;
}
builder.WebHost.UseUrls(uriBuilder.Uri.AbsoluteUri);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var appData = options.DataFolder;

var inputFolder = options.InputFolder;
if (!Directory.Exists(inputFolder))
{
    Directory.CreateDirectory(inputFolder);
}

var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = Path.Combine(appData, "store.db")
}.ToString();
var store = new SqliteDocumentStore(connectionString);
store.Init();

var oldStore = new FileSystemDocumentStore(appData);
foreach (var name in oldStore.GetPageNames())
{
    var page = oldStore.GetPage(name);
    store.Save(page);
}
oldStore.Clear();

builder.Services.AddSingleton(options);
builder.Services.AddSingleton<IDocumentStore>(new DocumentStoreCachingDecorator(store));
builder.Services.AddSingleton(new DrawingWorkbookFactory(inputFolder));
builder.Services.AddSingleton<ISvgCaptionParser, SvgCaptionParser>();
builder.Services.AddSingleton<ISvgHyperlinkFormatter, SvgHyperlinkFormatter>();
builder.Services.AddSingleton<SvgProcessor>();
builder.Services.AddSingleton<FullTextSearch>();
builder.Services.AddSingleton<StartPage>();

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
    var allDocuments = new List<RawDocument>();

    foreach (var file in files)
    {
        using var stream = file.OpenReadStream();

        var workbook = factory.TryCreate(file.Name);
        if (workbook != null)
        {
            var documents = workbook.Load(stream);
            allDocuments.AddRange(documents);
        }
    }

    processor.Process(allDocuments);

    return "OK";
})
.DisableAntiforgery();

app.MapGet("/startPage", (HttpContext context, IDocumentStore store, string pageName) =>
{
    StartPage startPage = app.Services.GetService<StartPage>();
    
    context.Response.ContentType = "image/svg+xml";
    return startPage.Svg;
});
app.MapGet("/svg", (HttpContext context, IDocumentStore store, string pageName) =>
{
    if (string.IsNullOrEmpty(pageName))
    {
        throw new ArgumentException("pageName not specified");

    }
    context.Response.ContentType = "image/svg+xml";
    return store.GetPage(pageName).Content;
});

app.MapGet("/pageNames", (IDocumentStore store) =>
{
    return store.GetPageNames();
});

app.MapGet("/search", (FullTextSearch search, string text) =>
{
    return search.Search(text);
});

app.MapPost("/clear", (IDocumentStore store) =>
{
    store.Clear();

    return "OK";
});

app.Run();
