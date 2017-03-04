(function () {
    'use strict';

    angular.module('migrator.svn').controller('SvnRepoController', SvnRepoController);

    SvnRepoController.$inject = ['svnService', 'localStorageService'];

    function SvnRepoController(svnService, localStorageService) {
        var vm = this;
        vm.init = init;
        vm.search = search;
        vm.saveSettings = saveSettings;
        vm.migrate = migrate;
        vm.navigate = navigate;

        vm.init();

        function init() {
            vm.model = {}
            vm.model = localStorageService.get('settings');
            vm.settingsCollapsed = true;
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

        function saveSettings() {
            return localStorageService.set('settings', vm.model);
        }

        function search() {
            svnService.search(vm.model).then(function (result) {
                vm.repos = result;
            });

        }
    }
})();