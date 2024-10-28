var beneficiarios = []
$(document).ready(function () {
    $('.cpf').mask('000.000.000-00', { reverse: true });

    function atualizarDadosTabela() {
        $('#dadosTabelaBeneficarios').empty();
        beneficiarios.forEach((beneficiario, index) => {
            $('#dadosTabelaBeneficarios').append('                                                                                      ' +
            '   <tr>                                                                                                                    ' +
            '       <td>' + beneficiario.CPF + '</td>                                                                                   ' +
            '       <td>' + beneficiario.Nome + '</td>                                                                                  ' +
            '       <td>                                                                                                                ' +
            '           <button type="button" class="btn btn-primary" onclick="alterarBeneficiario(' + index + ')">Alterar</button>     ' +
            '           <button type="button" class="btn btn-primary" onclick="excluirBeneficiario(' + index + ')">Excluir</button></td>' +
            '   </tr>                                                                                                                   ' +
            '');                                                                                                                    
        });                                                                                                                        
    }

    $('#formBeneficiario').submit(function (e) {
        e.preventDefault();

        beneficiario = {
            CPF: $('#CPFBeneficiario').val().replace(/\D/g, ''),
            Nome: $('#NomeBeneficiario').val()
        }

        if (beneficiarios.some(b => b.CPF === beneficiario.CPF)) {
            ModalDialog("Erro", "Este CPF já foi adicionado para o beneficiário.");
            return;
        }

        beneficiarios.push(beneficiario);

        atualizarDadosTabela();

        $("#formBeneficiario")[0].reset();
    });

    $('#formCadastro').submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: urlPost + '/',
            method: "POST",
            data: {
                "NOME": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "CPF": $(this).find("#CPF").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "Beneficiarios": beneficiarios
            },
            error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON);
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                },
            success:
                function (r) {
                    ModalDialog("Sucesso!", r)
                    $("#formCadastro")[0].reset();
                    $("#formBeneficiario")[0].reset();
                    beneficiarios = [];
                }
        });
    })

    window.alterarBeneficiario = function (index) {
        ModalDialog("Aviso", "Implementar alteração");
    }

    window.excluirBeneficiario = function (index) {
        beneficiarios.splice(index, 1);
        atualizarDadosTabela();
    }
})

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}
