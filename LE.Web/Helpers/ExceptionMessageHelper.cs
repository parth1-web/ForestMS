using LE.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LE.Web.Helpers
{
    /// <summary>
    /// Centralizes how exception messages are surfaced to clients (P2/S11).
    /// Application exceptions (CustomException and its subclasses) carry
    /// user-presentable, intentionally written messages — they are shown as-is.
    /// Anything else (framework, database, NREs, raw Npgsql errors…) can leak
    /// infrastructure details (stack contents, constraint names, file paths),
    /// so clients get a generic message while the full exception is logged.
    /// </summary>
    public static class ExceptionMessageHelper
    {
        public const string GenericErrorMessage = "An unexpected error occurred while processing your request. Please try again or contact support.";

        public static string getDisplayMessage(Exception ex)
        {
            return ex is CustomException ? ex.Message : GenericErrorMessage;
        }

        // TempData alerts (browser pages)
        public static void setMessage(Controller controller, Exception ex, messageType message_type = messageType.error)
        {
            AlertHelper.setMessage(controller, getDisplayMessage(ex), message_type);
        }

        // AJAX JSON responses
        public static object buildErrorObject(Exception ex)
        {
            return new { error = true, responseText = getDisplayMessage(ex) };
        }

        public static string buildErrorJson(Exception ex)
        {
            return JsonWrapper.buildErrorJson(getDisplayMessage(ex));
        }

        // Legacy success=false JSON shapes
        public static object buildFailObject(Exception ex)
        {
            return new { success = false, message = getDisplayMessage(ex) };
        }
    }
}
