(function () {
    'use strict';

    angular.module('migrator.svn').controller('SvnRepoController', SvnRepoController);

    SvnRepoController.$inject = ['svnService'];

    function SvnRepoController(svnService) {
        var vm = this;
        vm.init = init;
        vm.search = search;
        vm.migrate = migrate;

        vm.init();

        function init() {
            vm.model = {}
        }

        function migrate(repoUrl) {
            vm.model.repositorylUrl = repoUrl;
            svnService.migrate(vm.model);
        }

        function search() {
            svnService.search(vm.model).then(function (result) {
                vm.repos = result;
            });

        }
    }
})();