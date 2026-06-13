namespace WebAPI.Extensions
{
    public static class ResultExtensions
    {
        public static Results<Created<TResponse>, ValidationProblem, ProblemHttpResult>
            ToCreatedResult<TValue, TResponse>(
                this Result<TValue> result,
                Func<TValue, string> locationBuilder,
                Func<TValue, TResponse> mapResponse) =>
            result.Status switch
            {
                ResultStatus.Ok => TypedResults.Created(locationBuilder(result.Value), mapResponse(result.Value)),
                ResultStatus.Invalid => TypedResults.ValidationProblem(
                    result.ValidationErrors
                        .GroupBy(e => e.Identifier ?? string.Empty)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),
                _ => TypedResults.Problem(
                    title: "Create failed",
                    detail: string.Join("; ", result.Errors),
                    statusCode: StatusCodes.Status400BadRequest)
            };

        public static Results<Ok<TResponse>, NotFound, ProblemHttpResult>
            ToGetByIdResult<TValue, TResponse>(
                this Result<TValue> result,
                Func<TValue, TResponse> mapResponse) =>
            result.Status switch
            {
                ResultStatus.Ok => TypedResults.Ok(mapResponse(result.Value)),
                ResultStatus.NotFound => TypedResults.NotFound(),
                _ => TypedResults.Problem(
                    title: "Get failed",
                    detail: string.Join("; ", result.Errors),
                    statusCode: StatusCodes.Status400BadRequest)
            };

        public static Results<NoContent, NotFound, ProblemHttpResult>
            ToDeleteResult(this Result result) =>
            result.Status switch
            {
                ResultStatus.Ok => TypedResults.NoContent(),
                ResultStatus.NotFound => TypedResults.NotFound(),
                _ => TypedResults.Problem(
                    title: "Delete failed",
                    detail: string.Join("; ", result.Errors),
                    statusCode: StatusCodes.Status400BadRequest)
            };
    }
}
