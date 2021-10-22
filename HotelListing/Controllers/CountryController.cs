using AutoMapper;
using HotelListing.Data;
using HotelListing.DTO;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryController> _logger;

        public CountryController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CountryController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                IList<Country> entityList = await _unitOfWork.Countries.GetAll();
                IList<CountryDTO> dtoList = _mapper.Map<IList<CountryDTO>>(entityList);

                return Ok(dtoList);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                _logger.LogError(exc.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id:int}", Name ="GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // Version #1:
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                Expression<Func<Country, bool>> expression = c => c.Id == id;
                List<string> includes = new List<string> { "Hotels" };
                Country entity = await _unitOfWork.Countries.Get(expression, includes);

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // Version #2:
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                //Country entity = await _unitOfWork.Countries.Get(c => c.Id == id, new List<string> { "Hotels" });

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // Map Entity to DTO
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                CountryDTO dto = _mapper.Map<CountryDTO>(entity);

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // Return ActionResult
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                return Ok(dto);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                _logger.LogError(exc.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
        {
            if (ModelState.IsValid == false)
            {
                _logger.LogError($"Invalid POST: {nameof(CreateCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                Country country = _mapper.Map<Country>(countryDTO);
                await _unitOfWork.Countries.Insert(country);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                _logger.LogError(exc.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected server-side error was encountered when attempting to save a new country.");
            }
        }
    }
}
