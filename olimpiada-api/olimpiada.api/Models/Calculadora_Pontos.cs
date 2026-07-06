using System;
using System.Collections.Generic;
using System.Linq;

namespace Olimpiada.Api.Models;

public enum TipoModalidade
{
    Individual = 1,
    Coletiva1a4 = 2,
    Coletiva1a5 = 3,
    TenisDuplas = 4,
    Adaptadas = 5
}

public record Resultado(string Bandeira, TipoModalidade Modalidade, int Colocacao, int Quantidade = 1);

public record PontuacaoBandeira(string Bandeira, int Pontos);

public record MedalhaInfo(string Bandeira, int Ouro, int Prata, int Bronze, int Pontos, int Posicao);

/// <summary>
/// Calculadora de pontos para as modalidades descritas.
/// Use <see cref="AggregatePoints"/> para agregar uma lista de resultados e
/// <see cref="Rank"/> para obter as bandeiras ordenadas por pontos.
/// </summary>
public class Calculadora_Pontos
{
    private static readonly Dictionary<TipoModalidade, int[]> PointsByModalidade = new()
    {
        { TipoModalidade.Individual, new[] { 4, 3, 2, 1 } },
        { TipoModalidade.Coletiva1a4, new[] { 16, 13, 10, 7 } },
        { TipoModalidade.Coletiva1a5, new[] { 12, 10, 8, 6 } },
        { TipoModalidade.TenisDuplas, new[] { 8, 6, 4, 2 } },
        { TipoModalidade.Adaptadas, new[] { 16, 13, 10, 7 } }
    };

    public int GetPointsForPlacement(TipoModalidade modalidade, int colocacao)
    {
        if (colocacao < 1 || colocacao > 4) return 0;
        var table = PointsByModalidade[modalidade];
        return table[colocacao - 1];
    }

    /// <summary>
    /// Agrega os pontos por bandeira a partir de uma lista de resultados.
    /// Cada resultado pode conter uma quantidade (por exemplo: 2 resultados iguais).
    /// </summary>
    public Dictionary<string, int> AggregatePoints(IEnumerable<Resultado> resultados)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in resultados)
        {
            var pts = GetPointsForPlacement(r.Modalidade, r.Colocacao) * Math.Max(1, r.Quantidade);
            if (!map.ContainsKey(r.Bandeira)) map[r.Bandeira] = 0;
            map[r.Bandeira] += pts;
        }

        return map;
    }

    /// <summary>
    /// Agrega medalhas (ouro, prata, bronze) por bandeira.
    /// </summary>
    public Dictionary<string, (int Ouro, int Prata, int Bronze)> AggregateMedalhas(IEnumerable<Resultado> resultados)
    {
        var map = new Dictionary<string, (int, int, int)>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in resultados)
        {
            if (!map.ContainsKey(r.Bandeira))
                map[r.Bandeira] = (0, 0, 0);

            var (ouro, prata, bronze) = map[r.Bandeira];

            if (r.Colocacao == 1) ouro += r.Quantidade;
            else if (r.Colocacao == 2) prata += r.Quantidade;
            else if (r.Colocacao == 3) bronze += r.Quantidade;

            map[r.Bandeira] = (ouro, prata, bronze);
        }

        return map;
    }

    /// <summary>
    /// Retorna as bandeiras ordenadas por pontos decrescentes (e por nome como tiebreaker).
    /// </summary>
    public List<PontuacaoBandeira> Rank(IEnumerable<Resultado> resultados)
    {
        var aggregated = AggregatePoints(resultados);
        return aggregated
            .Select(kv => new PontuacaoBandeira(kv.Key, kv.Value))
            .OrderByDescending(p => p.Pontos)
            .ThenBy(p => p.Bandeira, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Retorna ranking completo com bandeira, medalhas, pontos e posição.
    /// </summary>
    public List<MedalhaInfo> RankComMedalhas(IEnumerable<Resultado> resultados)
    {
        var resultadosList = resultados.ToList();
        var pontos = AggregatePoints(resultadosList);
        var medalhas = AggregateMedalhas(resultadosList);

        var ranking = pontos
            .Select(kv => new MedalhaInfo(
                kv.Key,
                medalhas.TryGetValue(kv.Key, out var m) ? m.Ouro : 0,
                medalhas.TryGetValue(kv.Key, out var m2) ? m2.Prata : 0,
                medalhas.TryGetValue(kv.Key, out var m3) ? m3.Bronze : 0,
                kv.Value,
                0 // posição será preenchida depois
            ))
            .OrderByDescending(x => x.Pontos)
            .ThenBy(x => x.Bandeira, StringComparer.OrdinalIgnoreCase)
            .Select((item, index) => item with { Posicao = index + 1 })
            .ToList();

        return ranking;
    }
}
