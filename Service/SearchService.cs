using Microsoft.Extensions.Configuration;
using PFI_MOVIE_SEARCH.Models;
using PFI_MOVIE_SEARCH.Themoviedb;
using PFI_MOVIE_SEARCH.Themoviedb.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace PFI_MOVIE_SEARCH.Service
{
    public class SearchService
    {
        private Connector connector { get;set; }
        private string url { get; set; }
        private string accessToken { get; set; }
        public SearchService()
        {
            IConfigurationSection config = Program.configuration.GetSection("Themoviedb");
            if (config != null)
            {
                url = config["Url"];
                accessToken = config["Token"];
            }
            connector = new Connector(url, accessToken);
        }
        public ResultDto<searchResult> GetMovies(string title, string language)
        {
            int error = (int)ErrorCodes.NotFound;
            searchResult searchResult = null;
            SearchMovieResponse response = searchMovie(title, language, out HttpStatusCode status);
            if(response != null && response.results!=null && response.results.Count > 0)
            {
                error = (int)ErrorCodes.Success;
                var movie = response.results.FirstOrDefault();
                searchResult = new searchResult()
                {
                     title = movie.title,
                     original_title = movie.original_title,
                     overview = movie.overview,    
                     release_date = movie.release_date,                     
                     vote_average = movie.vote_average,
                     similar_movies = GetSimilarMovies(movie.genre_ids, movie.id, language)
                };
            }
            else
            {
                if (status == HttpStatusCode.Unauthorized) error = (int)ErrorCodes.AuthorizationError;
                if (status != HttpStatusCode.OK) error = (int)ErrorCodes.NoConnection;
            }
            
            return new ResultDto<searchResult>()
            {
                 ReturnCode = error,
                 Description=((ErrorCodes)error).ToString(), 
                 Result = searchResult
            };
        }

        private string GetSimilarMovies(List<int> genre_ids, int id, string language)
        {
            int error = (int)ErrorCodes.NotFound;
            string similar_movies = "";
            SearchMovieResponse response = discoverMovie(genre_ids, language);
            if(response!=null && response.results!=null && response.results.Count > 0)
            {
                int num = 0;
                foreach(var movie in response.results)
                {      
                    if(movie.id == id) continue; 
                    similar_movies += string.Format("{0} ({1}), ", movie.title, GetYear(movie.release_date));
                    num++;
                    if (num == 5) break;
                }
            }
            similar_movies = similar_movies.Substring(0, similar_movies.LastIndexOf(","));
            return similar_movies;
        }

        private string GetYear(string date, string format= "yyyy-MM-dd")
        {
            string year = "";
            try
            {
                DateTime dateTime = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
                year = dateTime.Year.ToString();
            }
            catch (Exception ex)
            {}
            return year;
        }

        private SearchMovieResponse searchMovie(string title, string language, out HttpStatusCode status)
        {
            
            string path = "?query=" + title+ "&language="+ language;
            SearchMovieResponse response = connector.Request<SearchMovieResponse>(out status, "movie", "search", "GET", null, path);
            return response;
        }

        private SearchMovieResponse discoverMovie(List<int> genre_ids, string language)
        {            
            string path = "?with_genres=";
            for(int i=0;i < genre_ids.Count;  i++)
            {
                path += genre_ids[i];
                if (i < genre_ids.Count - 1) path += ",";
            }
            path += "&language=" + language;
            SearchMovieResponse response = connector.Request<SearchMovieResponse>(out HttpStatusCode status, "movie", "discover", "GET", null, path);
            return response;
        }
    }
}
