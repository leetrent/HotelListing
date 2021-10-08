using AutoMapper;
using HotelListing.Data;
using HotelListing.DTO;
using HotelListing.IRepository;
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

        [HttpGet("{id:int}")]
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
    }
}
