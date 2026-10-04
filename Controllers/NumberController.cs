using Microsoft.AspNetCore.Mvc;
using Parcial1_P4_Yerferson.Models;
using Parcial1_P4_Yerferson.Services;

namespace Parcial1_P4_Yerferson.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NumberController(NumbersService numbersService) : ControllerBase
{
    [HttpGet("{numero:double}")]
    public IActionResult SumarMismoNumero(double numero)
    {
        double resultado = numero + numero;

        if (!double.IsFinite(numero) || !double.IsFinite(resultado))
            return BadRequest(new { mensaje = "El numero y el resultado deben ser finitos." });

        return Ok(new { original = numero, resultado });
    }

    [HttpGet("historial")]
    public async Task<IActionResult> GetList()
    {
        var records = await numbersService.GetListAsync();
        return Ok(records);
    }

    [HttpGet("historial/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var record = await numbersService.GetByIdAsync(id);

        if (record is null)
            return NotFound(new { mensaje = "Registro no encontrado." });

        return Ok(record);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] NumberRecord number)
    {
        double resultado = number.Numero + number.Numero;

        if (!double.IsFinite(number.Numero) || !double.IsFinite(resultado))
            return BadRequest(new { mensaje = "El numero y el resultado deben ser finitos." });

        var record = new NumberRecord(
            0, DateTime.UtcNow, number.Numero, resultado);

        int id = await numbersService.SaveAsync(record);
        var savedRecord = record with { Id = id };

        return CreatedAtAction(
            nameof(GetById), new { id }, savedRecord);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] NumberRecord number)
    {
        var previousRecord = await numbersService.GetByIdAsync(id);

        if (previousRecord is null)
            return NotFound(new { mensaje = "Registro no encontrado." });

        double resultado = number.Numero + number.Numero;

        if (!double.IsFinite(number.Numero) || !double.IsFinite(resultado))
            return BadRequest(new { mensaje = "El numero y el resultado deben ser finitos." });

        var record = new NumberRecord(
            id, DateTime.UtcNow, number.Numero, resultado);

        bool updated = await numbersService.UpdateAsync(record);

        if (!updated)
            return NotFound(new { mensaje = "Registro no encontrado." });

        return Ok(new { anterior = previousRecord, actual = record });
    }
}