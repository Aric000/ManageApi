using Microsoft.AspNetCore.Mvc;

namespace ManageApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    [HttpPost(Name = "Login")]
    public bool Get()
    {
        return true;
    }
}