using InSoundzTest.API;

namespace InSoundzTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            var configuration = ConfigurationManager.GetConfiguration();

            Hollywood.Initalize(configuration);

            var httpService = HttpService.CreateNew(configuration);
            httpService.StartService();
        }
    }
}
