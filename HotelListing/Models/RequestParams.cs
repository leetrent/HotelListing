using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Models
{
    public class RequestParams
    {
        public int PageNumber { get; set; } = 1;

        ////////////////////////////////////////////////////////////////////
        // PAGE SIZE: BEGIN
        ////////////////////////////////////////////////////////////////////
        const int MAX_PAGE_SIZE = 50;
        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
            }
        }
        ////////////////////////////////////////////////////////////////////
        // PAGE SIZE: END
        ////////////////////////////////////////////////////////////////////

    }
}
