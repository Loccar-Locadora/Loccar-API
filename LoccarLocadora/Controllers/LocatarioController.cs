using LoccarDomain.Locatario.Models;
using Microsoft.AspNetCore.Mvc;
using LoccarDomain;
using LoccarApplication.Interfaces;

namespace LoccarWebapi.Controllers
{
    [Route("api/locatario")]
    [ApiController]
    public class LocatarioController : ControllerBase
    {
        readonly ILocatarioApplication _locatarioApplication;
        public LocatarioController(ILocatarioApplication locatarioApplication)
        {
            _locatarioApplication = locatarioApplication;
        }
        [HttpPost]
        public BaseReturn<Locatario> CadastrarLocatario(Locatario locatario)
        {
            return _locatarioApplication.CadastrarLocatario(locatario);
        }
    }   
}
