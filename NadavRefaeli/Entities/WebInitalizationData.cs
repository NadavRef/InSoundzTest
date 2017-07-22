using System.Collections.Generic;

namespace InSoundzTest.Entities
{
    public class WebInitalizationData
    {
        public List<string> AudiosNames { get; set; }
        public List<string> VideosNames { get; set; }

        public static WebInitalizationData GetData()
        {
            const bool getOnlyName = true;
            return new WebInitalizationData()
            {
                AudiosNames = Hollywood.GetAllAvailableAudios(getOnlyName),
                VideosNames = Hollywood.GetAllAvailableVideos(getOnlyName)
            };
        }

        private WebInitalizationData()
        {
        }
    }
}
