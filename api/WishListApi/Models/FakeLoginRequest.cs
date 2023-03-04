using System.ComponentModel.DataAnnotations;

namespace WishListApi.Models;

public class FakeLoginRequest
{
	[Required]
	[EmailAddress]
	public string? Email { get; set; }

	[Required]
	public string? FirstName { get; set; }

	[Required]
	[MinLength(2)]
	public string? LastName { get; set; }
}

