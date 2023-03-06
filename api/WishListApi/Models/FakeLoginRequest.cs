﻿using System.ComponentModel.DataAnnotations;

namespace WishListApi.Models;

public class FakeLoginRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(2)]
    public string? Name { get; set; }
}

