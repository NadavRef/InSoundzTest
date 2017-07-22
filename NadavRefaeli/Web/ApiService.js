app.service("apiService", function ($http) {
    var url = "http://localhost:888/";
    this.Get = function (command) {
        return $http.get(url + command)
             .then(function (response) {
                 return response.data;
             });
    }
    this.Post = function (command, args) {
        return $http.post(url + command, args)
             .then(function (response) {
                 return response.data;
             });
    }
});