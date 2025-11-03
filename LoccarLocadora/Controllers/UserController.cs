using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.User.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoccarLocadora.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApplication;

        public UserController(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }

        /// <summary>
        /// Lista todos os usuários ativos do sistema
        /// </summary>
        /// <returns>Lista de usuários com name, email e cellphone</returns>
        /// <response code="200">Lista de usuários retornada com sucesso</response>
        /// <response code="401">Usuário não autorizado (apenas ADMIN e EMPLOYEE)</response>
        /// <response code="404">Nenhum usuário encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("list/all")]
        [ProducesResponseType(typeof(BaseReturn<List<User>>), 200)]
        [ProducesResponseType(typeof(BaseReturn<object>), 401)]
        [ProducesResponseType(typeof(BaseReturn<object>), 404)]
        [ProducesResponseType(typeof(BaseReturn<object>), 500)]
        public async Task<IActionResult> ListAllUsers()
        {
            var result = await _userApplication.ListAllUsers();

            return result.Code switch
            {
                "200" => Ok(result),
                "401" => Unauthorized(result),
                "404" => NotFound(result),
                _ => StatusCode(500, result)
            };
        }
    }
}
