(function () {
    'use strict';

    angular
        .module('app')
        .controller('menu', menu);

    menu.$inject = ['$location'];

    function menu($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'menu';

        activate();

        function activate() { }
    }
})();
