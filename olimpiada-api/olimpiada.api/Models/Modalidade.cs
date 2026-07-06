namespace Olimpiada.Api.Models;

public class Modalidade
{
    public int IdModalidade { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoModalidade Tipo { get; set; }
    public int Pontuacao1 { get; set; }
    public int Pontuacao2 { get; set; }
    public int Pontuacao3 { get; set; }
    public int Pontuacao4 { get; set; }
    public int QuantidadeParticipantes { get; set; }
}   