using Newtonsoft.Json;

namespace UnitTest.Bithumb.Entity
{
    public class RequestData<T>
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}