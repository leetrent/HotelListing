using AutoMapper;
using HotelListing.Data;
using HotelListing.DTO;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

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
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {

            IPagedList<Country> entityList = await _unitOfWork.Countries.GetPagedList(requestParams);
            IList<CountryDTO> dtoList = _mapper.Map<IList<CountryDTO>>(entityList);

            return Ok(dtoList);
        }

        [HttpGet("{id:int}", Name ="GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
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

            Country country = _mapper.Map<Country>(countryDTO);
            await _unitOfWork.Countries.Insert(country);
            await _unitOfWork.Save();

            return CreatedAtRoute("GetCountry", new { id = country.Id }, country);

        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
        {
            if (id < 1)
            {
                string errMsg = $"Country ID cannot be less than zero. Cannot continue with {nameof(UpdateCountry)}";
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }

            if (ModelState.IsValid == false)
            {
                _logger.LogError($"Model state is invalid in {nameof(UpdateCountry)} for Country ID {id}.");
                return BadRequest(ModelState);
            }

            Country country = await _unitOfWork.Countries.Get(q => q.Id == id);
            if (country == null)
            {
                string errMsg = $"Country with ID of {id} was NOT FOUND. Cannot continue with {nameof(UpdateCountry)}";
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }

            _mapper.Map(countryDTO, country);
            _unitOfWork.Countries.Update(country);
            await _unitOfWork.Save();

            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id < 1)
            {
                string errMsg = $"Country ID cannot be less than zero. Cannot continue with {nameof(DeleteCountry)}";
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }

            Country country = await _unitOfWork.Countries.Get(q => q.Id == id);
            if (country == null)
            {
                string errMsg = $"Country with ID of {id} was NOT FOUND. Cannot continue with {nameof(DeleteCountry)}";
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }

            await _unitOfWork.Countries.Delete(id);
            await _unitOfWork.Save();

            return NoContent();
        }
    }
}
