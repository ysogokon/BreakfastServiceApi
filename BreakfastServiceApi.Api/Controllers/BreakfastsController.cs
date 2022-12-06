using BreakfastServiceApi.Api.Models;
using BreakfastServiceApi.Api.Services.Breakfasts;
using BreakfastServiceApi.Contracts.Breakfast;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace BreakfastServiceApi.Api.Controllers;

public class BreakfastsController : ApiController
{
  private readonly IBreakfastService _breakfastService;

  public BreakfastsController(IBreakfastService breakfastService)
  {
    _breakfastService = breakfastService;
  }

  [HttpPost]
  public IActionResult CreateBreakfast(CreateBreakfastRequest request)
  {
    var requestToBreakfastResult = Breakfast.From(request);


    if (requestToBreakfastResult.IsError)
    {
      return Problem(requestToBreakfastResult.Errors);
    }

    var breakfast = requestToBreakfastResult.Value;
    // save breakfast to database
    var createdBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

    return createdBreakfastResult.Match(
      created => CreatedAtGetBreakfast(breakfast),
      errors => Problem(errors));

    // if (createdBreakfastResult.IsError)
    // {
    //   return Problem(createdBreakfastResult.Errors);
    // }
    // return CreatedAtGetBreakfast(breakfast);
  }

  [HttpGet("{id:guid}")]
  public IActionResult GetBreakfast(Guid id)
  {
    ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

    return getBreakfastResult.Match(
      breakfast => Ok(MapBreakfastResult(breakfast)),
      errors => Problem(errors));

    // if (getBreakfastResult.IsError && getBreakfastResult.FirstError == Errors.Breakfast.NotFound)
    // {
    //   return NotFound();
    // }

    // var breakfast = getBreakfastResult.Value;
    // BreakfastResponse response = MapBreakfastResult(breakfast);

    // return Ok(response);
  }

  [HttpPut("{id:guid}")]
  public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
  {
    var requestToBreakfastResult = Breakfast.From(id, request);

    if (requestToBreakfastResult.IsError)
    {
      return Problem(requestToBreakfastResult.Errors);
    }
    var breakfast = requestToBreakfastResult.Value;

    var upsertBreakfastResult = _breakfastService.UpsertBreakfast(breakfast);

    // TODO: Return 201 if a new breakfast was created

    return upsertBreakfastResult.Match(
      upserted => upserted.IsNewlyCreated ? CreatedAtGetBreakfast(breakfast) : NoContent(),
      errors => Problem(errors));
  }

  [HttpDelete("{id:guid}")]
  public IActionResult DeleteBreakfast(Guid id)
  {
    var deleteBreakfastResult = _breakfastService.DeleteBreakfast(id);

    return deleteBreakfastResult.Match(
      deleted => NoContent(),
      errors => Problem(errors));
  }

  private static BreakfastResponse MapBreakfastResult(Breakfast breakfast)
  {
    return new BreakfastResponse(
          breakfast.Id,
          breakfast.Name,
          breakfast.Description,
          StartDateTime: breakfast.StartDateTime,
          breakfast.EndDateTime,
          breakfast.LastModifiedDateTime,
          breakfast.Savory,
          breakfast.Sweet
        );
  }

  private IActionResult CreatedAtGetBreakfast(Breakfast breakfast)
  {
    return CreatedAtAction(
      nameof(GetBreakfast),
      new { id = breakfast.Id },
      MapBreakfastResult(breakfast)
     );
  }
}
