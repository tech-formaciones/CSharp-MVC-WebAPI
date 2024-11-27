var DemoApp = {
    Core: {},
    Properties: {},
    Modules: {
        Customers: {
            Init: function () {
                $('table').DataTable({
                    "language": {
                        "url": "//cdn.datatables.net/plug-ins/1.10.19/i18n/Spanish.json"
                    }
                });
            }
        }
    }
};
