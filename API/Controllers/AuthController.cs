using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Dtos;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Controllers
{
  public class AuthController : ControllerBase
  {
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config)
    {
      _dapper = new DataContextDapper(config);
      _config = config;
    }

    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
      if (userForRegistration.Password == userForRegistration.PasswordConfirm){
        string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegistration.Email + "'";

        IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

        if (existingUsers.Count() == 0)
        {
          byte[] passwordSalt = new byte[128 / 8];
          using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
          {
            rng.GetNonZeroBytes(passwordSalt);
          }

          string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);
        
          byte[] passwordHash = KeyDerivation.Pbkdf2(
            password: userForRegistration.Password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1000000,
            numBytesRequested: 256 / 8
          );

          string sqlAddAuth = @"
              INSERT INTO TutorialAppSchema.Auth ([Email],
              [PasswordHash],
              [PasswordSalt]) VALUES('" + userForRegistration.Email + 
              "', @PasswordHash, @PasswordSalt)";
          
          List<SqlParameter> sqlParameters = new List<SqlParameter>();

          SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
          passwordSaltParameter.Value = passwordSalt;

          SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
          passwordHashParameter.Value = passwordHash;

          sqlParameters.Add(passwordSaltParameter);

          sqlParameters.Add(passwordHashParameter);

          if(_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
          {
            return Ok();
          }
          throw new Exception("Failed to Register user");
        }
        throw new Exception("User with this email already exists");
      }

      throw new Exception("Passwords do not match!");
      
    }


    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
      return Ok();
    }
  }
}