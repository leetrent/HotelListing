using HotelListing.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/{v:apiversion}/country")]
    [ApiController]
    public class CountryV2Controller : ControllerBase
    {
        private DatabaseContext _context;

        public CountryV2Controller(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]

        //////////////////////////////////////////////////////////////////////////////////////////
        // REPLACED WITH ServiceExtensions.ConfigureHttpCacheHeaders
        //////////////////////////////////////////////////////////////////////////////////////////
        // [ResponseCache(Duration = 60)] // USE FOR FINE-GRAINED CACHING
        // [ResponseCache(CacheProfileName = "CacheDuration-120Seconds")] // USE FOR GLOBAL CACHING

        //////////////////////////////////////////////////////////////////////////////////////////
        // Can be used to override global caching on a particular endpoint at any point. 
        //////////////////////////////////////////////////////////////////////////////////////////
        // [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        // [HttpCacheValidation(MustRevalidate = false)]
        //////////////////////////////////////////////////////////////////////////////////////////
        ///
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCountries()
        {
            return Ok(_context.Countries);
        }
    }
}
