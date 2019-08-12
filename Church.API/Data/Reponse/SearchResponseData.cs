namespace Church.API.Data.Reponse
{
    public class SearchResponseData<T>
    {

        public string status { get; set; }
        public T data { get; set; }
        public string message { get; set; }


        public SearchResponseData()
        { }

        public SearchResponseData(string status, T data, string message)
        {
            this.status = status;
            this.message = message;
            this.data = data;
        }

        public SearchResponseData<T> GetResponse<T>(string status, T data, string message)
        {
            this.status = status;
            this.message = message;

            var resultData = new SearchResponseData<T>(status, data, message);

            return resultData;
        }
    }
    
}