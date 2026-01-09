using Frontman.Domain;
using Frontman.Features.Serve;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ReleaseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use versioned static files
var contentRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
app.UseVersionedStaticFiles(contentRoot);

app.Run();
