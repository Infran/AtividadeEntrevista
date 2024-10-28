
$(document).ready(function () {
    var id = window.location.pathname.split('/').reverse()[0]

    if (isNaN(id))
        id = '0'

    if (!obj) {
        $.ajax({
            url: urlListar + '/' + id,
            method: "GET",
            error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON);
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                },
            success:
                function (r) {
                    obj = r;
                }
        });
    }

    console.log('obj = ', obj);

    if (document.getElementById("gridBeneficiarios"))
        $('#gridBeneficiarios').jtable({
            actions: {
                listAction: urlListar,
            },
            fields: {
                CPF: {
                    title: 'CPF',
                    width: '35%'
                },
                Nome: {
                    title: 'Nome',
                    width: '50%'
                },
                Acoes: {
                    title: 'Ações',
                    display: function (data) {
                        return '<button onclick="window.location.href=\'' + urlAlteracao + '/' + data.record.Id + '\'" class="btn btn-primary btn-sm">Alterar</button>';
                    }
                }
            }
        });

    ////Load student list from server
    //if (document.getElementById("gridBeneficiarios"))
    //    $('#gridBeneficiarios').jtable('load');
})