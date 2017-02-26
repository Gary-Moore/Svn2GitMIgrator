(function () {
    'use strict';

    angular.module('migrator.svn').controller('SvnRepoController', SvnRepoController);

    SvnRepoController.$inject = ['svnService'];

    function SvnRepoController(svnService) {
        var vm = this;
        vm.search = search;
        vm.init = init;

        vm.init();

        function init() {
            vm.model = {}
        }

        function search() {
            svnService.search(vm.model).then(function (result) {
                vm.repos = result;
            });

        }
    }
})();