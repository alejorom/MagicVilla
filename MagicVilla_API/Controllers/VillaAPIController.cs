﻿using MagicVilla_API.Data;
using MagicVilla_API.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0) { return BadRequest(); }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null) { return NotFound(); } 
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDto)
        {
            if (villaDto == null || string.IsNullOrWhiteSpace(villaDto.Name)) { return BadRequest(); }
            if (villaDto.Id > 0) { return StatusCode(StatusCodes.Status500InternalServerError); }

            // Validar si el nombre ya existe (opcional)
            if (VillaStore.villaList.Any(v => v.Name.Equals(villaDto.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe.");
                return BadRequest(ModelState);
            }

            // Generar nuevo Id
            int newId = VillaStore.villaList.Count != 0 ? VillaStore.villaList.Max(v => v.Id) + 1 : 1;
            villaDto.Id = newId;
            VillaStore.villaList.Add(villaDto);

            return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto);
        }
    }
}
