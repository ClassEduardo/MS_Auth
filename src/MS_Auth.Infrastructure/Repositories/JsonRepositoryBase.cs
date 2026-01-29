using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using MS_Auth.Domain.Models;
using MS_Auth.Domain.Repositories;

namespace MS_Auth.Infrastructure.Repositories;

public class JsonRepositoryBase<T> : IRepositoryBase<T> where T : class, IEntity
{
    private readonly string _filePath = string.Empty;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public JsonRepositoryBase(string fileName)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var dataDirectory = Path.Combine(baseDirectory, "Data");

        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }

        _filePath = Path.Combine(dataDirectory, fileName);

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<T> CriarAsync(T entity)
    {
        var items = await LerTodosAsync();

        entity.Id = Guid.NewGuid();

        items.Add(entity);
        await SalvarTodosAsync(items);

        return entity;
    }

    public async Task<T?> ObterPorIdAsync(Guid id)
    {
        var items = await LerTodosAsync();
        return items.FirstOrDefault(x => x.Id == id);
    }

    public async Task<T> ObterPorFiltroAsync(Expression<Func<T, bool>> predicate)
    {
        var items = await LerTodosAsync();
        return (T)items.AsQueryable().Where(predicate);
    }

    public async Task<T> AtualizarAsync(T entity)
    {
        var items = await LerTodosAsync();
        var index = items.FindIndex(x => x.Id == entity.Id);

        if (index == -1)
            throw new KeyNotFoundException($"Entidade com ID {entity.Id} não encontrada");

        items[index] = entity;
        await SalvarTodosAsync(items);

        return entity;
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var items = await LerTodosAsync();
        var entity = items.FirstOrDefault(x => x.Id == id);

        if (entity == null)
            return false;

        items.Remove(entity);

        await SalvarTodosAsync(items);
        return true;
    }

    protected async Task SalvarTodosAsync(List<T> items)
    {
        await _semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(items, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<T>> LerTodosAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}