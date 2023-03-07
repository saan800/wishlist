using Shouldly;
using WishListApi.Helpers;
using Xunit;

namespace WishListApi.Tests.Helpers;

public class WishListHelperTests
{
    [Fact]
    public void FormatSeoUrlKey_should_remove_non_alpha_numeric_dot_or_dash_chars()
    {
        var result = WishListHelper.FormatSeoUrlKey("a!@#$%^&*()_+=[]{};:'\"\\|,.<>/?", "b-c");

        result.ShouldBe("a./b-c");
    }

    [Fact]
    public void FormatSeoUrlKey_should_concat_vars()
    {
        var result = WishListHelper.FormatSeoUrlKey("bugs  bunny", " birthday ( 9 ) ");

        result.ShouldBe("bugs-bunny/birthday-9");
    }

    [Fact]
    public void FormatKey_should_remove_non_alpha_numeric_dot_or_dash_chars()
    {
        var result = WishListHelper.FormatKey("a!@#$%^&*()_+=[]{};:'\"\\|,.<>/?b-c");

        result.ShouldBe("a.b-c");
    }

    [Fact]
    public void FormatKey_should_replace_double_strings_and_trim()
    {
        var result = WishListHelper.FormatKey("  bugs  bunny ");

        result.ShouldBe("bugs-bunny");
    }

    [Fact]
    public void FormatKey_should_lower_case()
    {
        var result = WishListHelper.FormatKey("BUGS Bunny");

        result.ShouldBe("bugs-bunny");
    }
}
