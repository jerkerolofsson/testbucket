﻿namespace TestBucket.Sdk.Client.Extensions;
internal static class HttpResponseMessageExtensions
{
    public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"{response.StatusCode}: {text}", null, response.StatusCode);
        }
    }
}
