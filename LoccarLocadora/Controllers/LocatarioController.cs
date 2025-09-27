using LoccarDomain.Locatario.Models;
using Microsoft.AspNetCore.Mvc;
using LoccarDomain;
using LoccarApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LoccarWebapi.Controllers
{
    [Route("api/locatario")]
    [ApiController]
    [Authorize]
    public class LocatarioController : ControllerBase
    {
        readonly ILocatarioApplication _locatarioApplication;
        public LocatarioController(ILocatarioApplication locatarioApplication)
        {
            _locatarioApplication = locatarioApplication;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<BaseReturn<Locatario>> CadastrarLocatario(Locatario locatario)
        {
            return await _locatarioApplication.RegisterLocatario(locatario);
        }
    }   
}
