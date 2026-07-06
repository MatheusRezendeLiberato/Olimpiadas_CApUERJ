namespace Olimpiada.Api.Models;

public class ResultadoRequest
{
    public string Bandeira { get; set; } = string.Empty;
    public string Modalidade { get; set; } = string.Empty;
    public int Colocacao { get; set; }
    public int Quantidade { get; set; } = 1;
}
