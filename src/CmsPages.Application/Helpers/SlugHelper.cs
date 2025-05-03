using CmsPages.Helpers;
namespace CmsPages.Helpers
{
    public class SlugHelper : ISlugHelper
    {
        public string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.ToLowerInvariant();
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\s+", "-"); // Replace spaces with dashes
            input = System.Text.RegularExpressions.Regex.Replace(input, @"[^a-z0-9\-]", ""); // Remove invalid url/slug characters
            input = System.Text.RegularExpressions.Regex.Replace(input, @"-+", "-"); // Remove multiple dashes
            return input.Trim('-');
        }
    }
}
