using SchoolManagement.Services;

namespace SchoolManagement.Endpoints;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this ServiceResult<T> result, Func<T, object>? dataMapper = null)
    {
        if (result.IsSuccess && result.Data != null)
        {
            var data = dataMapper != null ? dataMapper(result.Data) : result.Data;
            return result.Status switch
            {
                ServiceResultStatus.Created => Results.Created(string.Empty, data),
                _ => Results.Ok(data)
            };
        }

        return result.Status switch
        {
            ServiceResultStatus.NotFound => Results.NotFound(new { message = result.ErrorMessage }),
            ServiceResultStatus.BadRequest => Results.BadRequest(new { message = result.ErrorMessage }),
            ServiceResultStatus.Conflict => Results.Conflict(new { message = result.ErrorMessage }),
            _ => Results.Problem(result.ErrorMessage)
        };
    }
}
