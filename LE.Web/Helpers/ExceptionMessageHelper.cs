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

        // Development-only diagnostic suffix: the sanitizer intentionally hides
        // infrastructure errors, which makes localhost debugging a guessing game
        // (every failure looks identical). Never emitted in production.
        private static string withDevDetail(Exception ex, string display)
        {
            if (ex is CustomException) return display;
            if (!string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
                return display;
            var inner = ex.InnerException != null ? " <- " + ex.InnerException.GetType().Name + ": " + ex.InnerException.Message : string.Empty;
            return display + " [Technical: " + ex.GetType().Name + ": " + ex.Message + inner + "]";
        }

        // TempData alerts (browser pages)
        public static void setMessage(Controller controller, Exception ex, messageType message_type = messageType.error)
        {
            AlertHelper.setMessage(controller, withDevDetail(ex, getDisplayMessage(ex)), message_type);
        }

        // AJAX JSON responses
        public static object buildErrorObject(Exception ex)
        {
            return new { error = true, responseText = withDevDetail(ex, getDisplayMessage(ex)) };
        }

        public static string buildErrorJson(Exception ex)
        {
            return JsonWrapper.buildErrorJson(withDevDetail(ex, getDisplayMessage(ex)));
        }

        // Legacy success=false JSON shapes
        public static object buildFailObject(Exception ex)
        {
            return new { success = false, message = withDevDetail(ex, getDisplayMessage(ex)) };
        }
    }
}
