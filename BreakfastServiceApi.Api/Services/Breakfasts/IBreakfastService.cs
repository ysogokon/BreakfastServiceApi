using BreakfastServiceApi.Api.Models;
using ErrorOr;

namespace BreakfastServiceApi.Api.Services.Breakfasts;

public interface IBreakfastService
{
  ErrorOr<Created> CreateBreakfast(Breakfast breakfast);
  ErrorOr<Deleted> DeleteBreakfast(Guid id);
  ErrorOr<Breakfast> GetBreakfast(Guid id);
  ErrorOr<UpsertedBreakfast> UpsertBreakfast(Breakfast breakfast);
}
