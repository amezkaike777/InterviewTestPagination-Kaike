(function (angular) {
    "use strict";

    angular.module("todoApp")
        .directive("todoPaginatedList", [todoPaginatedList])
        .directive("pagination", [pagination]);

    function todoPaginatedList() {
        return {
            restrict: "E",
            templateUrl: "app/templates/todo.list.paginated.html",
            scope: {},
            controller: ["$scope", "$http", controller]
        };

        function controller($scope, $http) {
            // Estado inicial: 20 itens por página, ordenados pela data mais recente.
            $scope.todos = [];
            $scope.page = 1;
            $scope.pageSize = 20;
            $scope.totalItems = 0;
            $scope.totalPages = 1;
            $scope.sortBy = "createdDate";
            $scope.sortDirection = "desc";
            $scope.hasSorted = false;
            $scope.loading = false;
            $scope.error = "";

            // Cada mudança de página pede ao servidor somente os registros necessários.
            $scope.loadPage = function (page, pageSize) {
                if ($scope.loading)
                    return;

                $scope.loading = true;
                $scope.error = "";

                $http.get("api/todo/todos", {
                    params: {
                        page: page,
                        pageSize: pageSize,
                        sortBy: $scope.sortBy,
                        sortDirection: $scope.sortDirection
                    }
                }).then(function (response) {
                    // A API informa os registros e os totais da página efetivamente retornada.
                    $scope.todos = response.data.items;
                    $scope.page = response.data.page;
                    $scope.pageSize = response.data.pageSize;
                    $scope.totalItems = response.data.totalItems;
                    $scope.totalPages = response.data.totalPages;
                }, function () {
                    // Limpa resultados antigos para não mostrar dados de outra página após uma falha.
                    $scope.todos = [];
                    $scope.page = 1;
                    $scope.pageSize = pageSize;
                    $scope.totalItems = 0;
                    $scope.totalPages = 1;
                    $scope.error = "Could not load the tasks. Please try again.";
                }).finally(function () {
                    // O carregamento termina tanto no sucesso quanto no erro.
                    $scope.loading = false;
                });
            };

            // O primeiro clique ordena de forma crescente. Outro clique no mesmo campo inverte.
            $scope.changeSort = function (property) {
                if ($scope.loading)
                    return;

                if (!$scope.hasSorted || $scope.sortBy !== property) {
                    $scope.sortDirection = "asc";
                } else if ($scope.sortDirection === "asc") {
                    $scope.sortDirection = "desc";
                } else {
                    $scope.sortDirection = "asc";
                }

                $scope.sortBy = property;
                $scope.hasSorted = true;
                $scope.loadPage(1, $scope.pageSize);
            };

            $scope.loadPage($scope.page, $scope.pageSize);
        }
    }

    function pagination() {
        return {
            restrict: "E",
            templateUrl: "app/templates/pagination.html",
            // '=' compartilha um valor. '&' permite chamar a função passada pela lista.
            // Este componente não conhece Todo nem uma URL: ele serve para outras listas.
            scope: {
                page: "=",
                pageSize: "=",
                totalItems: "=",
                totalPages: "=",
                loading: "=",
                onChange: "&"
            },
            controller: ["$scope", controller]
        };

        function controller($scope) {
            // Os valores são números. Zero será enviado à API para representar 'all'.
            $scope.pageSizes = [
                { value: 10, label: "10" },
                { value: 20, label: "20" },
                { value: 30, label: "30" },
                { value: 0, label: "all" }
            ];

            // Mantém os campos locais em sincronia com os valores confirmados pelo servidor.
            $scope.$watch("page", function (value) {
                $scope.requestedPage = value;
            });

            $scope.$watch("pageSize", function (value) {
                $scope.selectedPageSize = value;
            });

            // Os limites evitam navegar abaixo da primeira página ou acima da última.
            $scope.goToPage = function (page) {
                if ($scope.loading)
                    return;

                if (typeof page !== "number" || !isFinite(page) || Math.floor(page) !== page) {
                    $scope.requestedPage = $scope.page;
                    return;
                }

                if (page < 1)
                    page = 1;

                if (page > $scope.totalPages)
                    page = $scope.totalPages;

                $scope.requestedPage = $scope.page;

                if (page !== $scope.page)
                    $scope.onChange({ page: page, pageSize: $scope.pageSize });
            };

            // Ao mudar o tamanho, volta para a primeira página e mantém a ordenação da lista.
            $scope.changePageSize = function () {
                if (!$scope.loading)
                    $scope.onChange({ page: 1, pageSize: $scope.selectedPageSize });
            };
        }
    }
})(angular);
