using Shouldly;
using WishListApi.Helpers;
using Xunit;

namespace WishListApi.Tests.Helpers;

public class WishListHelperTests
{
    [Fact]
    public void WishListHelper_should_remove_non_alpha_numeric_dot_or_dash_chars()
    {
        var result = WishListHelper.FormatSeoUrlKey("a!@#$%^&*()_+=[]{};:'\"\\|,.<>/?", "b-c");

        result.ShouldBe("a./b-c");
    }

    [Fact]
    public void WishListHelper_should_trim_and_replace_spaces_with_dash()
    {
        var result = WishListHelper.FormatSeoUrlKey("bugs  bunny", " birthday  ");

        result.ShouldBe("bugs-bunny/birthday");
    }

    [Fact]
    public void WishListHelper_should_lower_case()
    {
        var result = WishListHelper.FormatSeoUrlKey("BUGS Bunny", "Birthday (9)");

        result.ShouldBe("bugs-bunny/birthday-9");
    }
}
