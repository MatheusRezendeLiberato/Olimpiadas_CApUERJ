using System.Text.Json;
using Olimpiada.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Serviço em memória para armazenar resultados
var resultadosMemoria = new List<Resultado>();
var calculadora = new Calculadora_Pontos();
var modalidadesMemoria = new List<Modalidade>();

var app = builder.Build();

var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

static async Task<IResult> GetQuadroAsync(IWebHostEnvironment env)
{
    var filePath = Path.Combine(env.ContentRootPath, "Quadro.json");
    if (!File.Exists(filePath)) 
    {
        return Results.NotFound(new { message = "Quadro.json não encontrado." });
    }

    var json = await File.ReadAllTextAsync(filePath);
    var data = JsonSerializer.Deserialize<object>(json);
    return Results.Ok(data);
}

app.MapGet("/quadro", GetQuadroAsync)
   .WithName("GetQuadro");

// Endpoint para adicionar resultados, aceitando objeto único ou array
app.MapPost("/resultados", async (HttpContext http) =>
{
    using var document = await JsonDocument.ParseAsync(http.Request.Body);
    var requests = new List<ResultadoRequest>();

    if (document.RootElement.ValueKind == JsonValueKind.Array)
    {
        foreach (var element in document.RootElement.EnumerateArray())
        {
            var req = element.Deserialize<ResultadoRequest>(jsonOptions);
            if (req is not null) requests.Add(req);
        }
    }
    else if (document.RootElement.ValueKind == JsonValueKind.Object)
    {
        var req = document.RootElement.Deserialize<ResultadoRequest>(jsonOptions);
        if (req is not null) requests.Add(req);
    }
    else
    {
        return Results.BadRequest(new { erro = "Corpo inválido. Envie um objeto ou array JSON." });
    }

    if (requests.Count == 0)
    {
        return Results.BadRequest(new { erro = "Nenhum resultado válido encontrado no corpo da requisição." });
    }

    foreach (var req in requests)
    {
        if (!TryParseModalidade(req.Modalidade, out var modalidade))
        {
            return Results.BadRequest(new { erro = $"Modalidade '{req.Modalidade}' inválida." });
        }

        var resultado = new Resultado(req.Bandeira, modalidade, req.Colocacao, req.Quantidade);
        resultadosMemoria.Add(resultado);
    }

    return Results.Ok(new { mensagem = $"{requests.Count} resultado(s) adicionado(s)." });
})
.WithName("AddResultados");

// Endpoint para obter ranking com medalhas
app.MapGet("/ranking", () =>
{
    if (resultadosMemoria.Count == 0)
    {
        return Results.Ok(new List<MedalhaInfo>());
    }

    var ranking = calculadora.RankComMedalhas(resultadosMemoria);
    return Results.Ok(ranking);
})
.WithName("GetRanking");

// Endpoint para obter resultados cadastrados
app.MapGet("/resultados", () =>
{
    return Results.Ok(resultadosMemoria);
})
.WithName("ListResultados");

app.MapGet("/modalidades", () =>
{
    return Results.Ok(modalidadesMemoria);
});

// Endpoint para limpar resultados
app.MapDelete("/resultados", () =>
{
    var count = resultadosMemoria.Count;
    resultadosMemoria.Clear();
    return Results.Ok(new { mensagem = $"{count} resultado(s) removido(s)." });
})
.WithName("ClearResultados");
static bool TryParseModalidade(string raw, out TipoModalidade modalidade)
{
    modalidade = default;
    if (string.IsNullOrWhiteSpace(raw))
    {
        return false;
    }

    var value = raw.Trim();
    if (Enum.TryParse<TipoModalidade>(value, true, out modalidade))
    {
        return true;
    }

    var sportMapping = new Dictionary<string, TipoModalidade>(StringComparer.OrdinalIgnoreCase)
    {
        { "TenisDeMesa", TipoModalidade.Individual },
        { "Tênis de Mesa", TipoModalidade.Individual },
        { "Tenis de Mesa", TipoModalidade.Individual },
        { "Xadrez", TipoModalidade.Individual },

        { "TenisDeMesaDuplas", TipoModalidade.TenisDuplas },
        { "Tênis de Mesa Duplas", TipoModalidade.TenisDuplas },
        { "Tenis de Mesa Duplas", TipoModalidade.TenisDuplas },

        { "FutebolDe7", TipoModalidade.Coletiva1a4 },
        { "Futebol de 7", TipoModalidade.Coletiva1a4 },
        { "Futsal", TipoModalidade.Coletiva1a4 },
        { "Handebol", TipoModalidade.Coletiva1a4 },
        { "Basquete", TipoModalidade.Coletiva1a4 },
        { "Voleibol", TipoModalidade.Coletiva1a4 },
        { "Queimado", TipoModalidade.Coletiva1a4 },

        { "Bocha", TipoModalidade.Adaptadas },
        { "Boliche", TipoModalidade.Adaptadas },
        { "BolaAoAlvo", TipoModalidade.Adaptadas },
        { "Bola ao Alvo", TipoModalidade.Adaptadas },
        { "CambioSentado", TipoModalidade.Adaptadas },
        { "Câmbio Sentado", TipoModalidade.Adaptadas },

        { "FutsalMisto", TipoModalidade.Coletiva1a5 },
        { "HandebolMisto", TipoModalidade.Coletiva1a5 },
        { "QueimadoMisto", TipoModalidade.Coletiva1a5 },
        { "Handebol Misto", TipoModalidade.Coletiva1a5 },
        { "Queimado Misto", TipoModalidade.Coletiva1a5 },
    };

    if (sportMapping.TryGetValue(value, out modalidade))
    {
        return true;
    }

    var fallback = value.Replace("\u00BA", "").Replace("\u00AA", "").Replace("ª", "").Replace("º", "").Replace(" ", "");
    return sportMapping.TryGetValue(fallback, out modalidade);
}

app.Run();
