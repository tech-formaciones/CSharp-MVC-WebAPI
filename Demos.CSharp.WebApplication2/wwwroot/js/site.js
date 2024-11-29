var DemoApp = {
    Core: {
        CreateTable: function (selector) {
            DemoApp.Properties.Table = $(selector).DataTable({
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
    Properties: {
        Table: null
    },
    Modules: {
        Customers: {
            Init: function () {
                DemoApp.Core.CreateTable('table');
            },
            Delete: {
                Process: function (name, id) {
                    $.ajax({
                        method: 'POST',
                        url: '/customers/delete/' + id,
                        success: function (data, textStatus, jqXHR) {
                            console.log(data);

                            if (data.result == 'OK') {
                                DemoApp.Core.Message.Show(name + ' eliminado correctamente.', 'info');

                                let row = $('tbody tr[data-customer="' + id + '"]');
                                DemoApp.Properties.Table.row(row).remove().draw();
                            }
                            else if (data.result == 'NOK') DemoApp.Core.Message.Show(name + ' eliminado correctamente.', 'warning');
                            else DemoApp.Core.Message.Show(message, 'error');

                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            DemoApp.Core.Message.Show('Error: ' + textStatus, 'error');
                        }
                    });
                },
                Success: function (name, id) {
                    var $span = $('#dataresult');
                    var result = $span.data('result'); 
                    var message = $span.data('message'); 
                    var row = $('tbody tr[data-customer="' + id + '"]');

                    console.log('Name:', name);
                    console.log('ID:', id);
                    console.log('Result:', result);
                    console.log('Message:', message);

                    if (result == 'OK') {
                        DemoApp.Core.Message.Show(name + ' eliminado correctamente.', 'info');
                        DemoApp.Properties.Table.row(row).remove().draw();
                    }
                    else if (result == 'NOK') DemoApp.Core.Message.Show(name + ' eliminado correctamente.', 'warning');
                    else DemoApp.Core.Message.Show(message, 'error');
                }
            }
        },
        Orders: {
            Init: function () {
                DemoApp.Core.CreateTable('table');
            }
        }
    }
};
