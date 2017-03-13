using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace Taxes.App_Start {
    public class CustomExceptionHandler : ExceptionHandler {
        public override void Handle(ExceptionHandlerContext context) {
            var exception = context.Exception;

            Elmah.ErrorSignal.FromCurrentContext().Raise(exception);

#if DEBUG
            context.Result =
                new ResponseMessageResult(context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception));
#else
            context.Result =
                new ResponseMessageResult(context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Įvyko nenumatyta klaida"));
#endif


            base.Handle(context);
        }
    }
}