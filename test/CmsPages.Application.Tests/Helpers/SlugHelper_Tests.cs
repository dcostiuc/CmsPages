using CmsPages.Helpers;
using Xunit;

namespace CmsPages.Application.Tests.Helpers
{
    public class SlugHelperTests
    {
        private readonly SlugHelper _slugHelper;

        public SlugHelperTests()
        {
            _slugHelper = new SlugHelper();
        }

        [Theory]
        [InlineData("Hello World", "hello-world")]
        [InlineData("  ABP  Framework ", "abp-framework")]
        [InlineData("My@Cool#Page!", "mycoolpage")]
        [InlineData("Multiple     spaces", "multiple-spaces")]
        [InlineData("123 - TEST", "123-test")]
        [InlineData("Some---Dashes", "some-dashes")]
        [InlineData("", "")]
        [InlineData(null, "")]
        public void Slugify_Should_ConvertInputToSlug(string input, string expected)
        {
            var result = _slugHelper.Slugify(input);
            Assert.Equal(expected, result);
        }
    }
}
