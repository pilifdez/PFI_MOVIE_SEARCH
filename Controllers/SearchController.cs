using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PFI_MOVIE_SEARCH.Models;
using PFI_MOVIE_SEARCH.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFI_MOVIE_SEARCH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAnyCorsPolicy")]
    public class SearchController : Controller
    {

        [HttpGet("Movies")]
        public ResultDto<searchResult> GetMovies(string title, string language = "")
        {
            SearchService service = new SearchService();
            return service.GetMovies(title, language);
        }
    }
}
