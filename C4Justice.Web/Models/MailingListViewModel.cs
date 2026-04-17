using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models;

public class MailingListViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(10)]
    public string ZipCode { get; set; } = string.Empty;
}
