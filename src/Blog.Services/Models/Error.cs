﻿namespace Blog.Services.Models;

public class Error
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}