using System.Text;
using Nancy.ViewEngines.Razor;

namespace FreelanceManager.Web.Tools
{
    public static class HtmlHelpers
    {
        public static IHtmlString ValidationSummary<T>(this HtmlHelpers<T> helper)
        {
            if (helper.RenderContext.Context.ModelValidationResult.IsValid)
                return helper.Raw("");

            var sb = new StringBuilder();
            
            sb.AppendLine(@"<div class=""alert alert-danger"">");

            foreach (var propertyInError in helper.RenderContext.Context.ModelValidationResult.Errors)
            {
                foreach (var error in propertyInError.Value)
                {
                    sb.AppendLine("<div>" + error.ErrorMessage + "</div>");
                }
            }

            sb.AppendLine("</div>");

            return helper.Raw(sb.ToString());
        }
    }
}