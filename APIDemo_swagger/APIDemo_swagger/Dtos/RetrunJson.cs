namespace APIDemo_swagger.Dtos
{
    public class RetrunJson
    {
        public dynamic Data { get; set; }
        public int HttpCode { get; set; }
        public string ErrorMessage { get; set; }
        public dynamic Error { get; set; }
    }
}
