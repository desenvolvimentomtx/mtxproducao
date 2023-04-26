function chamarTelaMTXHome(opcao) {
    var controller = opcao;
    bloqueioTela();
    $.ajax({

        data: { controller: controller },
        types: "GET",
        processData: true,
        success: function () {

            window.location.href = '/Home/' + controller;

        }

    });

};

function chamarTelaMTX(opcao) {
    var controller = opcao;

    bloqueioTela();

    $.ajax({

        data: { controller: controller },
        types: "GET",
        processData: true,
        success: function () {

            window.location.href = '/TributacaoMTX/' + controller;

        }

    });

};

function bloqueioTela() {

    $.blockUI({
        message: '<h4><i style="font-size:1.3rem;" class="fa fa-spinner fa-pulse fa-3x fa-fw"></i> Aguarde...</h4>',
        css: {
            border: 'none',
            padding: '12px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });

}

/*Liberar a tela apos a execução do codigo dentro da chamada ajax*/
$(document).ajaxStop($.unblockUI);