using System.Collections.Generic;
using System.Linq;
using InSoundzTest.Entities;

namespace InSoundzTest.API
{
    public class MakeMovieRequest
    {
        public List<MediaData> Videos { get; set; }
        public List<MediaData> Audios { get; set; }

        public bool IsValid => Videos != null && Videos.Any() &&
                               Audios !=null && Audios.Any();
    }
}
