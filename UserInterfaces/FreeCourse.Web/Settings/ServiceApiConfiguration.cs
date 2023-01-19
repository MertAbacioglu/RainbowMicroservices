namespace FreeCourse.Web.Settings
{
    public class ServiceApiConfiguration
    {
        public string IdentityBaseUri { get; set; }
        public string GetewayBaseUri { get; set; }
        
        public string PhotoStockUri { get; set; }
        public ServiceApi Catalog { get; set; }
        public ServiceApi PhotoStock { get; set; }
    }

    public class ServiceApi
    {
        public string Path { get; set; }
    }
}
