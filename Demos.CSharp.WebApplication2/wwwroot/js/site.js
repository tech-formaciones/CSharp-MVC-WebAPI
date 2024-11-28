var DemoApp = {
    Core: {
        CreateTable: function (selector) {
            $(selector).DataTable({
                "language": {
                    "url": "//cdn.datatables.net/plug-ins/1.10.19/i18n/Spanish.json"
                }
            });
        },
        Message: {
            Show: function (message, icon) {
                Swal.fire({
                    title: "Northwind Inc.",
                    text: message,
                    icon: icon
                });
            }
        }
    },
    Properties: {},
    Modules: {
        Customers: {
            Init: function () {
                DemoApp.Core.CreateTable('table');
            }
        },
        Orders: {
            Init: function () {
                DemoApp.Core.CreateTable('table');
            }
        }
    }
};
