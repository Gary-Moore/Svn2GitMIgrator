(function () {
    'use strict';

    angular.module('migrator.svn').controller('MigrationModalController', MigrationModalController);

    MigrationModalController.$inject = ['$uibModalInstance', 'svnService', 'model'];
    
    function MigrationModalController($uibModalInstance, svnService, model) {
        var vm = this;
        vm.model = model;
        vm.cancel = cancel;
        vm.migrate = migrate;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function migrate() {
            
            $uibModalInstance.close(model);
        }
    }
})();