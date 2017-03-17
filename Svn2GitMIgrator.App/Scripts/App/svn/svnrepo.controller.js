(function () {
    'use strict';

    angular.module('migrator.svn').controller('SvnRepoController', SvnRepoController);

    SvnRepoController.$inject = ['svnService', 'localStorageService', '$uibModal', '$scope', 'Hub'];

    function SvnRepoController(svnService, localStorageService, $uibModal, $scope, Hub) {
        var vm = this;
        vm.init = init;
        vm.search = search;
        vm.saveSettings = saveSettings;
        vm.navigate = navigate;
        vm.navigateBack = navigateBack;
        vm.openMigrateModal = openMigrateModal;

        var migrationHub;

        vm.init();

        function init() {
            vm.model = {};
            vm.messages = "";
            vm.model = localStorageService.get('settings');
            vm.settingsCollapsed = true;

            migrationHub = new Hub('migrationHub', {
                listeners: {
                    'progress': function (message) {
                        vm.messages += message;
                        $scope.$apply();
                    }
                },
                errorHandler: function (error) {
                    console.error(error);
                }
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

        function openMigrateModal(repoUrl) {
            vm.model.repositorylUrl = repoUrl;
            var modalInstance = $uibModal.open({
                templateUrl: 'migrateRepo.html',
                controller: 'MigrationModalController',
                controllerAs: 'vm',
                resolve: {
                    model: function () {
                        return vm.model;
                    }
                }
            });

            modalInstance.result.then(function (model) {
                vm.showProgress = true;
                vm.messages = "";
                svnService.migrate(model).then(function (result) {
                    if (!result.Error) {
                        toastr.success('Migration Complete');
                    } else {
                        toastr.error(result.Message, "Migration process failed:");
                    }
                });
            }, function () {
                toastr.error('Migration Process failed due to an error');
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