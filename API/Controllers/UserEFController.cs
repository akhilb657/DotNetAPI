using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using API.Dtos;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
  DataContextEF _entityFramework;

  IUserRepository _userRepository;

  IMapper _mapper;

  public UserEFController(IConfiguration config, IUserRepository userRepository)
  {
    _entityFramework = new DataContextEF(config);

    _userRepository = userRepository;
    
    _mapper = new Mapper(new MapperConfiguration(cfg => {
      cfg.CreateMap<UserToAddDto, User>();
    }));
  }

  [HttpGet("GetUsers")]
  public IEnumerable<User> GetUsers()
  { 
    IEnumerable<User> users = _entityFramework.Users.ToList<User>();
    return users;
  }

  [HttpGet("GetSingleUser/{userId}")]
  public User GetSingleUser(int userId)
  { 
    User? user = _entityFramework.Users
      .Where(u => u.UserId == userId)
      .FirstOrDefault<User>();
    
    if (user != null)
    {
      return user;
    }
    
    throw new Exception("Failed to Get User");
    
  }

  [HttpPut("EditUser")]
  public IActionResult EditUser(User user)
  {

    User? userDb = _entityFramework.Users
      .Where(u => u.UserId == user.UserId)
      .FirstOrDefault<User>();
    
    if (userDb != null)
    {
      userDb.Active = user.Active;
      userDb.FirstName = user.FirstName;
      userDb.LastName = user.LastName;
      userDb.Email = user.Email;
      userDb.Gender = user.Gender;
      if(_userRepository.SaveChanges())
      {
        return Ok();
      }

      throw new Exception("Failed to Update User");
    }
    
    throw new Exception("Failed to Get User");
    
  }

  [HttpPost("AddUser")]
  public IActionResult AddUser(UserToAddDto user)
  {
    User userDb = _mapper.Map<User>(user);

    // userDb.Active = user.Active;
    // userDb.FirstName = user.FirstName;
    // userDb.LastName = user.LastName;
    // userDb.Email = user.Email;
    // userDb.Gender = user.Gender;

    _userRepository.AddEntity<User>(userDb);

    if(_userRepository.SaveChanges())
    {
      return Ok();
    }

    throw new Exception("Failed to Add User");
  }

  [HttpDelete("DeleteUser/{userId}")]
  public IActionResult DeleteUser(int userId)
  {
    User? userDb = _entityFramework.Users
      .Where(u => u.UserId == userId)
      .FirstOrDefault<User>();
    
    if (userDb != null)
    {
      _userRepository.RemoveEntity<User>(userDb);

      if(_userRepository.SaveChanges())
      {
        return Ok();
      }

      throw new Exception("Failed to Delete User");
    }
    
    throw new Exception("Failed to Get User");
  }

}