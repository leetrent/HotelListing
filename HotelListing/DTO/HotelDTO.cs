using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.DTO
{
    public class CreateHotelDTO
    {
        [Display(Name = "Hotel Name")]
        [Required(ErrorMessage = "{1} is required.")]   
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} must be between {1} and {2} characters in length.")]
        public string Name { get; set; }

        [Display(Name = "Hotel Address")]
        [Required(ErrorMessage = "{1} is required.")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "{0} must be between {1} and {2} characters in length.")]
        public string Address { get; set; }

        [Display(Name = "Hotel Rating")]
        [Required(ErrorMessage = "{1} is required.")]
        [Range(1, 5, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double Rating { get; set; }

        [Required]
        public int CountryId { get; set; }
    }

    public class HotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }
        public CountryDTO Country { get; set; }
    }

}
