using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.LEPagination
{
    [HtmlTargetElement("LeadingEdgePager")]
    public class LeadingEdgePagerTagHelper : TagHelper
    {
        public PaginatedMetaModel Info { get; set; }

        public string PreviousPageText { get; set; } = "Previous";

        public string NextPageText { get; set; } = "Next";

        public string Route { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            BuildParent(output);
            if (Info.PreviousPage.Display) AddPreviousPage(output);
            AddPageNodes(output);
            if (Info.NextPage.Display) AddNextPage(output);
        }

        /// <summary>
        /// Build parent tag (ul)
        /// </summary>
        private static void BuildParent(TagHelperOutput output)
        {
            output.TagName = "ul";
            output.Attributes.Add("class", "pagination");
            output.Attributes.Add("role", "navigation");
            output.Attributes.Add("aria-label", "Pagination");
        }

        /// <summary>
        /// Build previous page list item
        /// </summary>
        private void AddPreviousPage(TagHelperOutput output)
        {
            var href = BuildPageUrl(Info.PreviousPage.PageNumber);
            var html =
$@"<li class=""pagination-previous"">
    <a href=""{href}"" aria-label=""{PreviousPageText} page"">{PreviousPageText} <span class=""show-for-sr"">page</span></a>
</li>";

            output.Content.SetHtmlContent(output.Content.GetContent() + html);
        }

        /// <summary>
        /// Build next page list item
        /// </summary>
        private void AddNextPage(TagHelperOutput output)
        {
            var href = BuildPageUrl(Info.NextPage.PageNumber);
            var html =
$@"<li class=""pagination-next"">
    <a href=""{href}"" aria-label=""{NextPageText} page"">{NextPageText} <span class=""show-for-sr"">page</span></a>
</li>";

            output.Content.SetHtmlContent(output.Content.GetContent() + html);
        }

        private void AddPageNodes(TagHelperOutput output)
        {
            foreach (var infoPage in Info.Pages)
            {
                string html;
                if (infoPage.IsCurrent)
                {
                    html = $@"<li class=""current""><span class=""show-for-sr"">You're on page</span> {infoPage.PageNumber}</li>";
                    output.Content.SetHtmlContent(output.Content.GetContent() + html);
                    continue;
                }
                var href = BuildPageUrl(infoPage.PageNumber);
                html = $@"<li><a href=""{href}"" aria-label=""Page {infoPage.PageNumber}"">{infoPage.PageNumber}</a></li>";
                output.Content.SetHtmlContent(output.Content.GetContent() + html);
            }
        }

        private string BuildPageUrl(int pageNumber)
        {
            if (string.IsNullOrEmpty(Route))
            {
                return "#";
            }

            // Build URL manually to avoid version-specific UrlHelper issues
            var path = ViewContext.HttpContext.Request.Path.Value ?? "/";
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("", 
                ViewContext.HttpContext.Request.Query
                    .Where(kvp => kvp.Key != "page")
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()));
            
            // Add/update page parameter
            var separator = query.Contains('?') ? '&' : '?';
            query = $"{query}{separator}page={pageNumber}";
            
            return path + query;
        }
    }
}
