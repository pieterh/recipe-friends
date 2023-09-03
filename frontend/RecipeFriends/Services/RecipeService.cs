using System;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using RecipeFriends.Shared.DTO.v1;

namespace RecipeFriends.Services;

public class RecipeService : IRecipeService
{
    private readonly HttpClient _httpClient;
    public RecipeService(HttpClient httpClient)
    {
        if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
        _httpClient = httpClient;
    }

    async Task<RecipeInfo[]> IRecipeService.GetRecipesAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"api/v1/recipes", cancellationToken);
        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                {
                    var stationInfoResponse = await response.Content.ReadFromJsonAsync<RecipeInfo[]>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);
                    return stationInfoResponse;
                }
            case System.Net.HttpStatusCode.Unauthorized:
                throw new Exception("Unauthorized!");
            default:
                throw new Exception($"Uhh {response.StatusCode}");
        }
    }

    async Task<RecipeDetails> IRecipeService.GetRecipeDetailsAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"api/v1/recipes/{id}", cancellationToken);
        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                {
                    var stationInfoResponse = await response.Content.ReadFromJsonAsync<RecipeDetails>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);
                    return stationInfoResponse;
                }
            case System.Net.HttpStatusCode.Unauthorized:
                throw new Exception("Unauthorized!");
            default:
                throw new Exception($"Uhh {response.StatusCode}");
        }

    }
}

