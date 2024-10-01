using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

  public UserController()
  {

  }

  [HttpGet("GetUsers/{testValue}")]
  public string[] GetUsers(string testValue)
  {
    string[] responseArray = new string[] {
      "test1",
      "test2",
      testValue
    };
    return responseArray;
  }

}