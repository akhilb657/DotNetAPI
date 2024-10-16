using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using API.Dtos;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
  DataContextDapper _dapper;

  public UserCompleteController(IConfiguration config)
  {
    _dapper = new DataContextDapper(config);
  }

  [HttpGet("TestConnection")]
  public DateTime TestConnection()
  {
    return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
  }

  [HttpGet("GetUsers/{userId}/{isActive}")]
  public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
  {

    string sql = @"EXEC TutorialAppSchema.spUsers_Get";
    string parameters = "";

    if (userId != 0)
    {
      parameters += ", @UserId=" +  userId.ToString();
    }

    if (isActive)
    {
      parameters += ", @Active=" +  isActive.ToString();
    }

    sql += parameters.Substring(1);
    
    IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
    return users;
  }

  [HttpPut("EditUser")]
  public IActionResult EditUser(User user)
  {
    string sql = @"
      UPDATE TutorialAppSchema.Users
        SET [FirstName] = '" + user.FirstName + 
            "', [LastName] = '" + user.LastName +
            "', [Email] = '" + user.Email + 
            "', [Gender] = '" + user.Gender + 
            "', [Active] = '" + user.Active + 
        "' WHERE UserId = " + user.UserId;

    Console.WriteLine(sql);
      
    if(_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to Update User");
    
  }

  [HttpPost("AddUser")]
  public IActionResult AddUser(UserToAddDto user)
  {
    string sql = @"INSERT INTO TutorialAppSchema.Users(
              [FirstName],
              [LastName],
              [Email],
              [Gender],
              [Active]
          ) VALUES (" + 
              "'" + user.FirstName + 
              "', '" + user.LastName +
              "', '" + user.Email + 
              "', '" + user.Gender + 
              "', '" + user.Active + 
          "')";
    
    Console.WriteLine(sql);
      
    if(_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to Add User");
  }

  [HttpDelete("DeleteUser/{userId}")]
  public IActionResult DeleteUser(int userId)
  {
    string sql = @"DELETE FROM TutorialAppSchema.Users WHERE UserId = "
                + userId.ToString();
    
    Console.WriteLine(sql);
      
    if(_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to Delete User");
  }

}