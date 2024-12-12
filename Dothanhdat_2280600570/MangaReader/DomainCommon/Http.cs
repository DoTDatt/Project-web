using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MangaReader.DomainCommon;

public class Http: IDisposable
{
    private readonly HttpClient client = new();

    public Http()
    {
        client.DefaultRequestHeaders.Add("User-Agent","MangaReader");
    }

    public Task<string> GetStringAsync(string url)
    {
        return GetStringAsync(url, CancellationToken.None);
    }

    public async Task<string> GetStringAsync(string url, CancellationToken token)
    {
        try
        {
            return await client.GetStringAsync(url, token);
        }
        catch (HttpRequestException ex)
        {
            throw new NetworkException(ex.Message);
        }
    }

    public Task<byte[]> GetBytesAsync(string url)
    {
        return GetBytesAsync(url, CancellationToken.None);
    }

    public async Task<byte[]> GetBytesAsync(string url, CancellationToken token)
    {
        try
        {
            using var message = await client.GetAsync(url, token);
            return await message.Content.ReadAsByteArrayAsync(token);
        }
        catch (HttpRequestException ex)
        {
            throw new NetworkException(ex.Message);
        }
    }

    public void Dispose()
    {
        client.Dispose();
    }
}