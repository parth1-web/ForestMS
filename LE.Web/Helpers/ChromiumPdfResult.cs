using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Rotativa.AspNetCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Helpers
{
    /// <summary>
    /// Drop-in replacement for Rotativa's <see cref="ViewAsPdf"/> that renders
    /// the same Razor view but prints it with a headless Chromium browser
    /// (Edge / Chrome / Brave) instead of wkhtmltopdf.
    ///
    /// Why: wkhtmltopdf 0.12.6 cannot shape Devanagari at all — subset
    /// inspection of its output shows at most .notdef/space glyphs for Nepali
    /// runs no matter which font or CSS is used, so Particulars/remarks render
    /// as boxes. Chromium shapes Indic scripts correctly and embeds real
    /// subset fonts (verified: extracted PDF text contains the Nepali).
    ///
    /// Usage is identical to ViewAsPdf; all existing return types keep working:
    ///     return new ChromiumPdfResult("reportPrint", viewModel);
    /// </summary>
    public class ChromiumPdfResult : ViewAsPdf
    {
        // Captured directly: do not rely on what the base ctors do with the
        // model across Rotativa versions — rendering below uses this field.
        private readonly object _model;

        public ChromiumPdfResult(string viewName, object model)
            : base(viewName, model)
        {
            _model = model;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            // Render the same Razor view Rotativa would have converted, then
            // print the HTML with headless Chromium instead of wkhtmltopdf.
            var html = await RenderViewAsync(context);
            var pdfBytes = await ChromiumPdf.PrintHtmlAsync(html);
            context.HttpContext.Response.ContentType = "application/pdf";
            await context.HttpContext.Response.Body.WriteAsync(pdfBytes, 0, pdfBytes.Length);
        }

        private async Task<string> RenderViewAsync(ActionContext context)
        {
            var services = context.HttpContext.RequestServices;
            var engine = (ICompositeViewEngine)services.GetService(typeof(ICompositeViewEngine));
            var tempDataProvider = (ITempDataProvider)services.GetService(typeof(ITempDataProvider));

            var getViewResult = engine.GetView(executingFilePath: null, viewPath: ViewName, isMainPage: true);
            IView view = null;
            if (getViewResult.Success)
            {
                view = getViewResult.View;
            }
            else
            {
                var findViewResult = engine.FindView(context, ViewName, isMainPage: true);
                if (findViewResult.Success)
                {
                    view = findViewResult.View;
                }
                else
                {
                    var searched = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
                    throw new InvalidOperationException("Unable to find view '" + ViewName + "'. Searched:" + Environment.NewLine + string.Join(Environment.NewLine, searched));
                }
            }

            var viewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary());
            viewData.Model = _model;
            var tempData = new TempDataDictionary(context.HttpContext, tempDataProvider);
            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(context, view, viewData, tempData, output, new HtmlHelperOptions());
                await view.RenderAsync(viewContext);
                return output.ToString();
            }
        }
    }

    /// <summary>
    /// Prints an HTML string to PDF via an installed headless Chromium browser.
    /// </summary>
    public static class ChromiumPdf
    {
        private static readonly string[] BrowserCandidates = new[]
        {
            @"Microsoft\Edge\Application\msedge.exe",
            @"Google\Chrome\Application\chrome.exe",
            @"BraveSoftware\Brave-Browser\Application\brave.exe",
        };

        public static Task<byte[]> PrintHtmlAsync(string html)
        {
            // Process I/O is blocking; keep the thin async wrapper so callers
            // stay awaitable like Rotativa's pipeline.
            return Task.Run(() => PrintHtml(html));
        }

        public static byte[] PrintHtml(string html)
        {
            var browser = FindBrowser();
            var tag = "forestms-pdf-" + Guid.NewGuid().ToString("N");
            var htmlPath = Path.Combine(Path.GetTempPath(), tag + ".html");
            var pdfPath = Path.Combine(Path.GetTempPath(), tag + ".pdf");
            var profileDir = Path.Combine(Path.GetTempPath(), tag + "-profile");
            try
            {
                File.WriteAllText(htmlPath, html, System.Text.Encoding.UTF8);
                var args = "--headless --disable-gpu --no-first-run --no-default-browser-check"
                    + " --no-pdf-header-footer --virtual-time-budget=10000"
                    + " --user-data-dir=\"" + profileDir + "\""
                    + " --print-to-pdf=\"" + pdfPath + "\""
                    + " \"" + htmlPath + "\"";
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = browser,
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                    }
                };
                proc.Start();
                if (!proc.WaitForExit(120000))
                {
                    try { proc.Kill(); } catch { }
                    throw new Exception("PDF print timed out after 120 seconds.");
                }
                if (!File.Exists(pdfPath) || new FileInfo(pdfPath).Length == 0)
                {
                    var err = "";
                    try { err = proc.StandardError.ReadToEnd(); } catch { }
                    throw new Exception("PDF print failed. " + err);
                }
                return File.ReadAllBytes(pdfPath);
            }
            finally
            {
                TryDelete(htmlPath);
                TryDelete(pdfPath);
                TryDeleteDir(profileDir);
            }
        }

        private static string FindBrowser()
        {
            var roots = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)),
            };
            foreach (var root in roots)
            {
                if (string.IsNullOrEmpty(root))
                {
                    continue;
                }
                foreach (var rel in BrowserCandidates)
                {
                    var full = Path.Combine(root, rel);
                    if (File.Exists(full))
                    {
                        return full;
                    }
                }
            }
            throw new Exception("PDF print failed: no Chromium browser found. Install Microsoft Edge, Google Chrome or Brave.");
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); } catch { }
        }

        private static void TryDeleteDir(string path)
        {
            try { if (Directory.Exists(path)) Directory.Delete(path, true); } catch { }
        }
    }
}
