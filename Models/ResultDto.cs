namespace PFI_MOVIE_SEARCH.Models
{
    public class ResultDto<T>
    {
        public int ReturnCode { get; set; }
        public string Description { get; set; }
        public T Result { get; set; }        
    }
}
