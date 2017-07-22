using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Homework1.API;

namespace Homework1
{
    public static class Hollywood
    {
        public static string Output { get; set; }
        private static string FfmpegPath { get; set; }
        private static string ResourcesDirectory { get; set; }

        public static void Initalize(Configuration configuration)
        {
            Output = configuration.OutputPath;
            ResourcesDirectory = configuration.ResourcesDirectory;
            FfmpegPath = configuration.FfmpegPath;
        }

        public static string CreateNewMovie(MakeMovieRequest request)
        {
            var videoPaths = GetMediaPaths(request.Videos, true);
            var audioPaths = GetMediaPaths(request.Audios, false);

            if (videoPaths == null || !videoPaths.Any() ||
                audioPaths == null || !audioPaths.Any())
            {
                // logging
                return null;
            }

            var fileName = DateTime.UtcNow.ToBinary();
            string arguments;
            if (videoPaths.Count == Constants.MaxAmountOfVideos)
            {
                arguments = GeneateArgumentsForMultipleVideos(request, fileName, videoPaths, audioPaths);
            }
            else
            {
                arguments = GeneateArgumentsForSingleVideo(request, fileName, videoPaths, audioPaths);
            }
            
            Task.Factory.StartNew(() =>
            {
                var proc = Process.Start(FfmpegPath, arguments);
                if (proc == null)
                {
                    return;
                }
                proc.WaitForExit();
            }).Wait();

            return string.Concat(fileName, Constants.OutputExtension);
        }

        private static string GeneateArgumentsForMultipleVideos(MakeMovieRequest request, long fileName, List<string> videoPaths, List<string> audioPaths)
        {
            var videoSrc = videoPaths[0];
            var videoSrc2 = videoPaths[1];
            var audioSrc = audioPaths.First();
            var outputFileName = string.Concat(Output, fileName, Constants.OutputExtension);

            var requestedVideoMetadata = request.Videos[0];
            var requestedVideoMetadata2 = request.Videos[1];
            var requestedAudioMetadata = request.Audios.First();

            var videoDuration = requestedVideoMetadata.StopClip - requestedVideoMetadata.StartClip;
            var videoDuration2 = requestedVideoMetadata2.StopClip - requestedVideoMetadata2.StartClip;
            var audioDuration = requestedAudioMetadata.StopClip - requestedAudioMetadata.StartClip;

            if (videoDuration == default(int))
            {
                videoDuration = int.MaxValue;
            }

            if (videoDuration2 == default(int))
            {
                videoDuration2 = int.MaxValue;
            }

            if (audioDuration == default(int))
            {
                audioDuration = int.MaxValue;
            }

            return string.Format(Constants.FfmpegArgumentsMultiUnformatted,
                requestedVideoMetadata.Offset,
                requestedVideoMetadata.StartClip,
                videoDuration,
                videoSrc,
                requestedVideoMetadata2.Offset,
                requestedVideoMetadata2.StartClip,
                videoDuration2,
                videoSrc2,
                requestedAudioMetadata.Offset,
                requestedAudioMetadata.StartClip,
                audioDuration,
                audioSrc,
                outputFileName);
        }

        private static string GeneateArgumentsForSingleVideo(MakeMovieRequest request, long fileName, List<string> videoPaths, List<string> audioPaths)
        {
            var videoSrc = videoPaths.First();
            var audioSrc = audioPaths.First();
            var outputFileName = string.Concat(Output, fileName, Constants.OutputExtension);

            var requestedVideoMetadata = request.Videos.First();
            var requestedAudioMetadata = request.Audios.First();

            var videoDuration = requestedVideoMetadata.StopClip - requestedVideoMetadata.StartClip;
            var audioDuration = requestedAudioMetadata.StopClip - requestedAudioMetadata.StartClip;

            if (videoDuration == default(int))
            {
                videoDuration = int.MaxValue;
            }

            if (audioDuration == default(int))
            {
                audioDuration = int.MaxValue;
            }

            return string.Format(Constants.FfmpegArgumentsUnformatted,
                requestedVideoMetadata.Offset,
                requestedVideoMetadata.StartClip,
                videoDuration, 
                videoSrc,
                requestedAudioMetadata.Offset,
                requestedAudioMetadata.StartClip,
                audioDuration,
                audioSrc,
                outputFileName);
        }

        public static List<string> GetAllAvailableVideos(bool getOnlyName)
        {
            return GetPathsThatStartWith(Constants.VideoPrefix, getOnlyName);
        }

        public static List<string> GetAllAvailableAudios(bool getOnlyName)
        {
            return GetPathsThatStartWith(Constants.AudioPrefix, getOnlyName);
        }

        private static List<string> GetMediaPaths(List<MediaData> requestedData, bool isVideo)
        {
            const bool getOnlyName = false;
            var results = new List<string>();
            var availablePaths = isVideo ? GetAllAvailableVideos(getOnlyName) : GetAllAvailableAudios(getOnlyName);
            if (availablePaths == null)
            {
                // logg
                return null;
            }
            foreach (var data in requestedData)
            {
                if (data == null)
                {
                    continue;
                }
                if (availablePaths.Exists(path => path.Contains(data.Source)))
                {
                    results.Add(availablePaths.Find(path => path.Contains(data.Source)));
                }
            }
            return results;
        }

        private static List<string> GetPathsThatStartWith(string startWithPath, bool getOnlyName)
        {
            if (!Directory.Exists(ResourcesDirectory))
            {
                // Logging error
                return null;
            }

            var rawPaths = Directory.GetFiles(ResourcesDirectory, $"{startWithPath}*",
                SearchOption.AllDirectories);

            return getOnlyName ? rawPaths.Select(p => p.Remove(0, p.LastIndexOf("\\", StringComparison.Ordinal) + 1)).ToList()
                               : rawPaths.ToList();
        }
    }
}
