using Parcial1_P4_Yerferson.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<NumbersService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var numbersService = scope.ServiceProvider
        .GetRequiredService<NumbersService>();

    await numbersService.InitializeAsync();
}

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();