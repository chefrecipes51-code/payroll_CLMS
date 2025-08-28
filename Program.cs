
var builder = WebApplication.CreateBuilder(args);

// Access configuration
var configuration = builder.Configuration;

// Register database connection with correct configuration reference
builder.Services.AddSingleton<IDbConnection>(sp =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new SqlConnection(connectionString);
});



builder.Services.AddAuthorization();

// Register repository service
builder.Services.AddScoped<WageGradeMasterServiceRepository>();

// Add controllers
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Add this line to use authentication
app.UseAuthorization();

app.MapControllers();

app.Run();