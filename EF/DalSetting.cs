namespace SecretsSharingAPI.EF
{
    public class DalSetting
    {
        public string ConnectionString { get; }

        public DalSetting(IConfiguration configuration)
        {
            ConnectionString = configuration.GetSection("Dal").GetValue<string>("ConnectionString");
        }
    }
}
