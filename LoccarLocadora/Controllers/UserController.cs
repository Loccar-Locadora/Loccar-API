using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.Customer.Models;
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
        private readonly ICustomerApplication _customerApplication;

        public UserController(IUserApplication userApplication, ICustomerApplication customerApplication)
        {
            _userApplication = userApplication;
            _customerApplication = customerApplication;
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

        /// <summary>
        /// Atualiza um usuário pelo ID do usuário
        /// </summary>
        /// <param name="userId">ID do usuário a ser atualizado</param>
        /// <param name="customerData">Dados do cliente para atualização do usuário</param>
        /// <returns>Usuário atualizado</returns>
        /// <response code="200">Usuário atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("update/{userId}")]
        [ProducesResponseType(typeof(BaseReturn<User>), 200)]
        [ProducesResponseType(typeof(BaseReturn<object>), 400)]
        [ProducesResponseType(typeof(BaseReturn<object>), 404)]
        [ProducesResponseType(typeof(BaseReturn<object>), 500)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] Customer customerData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseReturn<object>
                {
                    Code = "400",
                    Message = "Invalid data provided.",
                    Data = ModelState
                });
            }

            var result = await _userApplication.UpdateUser(userId, customerData);

            return result.Code switch
            {
                "200" => Ok(result),
                "404" => NotFound(result),
                _ => StatusCode(500, result)
            };
        }

        /// <summary>
        /// Deleta um usuário pelo ID do usuário
        /// </summary>
        /// <param name="userId">ID do usuário a ser deletado</param>
        /// <returns>Confirmação da deleção</returns>
        /// <response code="200">Usuário deletado com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("delete/{userId}")]
        [ProducesResponseType(typeof(BaseReturn<object>), 200)]
        [ProducesResponseType(typeof(BaseReturn<object>), 404)]
        [ProducesResponseType(typeof(BaseReturn<object>), 500)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userApplication.DeleteUser(userId);

            return result.Code switch
            {
                "200" => Ok(result),
                "404" => NotFound(result),
                _ => StatusCode(500, result)
            };
        }

        [HttpGet("find/email")]
        public async Task<BaseReturn<User>> GetUserByEmail(string email)
        {
            return await _userApplication.GetUserByEmail(email);
        }
    }
}
