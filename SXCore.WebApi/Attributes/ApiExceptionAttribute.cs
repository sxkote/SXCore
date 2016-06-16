using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace SXCore.WebApi.Attributes
{
    public class ApiExceptionAttribute : ExceptionFilterAttribute
    {
        public ILogger Logger { get; set; }

        public override void OnException(HttpActionExecutedContext context)
        {
            try
            {
                if (this.Logger != null)
                    Logger.Error(context.Exception);

                //var logger = NLog.LogManager.GetLogger("Terminal");
                //if (logger != null)
                //    logger.Error(context.Exception);
            }
            catch { }

            if (context.Exception is NotImplementedException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else if (context.Exception is CustomNotFoundException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else if (context.Exception is CustomException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            else
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            context.Response.Content = new StringContent(GetExceptionMessage(context.Exception));

            //context.Response.Content = new ObjectContent<ErrorResponse>(new ErrorResponse(context.Exception), GlobalConfiguration.Configuration.Formatters.JsonFormatter);
        }

        protected string GetExceptionMessage(Exception ex)
        {
            if (ex == null)
                return "";

            var result = ex.Message;

            if (ex.InnerException != null)
                result += Environment.NewLine + Environment.NewLine + "---------------------------------------------" + Environment.NewLine + GetExceptionMessage(ex.InnerException);

            return result;
        }
    }

}
