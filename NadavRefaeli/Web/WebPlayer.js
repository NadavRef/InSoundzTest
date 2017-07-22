"use strict";

var app = angular.module("WebPlayerApp", []);
app.controller("mainController", function ($scope, apiService) {

    $scope.generateMovie = generateMovie;
    $scope.downloadMovie = downloadMovie;

    function downloadMovie() {
        apiService.Get(getMovieName($scope.currentMovie));

        function getMovieName(fullPath) {
            return fullPath.substring(fullPath.lastIndexOf("Get"));
        }
    }

    function generateMovie() {
        if ($scope.SelectedVideo == undefined || $scope.SelectedAudio == undefined) {
            alert("Selected video or audio is invalid");
            return;
        }

        var args = getArguments();

        var promise = apiService.Post("GenerateMovie", JSON.stringify(args));
        promise.then(function (response) {
            var video = document.getElementById("videoPlayer");
            var source = document.getElementById("videoSource");
            video.pause();
            $scope.currentMovie = response;
            source.setAttribute("src", $scope.currentMovie);
            video.load();
            video.play();
        });

        function getArguments() {
            var secondVideo = undefined;
            if ($scope.twoVideosMode) {
                secondVideo = {
                    Source: $scope.SelectedVideo2,
                    Offset: $scope.SelectedVideoOffset2,
                    StartClip: $scope.SelectedVideoStartTime2,
                    StopClip: $scope.SelectedVideoEndTime2
                }
            }
           return {
                Videos: [{
                    Source: $scope.SelectedVideo,
                    Offset: $scope.SelectedVideoOffset,
                    StartClip: $scope.SelectedVideoStartTime,
                    StopClip: $scope.SelectedVideoEndTime
                },
                secondVideo
                ],
                Audios: [{
                    Source: $scope.SelectedAudio,
                    Offset: $scope.SelectedAudioOffset,
                    StartClip: $scope.SelectedAudioStartTime,
                    StopClip: $scope.SelectedAudioEndTime
                }]
            };
        }
    }

    (function initalize() {
        var promise = apiService.Get("InitalizeWebData");
        promise.then(function (response) {
            if (response == undefined) {
                return;
            }

            $scope.AvailableVideos = response.VideosNames;
            $scope.AvailableAudios = response.AudiosNames;
            $scope.SelectedVideo = response.VideosNames[0];
            $scope.SelectedVideo2 = response.VideosNames[0];
            $scope.SelectedAudio = response.AudiosNames[0];
        });
    })();
});

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';
}]);