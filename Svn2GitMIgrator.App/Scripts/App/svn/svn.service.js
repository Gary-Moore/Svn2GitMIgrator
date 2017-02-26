(function () {
    angular.module('migrator.svn').factory('svnService', svnService);

    svnService.$inject = ['$http','$q'];

    function svnService($http, $q) {
        return {
            search: search
        };

        function search(model) {
            var url = '/Home/Search',
                data = model;

            var deferred = $q.defer();

            $http({
                url: url,
                method: "POST",
                data: data
            }).then(function (result) {
                deferred.resolve(result.data);
            }, function (error) {
                deferred.reject(error);
            });

            return deferred.promise;

            return hopHttp.get(url, { id: oralQuestionId });
        }
    }
})();