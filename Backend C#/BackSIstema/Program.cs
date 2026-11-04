using BackBLL;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuramos CORS primero
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

// Leemos la cadena y registramos BLL
string ConexionSQL = builder.Configuration.GetConnectionString("Conexion");
builder.Services.AddScoped(x => new PedidoBLL(ConexionSQL));
builder.Services.AddScoped(x => new UsuarioBLL(ConexionSQL));

builder.Services.AddOpenApi();

var app = builder.Build();

// 2. ACTIVAR CORS ACÁ, PRIMERO QUE TODO EN EL PIPELINE
app.UseCors("PermitirTodo");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();