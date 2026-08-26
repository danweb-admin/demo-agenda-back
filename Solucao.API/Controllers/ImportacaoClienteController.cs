using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Solucao.API.Controllers
{
  [Route("api/v1/importacao-cliente")]
  [ApiController]
  //[Authorize]
  public class ImportacaoClienteController : ControllerBase
  {
    private readonly ClienteImportacaoService _importacaoService;

    public ImportacaoClienteController( ClienteImportacaoService importacaoService) {
      _importacaoService = importacaoService;
    }

    [HttpPost("import")]
    public async Task<IActionResult> Importar( IFormFile arquivo) {

      var resultado = await _importacaoService.ImportarAsync(arquivo);

      return Ok(resultado);
    }
  }
}

