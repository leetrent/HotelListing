using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTO
{
    public class CreateCountryDTO
    {
        [Display(Name = "Country Name")]
        [Required(ErrorMessage = "{1} is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} must be between {1} and {2} characters in length.")]
        public string Name { get; set; }

        [Display(Name = "Short Country Name")]
        [Required(ErrorMessage = "{1} is required.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "{0} must be {1} characters in length.")]
        public string ShortName { get; set; }
    }
    public class CountryDTO : CreateCountryDTO
    {
        public int Id { get; set; }
        public IList<HotelDTO> Hotels { get; set; }
    }
}
