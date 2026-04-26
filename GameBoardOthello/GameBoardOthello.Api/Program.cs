using GameBoardOthello.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
// untuk dokumentasi API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Othello Game API",
        Version = "v1",
        Description = "RESTful API for Othello board game"
    });
});

// Register GameService as Singleton (in-memory session management)
builder.Services.AddSingleton<GameService>();

// Configure CORS untuk React frontend
//mengizinkan frontend React untuk bisa berkomunikasi dengan API ini
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")  // React dev servers
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if ( app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors("AllowReact");

app.UseAuthorization();

app.MapControllers();

app.Run();