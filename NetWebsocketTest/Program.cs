

using NetWebsocketTest.Infrastructure;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.None,
    //Json serialization as camelCase for flutter ( "myEntity" : "this is value" )
    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
    Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() },
    NullValueHandling = NullValueHandling.Ignore,
};

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
WebSocketRegistator.Register(builder.Services); 

var app = builder.Build();
app.UseWebSockets();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();


