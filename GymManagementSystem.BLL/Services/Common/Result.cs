using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Common
{
    public record Result (bool success, string? ErrorMessage = null, ResultKind kind = ResultKind.Ok)
    {
        public static Result Ok() => new Result(true);
        public static Result Fail(string errorMessage) => new Result(false, errorMessage,ResultKind.Conflict);
        public static Result NotFound(string errorMessage = "Not Found") => new Result(false, errorMessage,ResultKind.NotFound);
        public static Result Validation(string errorMessage) => new Result(false, errorMessage,ResultKind.ValidationFailed);

    }

    public record Result <T>(bool success,T? value ,string? ErrorMessage = null, ResultKind kind = ResultKind.Ok)
    {
        public static Result<T> Ok(T value) => new(true, value);
        public static Result<T> Fail(string errorMessage) => new(false, default, errorMessage, ResultKind.Conflict);
        public static Result<T> NotFound(string errorMessage = "Not Found") => new(false, default, errorMessage, ResultKind.NotFound);
        public static Result<T> Validation(string errorMessage) => new(false, default, errorMessage, ResultKind.ValidationFailed);
    }

    public enum ResultKind
    {
        Ok,
        NotFound,
        Conflict,
        ValidationFailed,
        Forbidden
    }
}
