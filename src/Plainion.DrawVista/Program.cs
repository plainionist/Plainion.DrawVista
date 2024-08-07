﻿using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

var port = -1;
if (args.Length > 0)
{
    if (args[0] == "-p")
    {
        port = Convert.ToInt32(args[1]);
    }
}

var builder = WebApplication.CreateBuilder(args);

var uriBuilder = new UriBuilder(builder.Configuration["HostUri"]);
if (port > -1)
{
    uriBuilder.Port = port;
}
builder.WebHost.UseUrls(uriBuilder.Uri.AbsoluteUri);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var appData = Path.Combine(Environment.GetEnvironmentVariable("ALLUSERSPROFILE"), GlobalConst.AppFolder);

var inputFolder = Path.Combine(appData, GlobalConst.InputDirName);
if (!Directory.Exists(inputFolder))
{
    Directory.CreateDirectory(inputFolder);
}

var store = new SqliteDocumentStore($"Data Source={appData}\\store.db");
store.Init();

var oldStore = new FileSystemDocumentStore(appData);
foreach (var name in oldStore.GetPageNames())
{
    var page = oldStore.GetPage(name);
    store.Save(page);
}
oldStore.Clear();

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
