using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Dtos;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using API.Helpers;
using Dapper;
using AutoMapper;
using API.Models;

namespace API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;
    private readonly ReusableSql _reusableSql;
    private readonly IMapper _mapper;
    public AuthController(IConfiguration config)
    {
      _dapper = new DataContextDapper(config);
      _authHelper = new AuthHelper(config);
      _reusableSql = new ReusableSql(config);
      _mapper = new Mapper(new MapperConfiguration(cfg => 
      {
        cfg.CreateMap<UserForRegistrationDto, UserComplete>();
      }));
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
      if (userForRegistration.Password == userForRegistration.PasswordConfirm){
        string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegistration.Email + "'";

        IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

        if (existingUsers.Count() == 0)
        {
          UserForLoginDto userForSetPassword = new UserForLoginDto() {
            Email = userForRegistration.Email,
            Password = userForRegistration.Password
          };
          if(_authHelper.SetPassword(userForSetPassword))
          {
            UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
            userComplete.Active = true;

            if (_reusableSql.UpsertUser(userComplete))
            {
              return Ok();
            }
            throw new Exception("Failed to Add User");
           
          }
          throw new Exception("Failed to Register user");
        }
        throw new Exception("User with this email already exists");
      }

      throw new Exception("Passwords do not match!");
      
    }

    [HttpPut("ResetPassword")]
    public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
    {
      if(_authHelper.SetPassword(userForSetPassword))
      {
        return Ok();
      }
      throw new Exception("Failed to update password");
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
      string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get 
            @Email = @EmailParam";
      
      DynamicParameters sqlParameters = new DynamicParameters();

      // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
      // emailParameter.Value = userForLogin.Email;
      // sqlParameters.Add(emailParameter);

      sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);
      
      UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

      byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

      // if (passwordHash == userForConfirmation.PasswordHash)

      for(int index = 0; index < passwordHash.Length; index++)
      {
        if(passwordHash[index] != userForConfirmation.PasswordHash[index])
        {
          return StatusCode(401, "Incorrect password");
        }
      }

      string userIdSql = @"
          SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" + 
          userForLogin.Email + "'";

      int userId = _dapper.LoadDataSingle<int>(userIdSql);


      return Ok(new Dictionary<string, string> {
        {"token", _authHelper.CreateToken(userId)}
      });
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
      string userId = User.FindFirst("userId")?.Value + "";

      string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = " + userId;
      
      int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);

      // if (string.IsNullOrEmpty(userId))
      //   {
      //       return BadRequest("User ID claim missing or invalid token");
      //   }

      // return CreateToken(userIdFromDB);

      return Ok(new Dictionary<string, string> {
        {"token", _authHelper.CreateToken(userIdFromDB)}
      });
    }
  }
}