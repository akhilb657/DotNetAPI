namespace API.Dtos;
{
  partial class UserForLoginConfirmationDto
  {
    byte[] PasswordHash {get; set;}
    byte[] PasswordSalt {get; set;}

    UserForLoginConfirmationDto()
    {
        if(PasswordHash == null)
        {
          PasswordHash = new byte[0];
        }

          if(PasswordSalt == null)
        {
          PasswordSalt = new byte[0];
        }
    }
  }
}