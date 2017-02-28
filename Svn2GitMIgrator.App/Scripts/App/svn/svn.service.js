(function () {
    angular.module('migrator.svn').factory('svnService', svnService);

    svnService.$inject = ['$http','$q'];

    function svnService($http, $q) {
        return {
            search: search,
            migrate: migrate
        };

        function migrate(repoBaseUrl) {
            var url = '/Home/MigrateRepo',
                data = repoBaseUrl;

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
        }

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
        }
    }
})();