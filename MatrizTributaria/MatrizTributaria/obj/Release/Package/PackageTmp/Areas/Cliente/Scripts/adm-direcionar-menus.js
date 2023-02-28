////$(document).ready(function () {
////    btnExecutar = document.getElementsByName("getTelaTrib"); //botao para confirmar a edição dos selecionados

////    //btnExecutar2 = document.getElementById("getTelaTrib2");
////    //var controller2 = btnExecutar2.value;
////    var controler = btnExecutar.value;

////    //fiz alterações aqui de novo
////    $('getTelaTrib').click(function (opcao) {

////        if (btnExecutar.value == 'AnaliseTributaria')
////        {
////            bloqueioTela();//bloqueia tela

////            //agora o ajax
////            $.ajax({

////                data: { controler: controler },
////                types: "GET",
////                processData: true,
////                success: function () {

////                    window.location.href = '/Cliente/TributacaoEmpresa/' + controler;

////                }

////            });

////        }
       


////    });
////    //$('#getTelaTrib2').click(function (opcao) {

      
////    //    if (btnExecutar2.value == 'AnaliseRedBaseCalSai') {
////    //        bloqueioTela();//bloqueia tela

////    //        //agora o ajax
////    //        $.ajax({

////    //            data: { controller2: controller2 },
////    //            types: "GET",
////    //            processData: true,
////    //            success: function () {

////    //                window.location.href = '/Cliente/TributacaoEmpresa/' + controller2;

////    //            }

////    //        });

////    //    }


////    //});



////});

//Executa a chamada da tela de acordo com a controller definida no botão
function chamarTela(opcao)
{
    var controller = opcao;
    bloqueioTela();
    $.ajax({

        data: { controller: controller },
                types: "GET",
                processData: true,
                success: function () {

                    window.location.href = '/Cliente/TributacaoEmpresa/' + controller;

                }

            });

};