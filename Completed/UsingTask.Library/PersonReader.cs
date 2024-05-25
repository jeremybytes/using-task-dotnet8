﻿using System.Text.Json;
using UsingTask.Shared;

namespace UsingTask.Library;

public class PersonReader
{
    private readonly HttpClient client = new() { BaseAddress = new Uri("http://localhost:9874") };
    private readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

    public async Task<List<Person>> GetPeopleAsync(
        CancellationToken cancelToken = new())
    {
        //throw new NotImplementedException("Jeremy did not implement GetPeopleAsync");

        await Task.Delay(3000, cancelToken);

        cancelToken.ThrowIfCancellationRequested();

        HttpResponseMessage response =
            await client.GetAsync("people");

        response.EnsureSuccessStatusCode();

        var stringResult =
            await response.Content.ReadAsStringAsync(cancelToken);
        var result = JsonSerializer.Deserialize<List<Person>>(stringResult, options);
        return result ?? [];
    }
}
