using System.Data;
using BookingQueue.Common.Models;
using BookingQueue.Common.Models.ViewModels;
using BookingQueue.DAL.GenericRepository;
using Dapper;

namespace BookingQueue.BLL.Services;

public class AdvanceService : IAdvanceService
{
    private readonly IDbConnection _db;
    private readonly IGenericRepository<Advance> _repository;

    public AdvanceService(IDbConnection db, IGenericRepository<Advance> repository)
    {
        _db = db;
        _repository = repository;
    }

    public async Task<string> BookTimeAsync(BookViewModel bookViewModel, int maxClients)
    {
        await CheckAdvanceDateTimeAsync(bookViewModel.BookingDate, bookViewModel.ServiceId);

        var uniqueNumber = await GenerateUniqueIDAsync();

        var result = await _repository.InsertAsync(new Advance
        {
            Id = uniqueNumber,
            Comments = $"{bookViewModel.Name}; {bookViewModel.PhoneNumber}",
            AdvanceTime = bookViewModel.BookingDate,
            Priority = 2,
            ServiceId = bookViewModel.ServiceId
        });

        if (result == 0)
            throw new Exception("Что-то пошло не так, очередь не сохранен.");

        return uniqueNumber.ToString();
    }

    #region Private Methods

    private async Task CheckAdvanceDateTimeAsync(DateTime? date, long? serviceId)
    {
        var query = $"SELECT COUNT(*) FROM {typeof(Advance).Name} WHERE advance_time = @DateTime && service_id = @ServiveId";
        var count = await _db.ExecuteScalarAsync<int>(query, new { DateTime = date, ServiveId =  serviceId});
        
        if (count > 10)
            throw new Exception("Нет свободного места, выберите другое время!");
    }

    private async Task<int> GenerateUniqueIDAsync()
    {
        var id = GenerateRandomID();
        
        while (!await IsUniqueIDAsync(id))
        {
            id = GenerateRandomID();
        }

        return Convert.ToInt32(id);
    }

    private string GenerateRandomID()
    {
        var random = new Random();
        
        var idLength = random.Next(1000, 999999999);
        var chars = "0123456789";
        
        return new string(Enumerable.Repeat(chars, idLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private async Task<bool> IsUniqueIDAsync(string id)
    {
        var query = $"SELECT COUNT(*) FROM {typeof(Advance).Name} WHERE id = @ID";
        var count = await _db.ExecuteScalarAsync<int>(query, new { ID = id });
        return count == 0;
    }

    #endregion
}