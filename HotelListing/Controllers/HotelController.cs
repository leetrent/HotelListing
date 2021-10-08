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
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelController> _logger;
        public HotelController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HotelController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                IList<Hotel> entityList = await _unitOfWork.Hotels.GetAll();
                IList<HotelDTO> dtoList = _mapper.Map<IList<HotelDTO>>(entityList);

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
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // Version #1:
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                Expression<Func<Hotel, bool>> expression = e => e.Id == id;
                List<string> includes = new List<string> { "Country" };
                Hotel entity = await _unitOfWork.Hotels.Get(expression, includes);

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // Version #2:
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                //Hotel entity = await _unitOfWork.Hotels.Get(e => e.Id == id, new List<string> { "Country" });

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // Map Entity to DTO
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                HotelDTO dto = _mapper.Map<HotelDTO>(entity);

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
