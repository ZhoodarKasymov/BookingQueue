using System.Data;
using Dapper;

namespace BookingQueue.DAL.GenericRepository;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly IDbConnection _db;

    public GenericRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        string query = $"SELECT * FROM {typeof(TEntity).Name}";
        return await _db.QueryAsync<TEntity>(query);
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        var query = $"SELECT * FROM {typeof(TEntity).Name} WHERE Id = @Id";
        return await _db.QueryFirstOrDefaultAsync<TEntity>(query, new { Id = id });
    }

    public async Task<int> InsertAsync(TEntity entity)
    {
        var query = GenerateInsertQuery();
        return await _db.ExecuteAsync(query, entity);
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        var query = GenerateUpdateQuery();
        var rowsAffected = await _db.ExecuteAsync(query, entity);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        string query = $"DELETE FROM {typeof(TEntity).Name} WHERE Id = @Id";
        int rowsAffected = await _db.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }

    #region Private methods

    private string GenerateInsertQuery()
    {
        var tableName = typeof(TEntity).Name;
        var properties = typeof(TEntity).GetProperties().Where(p => p.Name != "Id");
        var columns = string.Join(",", properties.Select(p => p.Name));
        var values = string.Join(",", properties.Select(p => "@" + p.Name));
        return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
    }

    private string GenerateUpdateQuery()
    {
        var tableName = typeof(TEntity).Name;
        var properties = typeof(TEntity).GetProperties().Where(p => p.Name != "Id");
        var columns = string.Join(",", properties.Select(p => $"{p.Name}=@{p.Name}"));
        return $"UPDATE {tableName} SET {columns} WHERE Id=@Id";
    }

    #endregion
}