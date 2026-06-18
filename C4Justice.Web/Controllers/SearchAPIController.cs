using C4Justice.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace C4Justice.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchApiController : Controller
{
    [EnableRateLimiting("abc")]
    [Route("[action]")]
    public IActionResult GetUsers()
    {
        var userList = new List<AdminUser>();
        for (int i = 0; i < 1500; i++)
        {
            var user = new AdminUser()
            {
                Id = i,
                Email = $"test{i}@gmail.com",
                IsActive = true,
                PasswordHash = "asdfa",
                Role = "Admin" + i,
                Username = "test",
            };
            userList.Add((AdminUser)user);
        }
        return Ok(userList);
    }

}