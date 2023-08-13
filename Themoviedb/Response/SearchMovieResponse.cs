using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace PFI_MOVIE_SEARCH.Themoviedb.Response
{
    public class SearchMovieResponse
    {
        public int page { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
        public List<Movie> results { get; set; }
    }

    public class Movie
    {
        public bool adult { get; set; }
        public List<int> genre_ids { get; set; }
        public int id { get; set; }
        public string original_language { get;set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public string release_date { get; set; }
        public string title { get; set; }
        public decimal vote_average { get; set; }

    }
}
