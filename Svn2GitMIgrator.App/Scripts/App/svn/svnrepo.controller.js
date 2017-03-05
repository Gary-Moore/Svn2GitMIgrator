(function () {
    'use strict';

    angular.module('migrator.svn').controller('SvnRepoController', SvnRepoController);

    SvnRepoController.$inject = ['svnService', 'localStorageService', '$uibModal'];

    function SvnRepoController(svnService, localStorageService, $uibModal) {
        var vm = this;
        vm.init = init;
        vm.search = search;
        vm.saveSettings = saveSettings;
        vm.migrate = migrate;
        vm.navigate = navigate;
        vm.navigateBack = navigateBack;
        vm.openMigrateModal = openMigrateModal;

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

        function navigateBack(url) {
            url = url.substring(0, url.lastIndexOf("/"));
            url = url.substring(0, url.lastIndexOf("/") + 1);
            vm.model.rootUrl = url;
            vm.search();
        }

        function openMigrateModal() {
            var modalInstance = $uibModal.open({
                templateUrl: 'migrateRepo.html'
            });
        }

        function saveSettings() {
            if(localStorageService.set('settings', vm.model)){
                toastr.success('Settings updated');
            }
        }

        function search() {
            svnService.search(vm.model).then(function (result) {
                if (!result.Error) {
                    vm.repos = result.Data;
                } else {
                    toastr.error(result.Message, "Error retrieving repo info:");
                }                
            });

        }
    }
})();