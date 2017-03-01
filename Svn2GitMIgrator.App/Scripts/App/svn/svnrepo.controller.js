(function () {
    'use strict';

    angular.module('migrator.svn').controller('SvnRepoController', SvnRepoController);

    SvnRepoController.$inject = ['svnService'];

    function SvnRepoController(svnService) {
        var vm = this;
        vm.init = init;
        vm.search = search;
        vm.migrate = migrate;
        vm.navigate = navigate;

        vm.init();

        function init() {
            vm.model = {}
        }

        function migrate(repoUrl) {
            vm.model.repositorylUrl = repoUrl;
            svnService.migrate(vm.model).then(function(result) {
                
            });
        }

        function navigate(url) {
            vm.model.rootUrl = url;
            vm.search();
        }

        function search() {
            svnService.search(vm.model).then(function (result) {
                vm.repos = result;
            });

        }
    }
})();