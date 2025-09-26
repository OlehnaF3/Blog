namespace ProjBlog
{

    public static class Configuration
    {
        public static IConfigurationRoot? Config => new ConfigurationBuilder()
            .AddJsonFile($"{Environment.CurrentDirectory}/app.json")
            .AddEnvironmentVariables()
            .Build();
    }
}
