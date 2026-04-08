namespace UserManagement.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string Status { get; set; } = default!;
}

