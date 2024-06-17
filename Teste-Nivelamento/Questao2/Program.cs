using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Goals
{
    public int Team1Goals { get; set; }
    public int Team2Goals { get; set; }
}

public class ApiResponse
{
    public int Page { get; set; }
    public int Per_Page { get; set; }
    public int Total { get; set; }
    public int Total_Pages { get; set; }
    public Goals[] Data { get; set; }
}

class Program
{
    static async Task<ApiResponse> FetchMatchesAsync(HttpClient client, string url, int page)
    {
        using (var request = new HttpRequestMessage(HttpMethod.Get, $"{url}&page={page}"))
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse>(responseString);
                }

                return null;
            }
        }
    }

    static async Task<List<Goals>> FetchAllMatchesAsync(HttpClient client, string url)
    {
        var allMatches = new List<Goals>();

        // Primeiro, buscar a primeira página para obter o total de páginas
        var initialResponse = await FetchMatchesAsync(client, url, 1);
        if (initialResponse != null)
        {
            allMatches.AddRange(initialResponse.Data);
            int totalPages = initialResponse.Total_Pages;

            // Loop para buscar todas as páginas subsequentes
            for (int page = 2; page <= totalPages; page++)
            {
                var response = await FetchMatchesAsync(client, url, page);
                if (response != null)
                {
                    allMatches.AddRange(response.Data);
                }
            }
        }

        return allMatches;
    }

    static async Task<int> GetTotalGoalsAsync(string team, int year)
    {
        int total = 0;
        string encodedTeam = Uri.EscapeDataString(team);
        string baseUrl = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={encodedTeam}";

        using (var client = new HttpClient())
        {
            var team1Matches = await FetchAllMatchesAsync(client, baseUrl);
            total += team1Matches.Sum(g => g.Team1Goals);

            baseUrl = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team2={encodedTeam}";
            var team2Matches = await FetchAllMatchesAsync(client, baseUrl);
            total += team2Matches.Sum(g => g.Team2Goals);
        }

        return total;
    }

    static async Task Main(string[] args)
    {
        Console.WriteLine($"Verificando resultados para Paris Saint-Germain em 2013, aguarde");
        int totalGoals = await GetTotalGoalsAsync("Paris Saint-Germain", 2013);
        Console.WriteLine($"Team Paris Saint - Germain scored {totalGoals} goals in 2013");
        Console.WriteLine();
        Console.WriteLine($"Verificando resultados para Chelsea em 2014, aguarde");
        totalGoals = await GetTotalGoalsAsync("Chelsea", 2014);
        Console.WriteLine($"Team Chelsea scored {totalGoals} goals in 2014");
    }
}
