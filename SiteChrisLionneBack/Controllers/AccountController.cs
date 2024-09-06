using Microsoft.AspNetCore.Mvc;
using SiteChrisLionneBack.Models.Users;
using System.Text.Json;
using SiteChrisLionneBack.Models.Serialization;
using System.Text;
using System.Net.Http;
using System.Net;

namespace SiteChrisLionneBack.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();
        private static Dictionary<string, string> errorMessages = new Dictionary<string, string>();

        static AccountController() {
            errorMessages.Add("INVALID_EMAIL", "L'utilisateur n'éxiste pas");
            errorMessages.Add("INVALID_LOGIN_CREDENTIALS", "L'email ou le mot de passe est invalide");
        }

        [HttpPost]
        [Route("login", Name = "Login")]
        public async Task<IActionResult> Login([FromForm] UserDTO user)
        {
            string jsonString = System.IO.File.ReadAllText("firebaseconfig.json");
            FirebaseProjectConfig firebaseConfig = JsonSerializer.Deserialize<FirebaseProjectConfig>(jsonString)!;

            var payload = new
            {
                email = user.email,
                password = user.password_clear,
                returnSecureToken = true
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + firebaseConfig.apiKey,
                content
            );

            Console.WriteLine(await response.Content.ReadAsStringAsync());

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var errorBody = JsonSerializer.Deserialize<AuthSuccessContent>(response.Content.ReadAsStream())!;
                return StatusCode(200, errorBody.idToken);
            }
            else
            {
                var errorBody = JsonSerializer.Deserialize<AuthErrorContent>(response.Content.ReadAsStream())!;
                return StatusCode(errorBody.error.code, errorMessages[errorBody.error.message]);
            }
        }
    }
}
