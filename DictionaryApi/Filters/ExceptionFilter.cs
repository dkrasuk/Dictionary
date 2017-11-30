using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using AlfaBank.Logger;
using CommonServiceLocator;

namespace DictionaryApi.Filters
{
    /// <summary>
    /// ExceptionFilter
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// OnExceptionAsync
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var logger = ServiceLocator.Current.GetInstance<ILogger>();

            var errorMessage = GetErrorMessage(actionExecutedContext.Exception);

            actionExecutedContext.Response = new HttpResponseMessage
            {
                Content = new StringContent($"Error: {errorMessage}"),
                StatusCode = HttpStatusCode.InternalServerError,
                ReasonPhrase = "Error"
            };

            logger.Error(errorMessage);
            
            return Task.FromResult(0);
        }

        private static string GetErrorMessage(Exception ex)
        {
            var innerExceptionMessage = string.Empty;

            if (ex.InnerException != null && ex.InnerException.Message != string.Empty)
            {
                innerExceptionMessage = GetErrorMessage(ex.InnerException);
            }

            return $"{ex.Message}\n{innerExceptionMessage}";
        }
    }
}