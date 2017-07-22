namespace InSoundzTest
{
    public static class Constants
    {
        public const string VideoCodec = "h264";
        public const string AudioCodec = "aac";
        public const string VideoPrefix = "camera";
        public const string AudioPrefix = "mic";
        public const string UrlPrefix = "http://localhost:";
        public const string OutputExtension = ".mp4";
        public const string FfmpegArgumentsUnformatted = " -itsoffset {0} -ss {1} -t {2} -i {3} -itsoffset {4} -ss {5} -t {6} -i {7} -c copy -map 0:v:0 -map 1:a:0 {8}";
        public const string FfmpegArgumentsMultiUnformatted = " -itsoffset {0} -ss {1} -t {2} -i {3} -itsoffset {4} -ss {5} -t {6} -i {7} -itsoffset {8} -ss {9} -t {10} -i {11} -filter_complex hstack {12}";
        public const string InitalizeWebCommand = "/InitalizeWebData";
        public const string GenerateMovieCommand = "/GenerateMovie";
        public const string GetMovieCommand = "GetMovie";
        public const string DefaultContentType = "application/json";
        public const int MaxAmountOfVideos = 2;
    }
}
