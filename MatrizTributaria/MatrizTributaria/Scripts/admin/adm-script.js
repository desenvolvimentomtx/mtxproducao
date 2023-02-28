//MUDAR DADOS DE NCM EM MASSA: 10/12/2021
$(document).ready(function () {
    var btnAlterar = document.getElementById("mesmoNCM"); //botao
    //se for instanciado
    if (btnAlterar) {
        //click do botao
        btnAlterar.addEventListener("click", function () {
            var selecao = document.getElementById("selAlterar"); //pegar o elemento select
            var value = selecao.options[selecao.selectedIndex].value; //pegar o valor do select
            var inputCest = document.getElementById("cest"); //input do cest


            switch (value)
            {
                case "1":
                    document.getElementById('mudarValoresCest').style.display = 'none';
                    document.getElementById("selAlterar").focus();
                    inputCest.value = "";
                   
                    break;
                case "2":
                    document.getElementById('mudarValoresCest').style.display = 'block';
                    document.getElementById("selAlterar").focus();
                    document.getElementById("cest").removeAttribute("readonly");
                    inputCest.value = "";
                    break;
                case "3":
                    document.getElementById('mudarValoresCest').style.display = 'block';
                    document.getElementById("selAlterar").focus();
                    inputCest.value = "NULL";
                    document.getElementById("cest").setAttribute("readonly", true);

                    break;

                
            }


            //if (value == 1) {

            //   /* document.getElementById('mudarValoresNatRec').style.display = 'none';*/
            //   /* document.getElementById('mudarValoresSetorFecp').style.display = 'none';*/
            //    document.getElementById('mudarValoresCest').style.display = 'none';
                

            //   /* document.getElementById('mudarValoresTab').style.display = 'none';*/


            //    document.getElementById("selAlterar").focus();


            //} else {
            //   /* document.getElementById('mudarValoresTab').style.display = 'block';*/

            //    document.getElementById('mudarValoresCest').style.display = 'block';
            //    //document.getElementById('mudarValoresNatRec').style.display = 'flex';
            //    //document.getElementById('mudarValoresSetorFecp').style.display = 'block';
            //    document.getElementById("selAlterar").focus();

            //}

        });
    }






});


//atribuir valores de tributação quando o uf ja vem preenchido: 21/10/2022
$(document).ready(function () {
    var uforigem = document.getElementById("ufOrigem");
    var ufdestino = document.getElementById("ufDestino");

    var fecp = document.getElementById("fecp");
    var codReceita = document.getElementById("CodReceita");
    var idCstSaiPC = document.getElementById("idCstSaiPC");
    var alpS = document.getElementById("alpS");
    var alcS = document.getElementById("alcS");
    var idFundamentoLegal = document.getElementById("IdFundamentoLegal");

    var idCstVeVarCF = document.getElementById("idCstVeVarCF");
    var alVeVarCF = document.getElementById("alVeVarCF");
    var alVeVarCFSt = document.getElementById("alVeVarCFSt");
    var rBcVeVarCF = document.getElementById("rBcVeVarCF");
    var rBcSTVeVarCF = document.getElementById("rBcSTVeVarCF");

    var idCstVeVarCont = document.getElementById("idCstVeVarCont");
    var alVeVarCont = document.getElementById("alVeVarCont");
    var alVeVarContSt = document.getElementById("alVeVarContSt");
    var rBcVeVarCont = document.getElementById("rBcVeVarCont");
    var rBcSTVeVarCont = document.getElementById("rBcSTVeVarCont");

    var idCstVeAtaCont = document.getElementById("idCstVeAtaCont");
    var alVaC = document.getElementById("alVaC");
    var alVaCSt = document.getElementById("alVaCSt");
    var rBcVaC = document.getElementById("rBcVaC");
    var rBcSTVaC = document.getElementById("rBcSTVaC");

    var idCstVeAtaSN = document.getElementById("idCstVeAtaSN");
    var alVSN = document.getElementById("alVSN");
    var alVSNSt = document.getElementById("alVSNSt");
    var rBcVSN = document.getElementById("rBcVSN");
    var rBcSTVSN = document.getElementById("rBcSTVSN");

    var IdFundLegalSaidaICMS = document.getElementById("IdFundLegalSaidaICMS");


    if (ufOrigem.value != null && ufdestino.value != null)
    {
        ufdestino.setAttribute('disabled', true);
        uforigem.setAttribute('disabled', true);
        fecp.removeAttribute('readonly');

        codReceita.removeAttribute('disabled');

        idCstSaiPC.removeAttribute('disabled');
        alpS.removeAttribute('readonly');
        alcS.removeAttribute('readonly');
        idFundamentoLegal.removeAttribute('disabled');

        idCstVeVarCF.removeAttribute('disabled');

        alVeVarCF.removeAttribute('readonly');
        alVeVarCFSt.removeAttribute('readonly');
        rBcVeVarCF.removeAttribute('readonly');
        rBcSTVeVarCF.removeAttribute('readonly');
        idCstVeVarCont.removeAttribute('disabled');

        alVeVarCont.removeAttribute('readonly');
        alVeVarContSt.removeAttribute('readonly');
        rBcVeVarCont.removeAttribute('readonly');
        rBcSTVeVarCont.removeAttribute('readonly');
        idCstVeAtaCont.removeAttribute('disabled');
        alVaC.removeAttribute('readonly');
        alVaCSt.removeAttribute('readonly');

        idCstVeAtaSN.removeAttribute('disabled');
        alVSN.removeAttribute('readonly');
        alVSNSt.removeAttribute('readonly');
        rBcVSN.removeAttribute('readonly');
        rBcSTVSN.removeAttribute('readonly');
        IdFundLegalSaidaICMS.removeAttribute('disabled');

        rBcVaC.removeAttribute('readonly');
        rBcSTVaC.removeAttribute('readonly');

    }










});

//atribuir valores de tributação
//$(document).ready(function ()
//{
//    var uforigem = document.getElementById("ufOrigem");
//    var ufdestino = document.getElementById("ufDestino");

//    var fecp = document.getElementById("fecp");
//    var codReceita = document.getElementById("CodReceita");
//    var idCstSaiPC = document.getElementById("idCstSaiPC");
//    var alpS = document.getElementById("alpS");
//    var alcS = document.getElementById("alcS");
//    var idFundamentoLegal = document.getElementById("IdFundamentoLegal");

//    var idCstVeVarCF = document.getElementById("idCstVeVarCF");
//    var alVeVarCF = document.getElementById("alVeVarCF");
//    var alVeVarCFSt = document.getElementById("alVeVarCFSt");
//    var rBcVeVarCF = document.getElementById("rBcVeVarCF");
//    var rBcSTVeVarCF = document.getElementById("rBcSTVeVarCF");

//    var idCstVeVarCont = document.getElementById("idCstVeVarCont");
//    var alVeVarCont = document.getElementById("alVeVarCont");
//    var alVeVarContSt = document.getElementById("alVeVarContSt");
//    var rBcVeVarCont = document.getElementById("rBcVeVarCont");
//    var rBcSTVeVarCont = document.getElementById("rBcSTVeVarCont");

//    var idCstVeAtaCont = document.getElementById("idCstVeAtaCont");
//    var alVaC = document.getElementById("alVaC");
//    var alVaCSt = document.getElementById("alVaCSt");
//    var rBcVaC = document.getElementById("rBcVaC");
//    var rBcSTVaC = document.getElementById("rBcSTVaC");

//    var idCstVeAtaSN = document.getElementById("idCstVeAtaSN");
//    var alVSN = document.getElementById("alVSN");
//    var alVSNSt = document.getElementById("alVSNSt");
//    var rBcVSN = document.getElementById("rBcVSN");
//    var rBcSTVSN = document.getElementById("rBcSTVSN");

//    var IdFundLegalSaidaICMS = document.getElementById("IdFundLegalSaidaICMS");


//    uforigem.addEventListener("change", function () {


        
//        var opcaoValorOrigem = uforigem.options[uforigem.selectedIndex].text; //pegou o valor
//        var opcaoValorDestino = ufdestino.options[ufdestino.selectedIndex].text; //pegou o valor


//        if (opcaoValorOrigem === "UF de Origem") {

//            alert("UF de origem e destino devem ser preenchidos");
//            ufdestino.setAttribute('disabled', true);
//            fecp.setAttribute('readonly', true);

//            codReceita.setAttribute('disabled', true);
//            idCstSaiPC.setAttribute('disabled', true);
//            alpS.setAttribute('readonly', true);
//            alcS.setAttribute('readonly', true);
//            idFundamentoLegal.setAttribute('disabled', true);

//            idCstVeVarCF.setAttribute('disabled', true);

//            alVeVarCF.setAttribute('readonly', true);
//            alVeVarCFSt.setAttribute('readonly', true);
//            rBcVeVarCF.setAttribute('readonly', true);
//            rBcSTVeVarCF.setAttribute('readonly', true);
//            idCstVeVarCont.setAttribute('disabled', true);

//            alVeVarCont.setAttribute('readonly', true);
//            alVeVarContSt.setAttribute('readonly', true);
//            rBcVeVarCont.setAttribute('readonly', true);
//            rBcSTVeVarCont.setAttribute('readonly', true);
//            idCstVeAtaCont.setAttribute('disabled', true);
//            alVaC.setAttribute('readonly', true);
//            alVaCSt.setAttribute('readonly', true);

//            idCstVeAtaSN.setAttribute('disabled', true);
//            alVSN.setAttribute('readonly', true);
//            alVSNSt.setAttribute('readonly', true);
//            rBcVSN.setAttribute('readonly', true);
//            rBcSTVSN.setAttribute('readonly', true);
//            IdFundLegalSaidaICMS.setAttribute('disabled', true);

//            rBcVaC.setAttribute('readonly', true);
//            rBcSTVaC.setAttribute('readonly', true);

            


//        } else {
//            if (opcaoValorDestino == "UF de Destino") {
//                alert("UF de origem e destino devem ser preenchidos");
//                ufdestino.removeAttribute('disabled');
//                fecp.setAttribute('readonly', true);

//                codReceita.setAttribute('disabled', true);
//                idCstSaiPC.setAttribute('disabled', true);
//                alpS.setAttribute('readonly', true);
//                alcS.setAttribute('readonly', true);
//                idFundamentoLegal.setAttribute('disabled', true);

//                idCstVeVarCF.setAttribute('disabled', true);

//                alVeVarCF.setAttribute('readonly', true);
//                alVeVarCFSt.setAttribute('readonly', true);
//                rBcVeVarCF.setAttribute('readonly', true);
//                rBcSTVeVarCF.setAttribute('readonly', true);
//                idCstVeVarCont.setAttribute('disabled', true);

//                alVeVarCont.setAttribute('readonly', true);
//                alVeVarContSt.setAttribute('readonly', true);
//                rBcVeVarCont.setAttribute('readonly', true);
//                rBcSTVeVarCont.setAttribute('readonly', true);
//                idCstVeAtaCont.setAttribute('disabled', true);
//                alVaC.setAttribute('readonly', true);
//                alVaCSt.setAttribute('readonly', true);

//                idCstVeAtaSN.setAttribute('disabled', true);
//                alVSN.setAttribute('readonly', true);
//                alVSNSt.setAttribute('readonly', true);
//                rBcVSN.setAttribute('readonly', true);
//                rBcSTVSN.setAttribute('readonly', true);
//                IdFundLegalSaidaICMS.setAttribute('disabled', true);

//                rBcVaC.setAttribute('readonly', true);
//                rBcSTVaC.setAttribute('readonly', true);



//            }
//            else {
//                ufdestino.removeAttribute('disabled');
//                fecp.removeAttribute('readonly');

//                codReceita.removeAttribute('disabled');

//                idCstSaiPC.removeAttribute('disabled');
//                alpS.removeAttribute('readonly');
//                alcS.removeAttribute('readonly');
//                idFundamentoLegal.removeAttribute('disabled');

//                idCstVeVarCF.removeAttribute('disabled');

//                alVeVarCF.removeAttribute('readonly');
//                alVeVarCFSt.removeAttribute('readonly');
//                rBcVeVarCF.removeAttribute('readonly');
//                rBcSTVeVarCF.removeAttribute('readonly');
//                idCstVeVarCont.removeAttribute('disabled');

//                alVeVarCont.removeAttribute('readonly');
//                alVeVarContSt.removeAttribute('readonly');
//                rBcVeVarCont.removeAttribute('readonly');
//                rBcSTVeVarCont.removeAttribute('readonly');
//                idCstVeAtaCont.removeAttribute('disabled');
//                alVaC.removeAttribute('readonly');
//                alVaCSt.removeAttribute('readonly');

//                idCstVeAtaSN.removeAttribute('disabled');
//                alVSN.removeAttribute('readonly');
//                alVSNSt.removeAttribute('readonly');
//                rBcVSN.removeAttribute('readonly');
//                rBcSTVSN.removeAttribute('readonly');
//                IdFundLegalSaidaICMS.removeAttribute('disabled');

//                rBcVaC.removeAttribute('readonly');
//                rBcSTVaC.removeAttribute('readonly');

//            }

//        }

//    });

//    ufdestino.addEventListener("change", function () {

       
//        var opcaoValorOrigem = uforigem.options[uforigem.selectedIndex].text; //pegou o valor
//        var opcaoValorDestino = ufdestino.options[ufdestino.selectedIndex].text; //pegou o valor
       


//        if (opcaoValorOrigem === "UF de Origem"){
          
//            alert("UF de origem e destino devem ser preenchidos");
//            fecp.setAttribute('readonly', true);

//            codReceita.setAttribute('disabled', true);
//            idCstSaiPC.setAttribute('disabled', true);
//            alpS.setAttribute('readonly', true);
//            alcS.setAttribute('readonly', true);
//            idFundamentoLegal.setAttribute('disabled', true);

//            idCstVeVarCF.setAttribute('disabled', true);

//            alVeVarCF.setAttribute('readonly', true);
//            alVeVarCFSt.setAttribute('readonly', true);
//            rBcVeVarCF.setAttribute('readonly', true);
//            rBcSTVeVarCF.setAttribute('readonly', true);
//            idCstVeVarCont.setAttribute('disabled', true);

//            alVeVarCont.setAttribute('readonly', true);
//            alVeVarContSt.setAttribute('readonly', true);
//            rBcVeVarCont.setAttribute('readonly', true);
//            rBcSTVeVarCont.setAttribute('readonly', true);
//            idCstVeAtaCont.setAttribute('disabled', true);
//            alVaC.setAttribute('readonly', true);
//            alVaCSt.setAttribute('readonly', true);

//            idCstVeAtaSN.setAttribute('disabled', true);
//            alVSN.setAttribute('readonly', true);
//            alVSNSt.setAttribute('readonly', true);
//            rBcVSN.setAttribute('readonly', true);
//            rBcSTVSN.setAttribute('readonly', true);
//            IdFundLegalSaidaICMS.setAttribute('disabled', true);

//            rBcVaC.setAttribute('readonly', true);
//            rBcSTVaC.setAttribute('readonly', true);
            
//        } else {
//            if (opcaoValorDestino == "UF de Destino") {
//                alert("UF de origem e destino devem ser preenchidos");

//                fecp.setAttribute('readonly', true);

//                codReceita.setAttribute('disabled', true);
//                idCstSaiPC.setAttribute('disabled', true);
//                alpS.setAttribute('readonly', true);
//                alcS.setAttribute('readonly', true);
//                idFundamentoLegal.setAttribute('disabled', true);

//                idCstVeVarCF.setAttribute('disabled', true);

//                alVeVarCF.setAttribute('readonly', true);
//                alVeVarCFSt.setAttribute('readonly', true);
//                rBcVeVarCF.setAttribute('readonly', true);
//                rBcSTVeVarCF.setAttribute('readonly', true);
//                idCstVeVarCont.setAttribute('disabled', true);

//                alVeVarCont.setAttribute('readonly', true);
//                alVeVarContSt.setAttribute('readonly', true);
//                rBcVeVarCont.setAttribute('readonly', true);
//                rBcSTVeVarCont.setAttribute('readonly', true);
//                idCstVeAtaCont.setAttribute('disabled', true);
//                alVaC.setAttribute('readonly', true);
//                alVaCSt.setAttribute('readonly', true);

//                idCstVeAtaSN.setAttribute('disabled', true);
//                alVSN.setAttribute('readonly', true);
//                alVSNSt.setAttribute('readonly', true);
//                rBcVSN.setAttribute('readonly', true);
//                rBcSTVSN.setAttribute('readonly', true);
//                IdFundLegalSaidaICMS.setAttribute('disabled', true);

//                rBcVaC.setAttribute('readonly', true);
//                rBcSTVaC.setAttribute('readonly', true);
               
//            }
//            else {
//                ufdestino.removeAttribute('disabled');
//                fecp.removeAttribute('readonly');

//                codReceita.removeAttribute('disabled');

//                idCstSaiPC.removeAttribute('disabled');
//                alpS.removeAttribute('readonly');
//                alcS.removeAttribute('readonly');
//                idFundamentoLegal.removeAttribute('disabled');

//                idCstVeVarCF.removeAttribute('disabled');

//                alVeVarCF.removeAttribute('readonly');
//                alVeVarCFSt.removeAttribute('readonly');
//                rBcVeVarCF.removeAttribute('readonly');
//                rBcSTVeVarCF.removeAttribute('readonly');
//                idCstVeVarCont.removeAttribute('disabled');

//                alVeVarCont.removeAttribute('readonly');
//                alVeVarContSt.removeAttribute('readonly');
//                rBcVeVarCont.removeAttribute('readonly');
//                rBcSTVeVarCont.removeAttribute('readonly');
//                idCstVeAtaCont.removeAttribute('disabled');
//                alVaC.removeAttribute('readonly');
//                alVaCSt.removeAttribute('readonly');

//                idCstVeAtaSN.removeAttribute('disabled');
//                alVSN.removeAttribute('readonly');
//                alVSNSt.removeAttribute('readonly');
//                rBcVSN.removeAttribute('readonly');
//                rBcSTVSN.removeAttribute('readonly');
//                IdFundLegalSaidaICMS.removeAttribute('disabled');

//                rBcVaC.removeAttribute('readonly');
//                rBcSTVaC.removeAttribute('readonly');

//            }
            
//        }

//    });



    





//});
function liberarCampos() {
    var uforigem = document.getElementById("ufOrigem");
    var ufdestino = document.getElementById("ufDestino");

    var fecp = document.getElementById("fecp");
    var codReceita = document.getElementById("CodReceita");
    var idCstSaiPC = document.getElementById("idCstSaiPC");
    var alpS = document.getElementById("alpS");
    var alcS = document.getElementById("alcS");
    var idFundamentoLegal = document.getElementById("IdFundamentoLegal");

    var idCstVeVarCF = document.getElementById("idCstVeVarCF");
    var alVeVarCF = document.getElementById("alVeVarCF");
    var alVeVarCFSt = document.getElementById("alVeVarCFSt");
    var rBcVeVarCF = document.getElementById("rBcVeVarCF");
    var rBcSTVeVarCF = document.getElementById("rBcSTVeVarCF");

    var idCstVeVarCont = document.getElementById("idCstVeVarCont");
    var alVeVarCont = document.getElementById("alVeVarCont");
    var alVeVarContSt = document.getElementById("alVeVarContSt");
    var rBcVeVarCont = document.getElementById("rBcVeVarCont");
    var rBcSTVeVarCont = document.getElementById("rBcSTVeVarCont");

    var idCstVeAtaCont = document.getElementById("idCstVeAtaCont");
    var alVaC = document.getElementById("alVaC");
    var alVaCSt = document.getElementById("alVaCSt");
    var rBcVaC = document.getElementById("rBcVaC");
    var rBcSTVaC = document.getElementById("rBcSTVaC");

    var idCstVeAtaSN = document.getElementById("idCstVeAtaSN");
    var alVSN = document.getElementById("alVSN");
    var alVSNSt = document.getElementById("alVSNSt");
    var rBcVSN = document.getElementById("rBcVSN");
    var rBcSTVSN = document.getElementById("rBcSTVSN");

    var IdFundLegalSaidaICMS = document.getElementById("IdFundLegalSaidaICMS");

     


    if (uforigem.onselect && ufdestino.onselect) {
        fecp.removeAttribute('readonly');


    }
}



$(document).ready(function () {
    var a = document.querySelector(".pr-titulo"); //pegar o nome do controler
    var controller = a.innerText;
    var btnEnviar = document.getElementById("enviarIdProd") //Botão responsavel por pegar o id do produto e passar para controller

    //verificar se o botão existe
    if (btnEnviar) {
        btnEnviar.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                alert("Selecione pelo menos uma linha");
                return false;
            }

            var dados = ""; //variavel auxiliar para receber o ID

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                dados1 = selecionado[1].innerHTML;
            }

            dados1 = dados1.trim();

            var id = parseInt(dados); //converte para inteiro

            // alert(controller);
            $.ajax(
                {
                    url: '/' + controller + '/Create',
                    data: { idprod: id },
                    type: "POST",
                    processData: true,
                    success: function () {
                        $("#descproduto").val(dados1).trim();
                    }


                });

        });
    }

});

//Chamar graficos MENU
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoGrafCstSaida = document.getElementById("grafCstSaida"); //variavel para receber o botao de salvar

    //verificar se o botão existe
    if (botaoGrafCstSaida) {
        botaoGrafCstSaida.addEventListener("click", function () {

            bloqueioTela();//bloqueia tela
            //agora o ajax
            $.ajax({

                types: "GET",
                processData: true,
                success: function () {

                    window.location.href = '/Tributacao/GraficoCstSaida';

                }

            });


        });
    }



});//fim chamar graficos menu



//DataTable -->
$(document).ready(function () {
    $('#table-graficos2').DataTable({
        "aoColumnDefs": [
            {
                'bSortable': false,
                'aTargets': [0]
            }],
        language: {
            lengthMenu: "Mostrando _MENU_ registros por página..",
            decimal: ",",
            search: "Pesquisar:",
            emptyTable: "Lista Vazia",
            info: "Mostrando de _START_ até _END_",
            infoEmpty: "Tabela vazia",
            infoFiltered: "(Filtro entre _MAX_ total)",
            infoPostFix: "",
            thousands: ".",
            loadingRecords: "Carregando...",
            processing: "Processando...",
            paginate: {
                first: "Primeiro",
                previous: "Anterior",
                next: "Próximo",
                last: "Último"
            },


        }
    });



});


//Salvar em massa aliq icms st venda atacado para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsSTVendaAtaSN = document.getElementById("salvarAliqIcmsSTVenAtaSN"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsSTVendaAtaSN) {
        botaoSalvarAliqIcmsSTVendaAtaSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTVendAtaSN = document.getElementById("aliqIcmsSTVenAtaSN").value; //pegar o valor do imput
            if (aliqIcmsSTVendAtaSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTVendAtaSN: aliqIcmsSTVendAtaSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsSTVenAtaSNMassaModalPost?strDados=' + strDados + '&aliqIcmsSTVendAtaSN=' + aliqIcmsSTVendAtaSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTVendAtaSN: aliqIcmsSTVendAtaSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsSTVenAtaSNMassaModalPost?strDados=' + strDados + '&aliqIcmsSTVendAtaSN=' + aliqIcmsSTVendAtaSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsSTVenAtaSN").focus();
                }

            }

        });
    }

});


//Salvar em massa aliq icms st venda atacado para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsVendaAtaSN = document.getElementById("salvarAliqIcmsVenAtaSN"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsVendaAtaSN) {
        botaoSalvarAliqIcmsVendaAtaSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsVendAtaSN = document.getElementById("aliqIcmsVenAtaSN").value; //pegar o valor do imput
            if (aliqIcmsVendAtaSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsVendAtaSN: aliqIcmsVendAtaSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsVenAtaSNMassaModalPost?strDados=' + strDados + '&aliqIcmsVendAtaSN=' + aliqIcmsVendAtaSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsVendAtaSN: aliqIcmsVendAtaSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsVenAtaSNMassaModalPost?strDados=' + strDados + '&aliqIcmsVendAtaSN=' + aliqIcmsVendAtaSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsVenAtaSN").focus();
                }

            }

        });
    }

});

//Salvar em massa aliq icms st venda atacado para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsSTVendaAtaCont = document.getElementById("salvarAliqIcmsSTVenAtaCont"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsSTVendaAtaCont) {
        botaoSalvarAliqIcmsSTVendaAtaCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTVendAtaCont = document.getElementById("aliqIcmsSTVenAtaCont").value; //pegar o valor do imput
            if (aliqIcmsSTVendAtaCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTVendAtaCont: aliqIcmsSTVendAtaCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsSTVenAtaContMassaModalPost?strDados=' + strDados + '&aliqIcmsSTVendAtaCont=' + aliqIcmsSTVendAtaCont;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTVendAtaCont: aliqIcmsSTVendAtaCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsSTVenAtaContMassaModalPost?strDados=' + strDados + '&aliqIcmsSTVendAtaCont=' + aliqIcmsSTVendAtaCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsSTVenAtaCont").focus();
                }

            }

        });
    }

});

//Salvar em massa aliq icms venda atacado para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsVendaAtaCont = document.getElementById("salvarAliqIcmsVenAtaCont"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsVendaAtaCont) {
        botaoSalvarAliqIcmsVendaAtaCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsVendAtaCont = document.getElementById("aliqIcmsVenAtaCont").value; //pegar o valor do imput
            if (aliqIcmsVendAtaCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsVendAtaCont: aliqIcmsVendAtaCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsVenAtaContMassaModalPost?strDados=' + strDados + '&aliqIcmsVendAtaCont=' + aliqIcmsVendAtaCont;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsVendAtaCont: aliqIcmsVendAtaCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsVenAtaContMassaModalPost?strDados=' + strDados + '&aliqIcmsVendAtaCont=' + aliqIcmsVendAtaCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsVenAtaCont").focus();
                }

            }

        });
    }

});


//Salvar em massa aliq icms st venda varejo para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsSTVendaVarCont = document.getElementById("salvarAliqIcmsSTVenVarCont"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsSTVendaVarCont) {
        botaoSalvarAliqIcmsSTVendaVarCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTVenVarCont = document.getElementById("aliqIcmsSTVenVarCont").value; //pegar o valor do imput
            if (aliqIcmsSTVenVarCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTVenVarCont: aliqIcmsSTVenVarCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsSTVenVarContMassaModalPost?strDados=' + strDados + '&aliqIcmsSTVenVarCont=' + aliqIcmsSTVenVarCont;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTVenVarCont: aliqIcmsSTVenVarCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsSTVenVarContMassaModalPost?strDados=' + strDados + '&aliqIcmsSTVenVarCont=' + aliqIcmsSTVenVarCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsSTVenVarCont").focus();
                }

            }

        });
    }

});

//Salvar em massa aliq icms venda varejo para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsVendaVarCont = document.getElementById("salvarAliqIcmsVenVarCont"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsVendaVarCont) {
        botaoSalvarAliqIcmsVendaVarCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsVenVarCont = document.getElementById("aliqIcmsVenVarCont").value; //pegar o valor do imput
            if (aliqIcmsVenVarCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsVenVarCont: aliqIcmsVenVarCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsVenVarContMassaModalPost?strDados=' + strDados + '&aliqIcmsVenVarCont=' + aliqIcmsVenVarCont;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsVenVarCont: aliqIcmsVenVarCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsVenVarContMassaModalPost?strDados=' + strDados + '&aliqIcmsVenVarCont=' + aliqIcmsVenVarCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsVenVarCont").focus();
                }

            }

        });
    }

});


//Salvar em massa aliq icms st venda var consumidor final
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsSTVenVarCF = document.getElementById("salvarAliqIcmsSTVenVarCF"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsSTVenVarCF) {
        botaoSalvarAliqIcmsSTVenVarCF.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTVenVarCF = document.getElementById("aliqIcmsSTVenVarCF").value; //pegar o valor do imput
            if (aliqIcmsSTVenVarCF) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTVenVarCF: aliqIcmsSTVenVarCF },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsSTVenVarCFMassaModalPost?strDados=' + strDados + '&aliqIcmsSTVenVarCF=' + aliqIcmsSTVenVarCF;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTVenVarCF: aliqIcmsSTVenVarCF },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsSTVenVarCFMassaModalPost?strDados=' + strDados + '&aliqIcmsSTVenVarCF=' + aliqIcmsSTVenVarCF;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsSTVenVarCF").focus();
                }

            }

        });
    }

});



//Salvar em massa aliq icms venda var consumidor final
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsVenVarCF = document.getElementById("salvarAliqIcmsVenVarCF"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsVenVarCF) {
        botaoSalvarAliqIcmsVenVarCF.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsVenVarCF = document.getElementById("aliqIcmsVenVarCF").value; //pegar o valor do imput
            if (aliqIcmsVenVarCF) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsVenVarCF: aliqIcmsVenVarCF },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsVenVarCFMassaModalPost?strDados=' + strDados + '&aliqIcmsVenVarCF=' + aliqIcmsVenVarCF;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsVenVarCF: aliqIcmsVenVarCF },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsVenVarCFMassaModalPost?strDados=' + strDados + '&aliqIcmsVenVarCF=' + aliqIcmsVenVarCF;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsVenVarCF").focus();
                }

            }

        });
    }

});


//Salvar em massa aliq icms nfe compra de ata
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsNfeCompAta = document.getElementById("salvarAliqIcmsNfeCompAta"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsNfeCompAta) {
        botaoSalvarAliqIcmsNfeCompAta.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsNfeCompraAta = document.getElementById("aliqIcmsNfeCompraAta").value; //pegar o valor do imput
            if (aliqIcmsNfeCompraAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsNfeCompraAta: aliqIcmsNfeCompraAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsNfeAtaMassaModalPost?strDados=' + strDados + '&aliqIcmsNfeCompraAta=' + aliqIcmsNfeCompraAta;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsNfeCompraAta: aliqIcmsNfeCompraAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsNfeAtaMassaModalPost?strDados=' + strDados + '&aliqIcmsNfeCompraAta=' + aliqIcmsNfeCompraAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsNfeCompraAta").focus();
                }

            }

        });
    }

});

//Salvar em massa aliq icms nfe compra de SN
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsNfeCompSN = document.getElementById("salvarAliqIcmsNfeCompSN"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsNfeCompSN) {
        botaoSalvarAliqIcmsNfeCompSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsNfeCompraSN = document.getElementById("aliqIcmsNfeCompraSN").value; //pegar o valor do imput
            if (aliqIcmsNfeCompraSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsNfeCompraSN: aliqIcmsNfeCompraSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsNfeSNMassaModalPost?strDados=' + strDados + '&aliqIcmsNfeCompraSN=' + aliqIcmsNfeCompraSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsNfeCompraSN: aliqIcmsNfeCompraSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsNfeSNMassaModalPost?strDados=' + strDados + '&aliqIcmsNfeCompraSN=' + aliqIcmsNfeCompraSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsNfeCompraSN").focus();
                }

            }

        });
    }

});



//Salvar em massa aliq icms nfe compra de ind
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsNfeCompInd = document.getElementById("salvarAliqIcmsNfeCompInd"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsNfeCompInd) {
        botaoSalvarAliqIcmsNfeCompInd.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsNfeCompraInd = document.getElementById("aliqIcmsNfeCompraInd").value; //pegar o valor do imput
            if (aliqIcmsNfeCompraInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsNfeCompraInd: aliqIcmsNfeCompraInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsNfeIndMassaModalPost?strDados=' + strDados + '&aliqIcmsNfeCompraInd=' + aliqIcmsNfeCompraInd;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsNfeCompraInd: aliqIcmsNfeCompraInd },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsNfeIndMassaModalPost?strDados=' + strDados + '&aliqIcmsNfeCompraInd=' + aliqIcmsNfeCompraInd;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsNfeCompraInd").focus();
                }

            }

        });
    }

});

//Salvar em massa aliq icms ST compra de simples nacional
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsSTCompSN = document.getElementById("salvarAliqIcmsSTCompSN"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsSTCompSN) {
        botaoSalvarAliqIcmsSTCompSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTCompraSN = document.getElementById("aliqIcmsSTCompraSN").value; //pegar o valor do imput
            if (aliqIcmsSTCompraSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTCompraSN: aliqIcmsSTCompraSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsSTCompSNMassaModalPost?strDados=' + strDados + '&aliqIcmsSTCompraSN=' + aliqIcmsSTCompraSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTCompraSN: aliqIcmsSTCompraSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsSTCompSNMassaModalPost?strDados=' + strDados + '&aliqIcmsSTCompraSN=' + aliqIcmsSTCompraSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsSTCompraSN").focus();
                }

            }

        });
    }

});


//Salvar em massa aliq icms compra de simples nacional
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsCompSN = document.getElementById("salvarAliqIcmsCompSN"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsCompSN) {
        botaoSalvarAliqIcmsCompSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsCompraSN = document.getElementById("aliqIcmsCompraSN").value; //pegar o valor do imput
            if (aliqIcmsCompraSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsCompraSN: aliqIcmsCompraSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsCompSNMassaModalPost?strDados=' + strDados + '&aliqIcmsCompraSN=' + aliqIcmsCompraSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsCompraSN: aliqIcmsCompraSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsCompSNMassaModalPost?strDados=' + strDados + '&aliqIcmsCompraSN=' + aliqIcmsCompraSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsCompraSN").focus();
                }

            }

        });
    }

});

//Salvar em mass aliq icms st compra de atacado
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsSTCompAta = document.getElementById("salvarAliqIcmsSTCompAta"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsSTCompAta) {
        botaoSalvarAliqIcmsSTCompAta.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTCompraAta = document.getElementById("aliqIcmsSTCompraAta").value; //pegar o valor do imput
            if (aliqIcmsSTCompraAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTCompraAta: aliqIcmsSTCompraAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsSTCompAtaMassaModalPost?strDados=' + strDados + '&aliqIcmsSTCompraAta=' + aliqIcmsSTCompraAta;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTCompraAta: aliqIcmsSTCompraAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsSTCompAtaMassaModalPost?strDados=' + strDados + '&aliqIcmsSTCompraAta=' + aliqIcmsSTCompraAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsSTCompraAta").focus();
                }

            }

        });
    }

});


//Salvar em massa aliq icms compra de atacado
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsCompAta = document.getElementById("salvarAliqIcmsCompAta"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsCompAta) {
        botaoSalvarAliqIcmsCompAta.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsCompraAta = document.getElementById("aliqIcmsCompraAta").value; //pegar o valor do imput
            if (aliqIcmsCompraAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsCompraAta: aliqIcmsCompraAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsCompAtaMassaModalPost?strDados=' + strDados + '&aliqIcmsCompraAta=' + aliqIcmsCompraAta;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsCompraAta: aliqIcmsCompraAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsCompAtaMassaModalPost?strDados=' + strDados + '&aliqIcmsCompraAta=' + aliqIcmsCompraAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsCompraAta").focus();
                }

            }

        });
    }

});



//Salvar em massa Aliq icms ST compra de industria
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsSTCompInd = document.getElementById("salvarAliqIcmsSTCompInd"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsSTCompInd) {
        botaoSalvarAliqIcmsSTCompInd.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTCompraInd = document.getElementById("aliqIcmsSTCompraInd").value; //pegar o valor do imput
            if (aliqIcmsSTCompraInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTCompraInd: aliqIcmsSTCompraInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsSTCompIndMassaModalPost?strDados=' + strDados + '&aliqIcmsSTCompraInd=' + aliqIcmsSTCompraInd;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTCompraInd: aliqIcmsSTCompraInd },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsSTCompIndMassaModalPost?strDados=' + strDados + '&aliqIcmsSTCompraInd=' + aliqIcmsSTCompraInd;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsSTCompraInd").focus();
                }

            }

        });
    }

});


//Salvar em massa Aliq icms compra de industria
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqIcmsCompInd = document.getElementById("salvarAliqIcmsCompInd"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqIcmsCompInd) {
        botaoSalvarAliqIcmsCompInd.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsCompraInd = document.getElementById("aliqIcmsCompraInd").value; //pegar o valor do imput
            if (aliqIcmsCompraInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsCompraInd: aliqIcmsCompraInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliqIcmsCompIndMassaModalPost?strDados=' + strDados + '&aliqIcmsCompraInd=' + aliqIcmsCompraInd;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsCompraInd: aliqIcmsCompraInd },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliqIcmsCompIndMassaModalPost?strDados=' + strDados + '&aliqIcmsCompraInd=' + aliqIcmsCompraInd;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsCompraInd").focus();
                }

            }

        });
    }

});


//Salvar Red Base Calc Venda Atacado para simples nacional
//Salvar Red Base Calc Venda atacado para simples nacional
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCSTVendaAtaSN = document.getElementById("salvarRedBasCalcSTVendaAtaSN"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCSTVendaAtaSN) {
        botaoSalvarRedBCSTVendaAtaSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcSTVendaAtaSN = document.getElementById("aliqIcmsStVendaAtaSN").value; //pegar o valor do imput
            if (aliqRedBasCalcSTVendaAtaSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcSTVendaAtaSN: aliqRedBasCalcSTVendaAtaSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsSTVendaAtaSNMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcSTVendaAtaSN=' + aliqRedBasCalcSTVendaAtaSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcSTVendaAtaSN: aliqRedBasCalcSTVendaAtaSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsSTVendaAtaSNMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcSTVendaAtaSN=' + aliqRedBasCalcSTVendaAtaSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms ST  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsStVendaAtaSN").focus();
                }

            }

        });
    }

});


//Salvar Red Base Calc Venda atacado para simples nacional
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCVendaAtaSN = document.getElementById("salvarAliqRedBasCalVendaAtaSN"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCVendaAtaSN) {
        botaoSalvarRedBCVendaAtaSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcVendaAtaSN = document.getElementById("aliqRedBasCalcVendaAtaSN").value; //pegar o valor do imput
            if (aliqRedBasCalcVendaAtaSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcVendaAtaSN: aliqRedBasCalcVendaAtaSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsVendaAtaSNMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcVendaAtaSN=' + aliqRedBasCalcVendaAtaSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcVendaAtaSN: aliqRedBasCalcVendaAtaSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsVendaAtaSNMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcVendaAtaSN=' + aliqRedBasCalcVendaAtaSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms  de venda para atacado para Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcVendaAtaSN").focus();
                }

            }

        });
    }

});

//Salvar Red BAse Calc ST Venda Atacado para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCSTVendaAtaCont = document.getElementById("salvarAliqRedBasCalcSTVendaAtaCont"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCSTVendaAtaCont) {
        botaoSalvarRedBCSTVendaAtaCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcSTVendaAtaCont = document.getElementById("aliqRedBasCalcSTVendaAtaCont").value; //pegar o valor do imput
            if (aliqRedBasCalcSTVendaAtaCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcSTVendaAtaCont: aliqRedBasCalcSTVendaAtaCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsSTVendaAtaContMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcSTVendaAtaCont=' + aliqRedBasCalcSTVendaAtaCont;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcSTVendaAtaCont: aliqRedBasCalcSTVendaAtaCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsSTVendaAtaContMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcSTVendaAtaCont=' + aliqRedBasCalcSTVendaAtaCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms ST de venda para atacado para contribuinte para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcSTVendaAtaCont").focus();
                }

            }

        });
    }

});


//Salvar Red BAse Calc Venda Atacado para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCVendaAtaCont = document.getElementById("salvarAliqRedBasCalVendaAtaCont"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCVendaAtaCont) {
        botaoSalvarRedBCVendaAtaCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcVendaAtaCont = document.getElementById("aliqRedBasCalcVendaAtaCont").value; //pegar o valor do imput
            if (aliqRedBasCalcVendaAtaCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcVendaAtaCont: aliqRedBasCalcVendaAtaCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsVendaAtaContMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcVendaAtaCont=' + aliqRedBasCalcVendaAtaCont;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcVendaAtaCont: aliqRedBasCalcVendaAtaCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsVendaAtaContMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcVendaAtaCont=' + aliqRedBasCalcVendaAtaCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms de venda para atacado para contribuinte para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcVendaAtaCont").focus();
                }

            }

        });
    }

});


//Salvar red bas calc st venda varejo contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCSTVendaVarCont = document.getElementById("salvarAliqRedBasCalcSTVendaVarCont"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCSTVendaVarCont) {
        botaoSalvarRedBCSTVendaVarCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcSTVendaVarCont = document.getElementById("aliqRedBasCalcSTVendaVarCont").value; //pegar o valor do imput
            if (aliqRedBasCalcSTVendaVarCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcSTVendaVarCont: aliqRedBasCalcSTVendaVarCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsSTVendaVarContMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcSTVendaVarCont=' + aliqRedBasCalcSTVendaVarCont;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcSTVendaVarCont: aliqRedBasCalcSTVendaVarCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsSTVendaVarContMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcSTVendaVarCont=' + aliqRedBasCalcSTVendaVarCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms ST de venda para varejo para contribuinte para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcSTVendaVarCont").focus();
                }

            }

        });
    }

});

//Salvar red bas calc venda varejo contribuinte
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCVendaVarCont = document.getElementById("salvarAliqRedBasCalVendaVarCont"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCVendaVarCont) {
        botaoSalvarRedBCVendaVarCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcVendaVarCont = document.getElementById("aliqRedBasCalcVendaVarCont").value; //pegar o valor do imput
            if (aliqRedBasCalcVendaVarCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcVendaVarCont: aliqRedBasCalcVendaVarCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsVendaVarContMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcVendaVarCont=' + aliqRedBasCalcVendaVarCont;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcVendaVarCont: aliqRedBasCalcVendaVarCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsVendaVarContMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcVendaVarCont=' + aliqRedBasCalcVendaVarCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms de venda para varejo para contribuinte para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcSTVendaVarCF").focus();
                }

            }

        });
    }

});

//Salvar Aliquota red bas calc St venda varejo consumidor final
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCSTVenVarCF = document.getElementById("salvarAliqRedBasCalSTVendaVarCF"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCSTVenVarCF) {
        botaoSalvarRedBCSTVenVarCF.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcSTVendVarCF = document.getElementById("aliqRedBasCalcSTVendaVarCF").value; //pegar o valor do imput
            if (aliqRedBasCalcSTVendVarCF) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcSTVendVarCF: aliqRedBasCalcSTVendVarCF },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsSTVendaVarCFMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcSTVendVarCF=' + aliqRedBasCalcSTVendVarCF;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcSTVendVarCF: aliqRedBasCalcSTVendVarCF },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsSTVendaVarCFMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcSTVendVarCF=' + aliqRedBasCalcSTVendVarCF;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms ST de venda para consumidor final para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcSTVendaVarCF").focus();
                }

            }

        });
    }

});

//Salvar aliquota red bas calc venda varejo consumidor final
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCVenVarCF = document.getElementById("salvarAliqRedBasCalVendaVarCF"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCVenVarCF) {
        botaoSalvarRedBCVenVarCF.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcVendVarCF = document.getElementById("aliqRedBasCalcVendaVarCF").value; //pegar o valor do imput
            if (aliqRedBasCalcVendVarCF) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcVendVarCF: aliqRedBasCalcVendVarCF },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsVendaVarCFMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcVendVarCF=' + aliqRedBasCalcVendVarCF;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcVendVarCF: aliqRedBasCalcVendVarCF },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsVendaVarCFMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcVendVarCF=' + aliqRedBasCalcVendVarCF;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. icms de venda para consumidor final para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcVendaVarCF").focus();
                }

            }

        });
    }

});

//Salvar Aliquota red bas calc ST compra de simples nacional
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCSTEntSN = document.getElementById("salvarAliqRedBasCalSTEntSN"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCSTEntSN) {
        botaoSalvarRedBCSTEntSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcIcmsSTEntSN = document.getElementById("aliqRedBasCalcSTEntSN").value; //pegar o valor do imput
            if (aliqRedBasCalcIcmsSTEntSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcIcmsSTEntSN: aliqRedBasCalcIcmsSTEntSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsSTCompSNMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsSTEntSN=' + aliqRedBasCalcIcmsSTEntSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcIcmsSTEntSN: aliqRedBasCalcIcmsSTEntSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsSTCompSNMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsSTEntSN=' + aliqRedBasCalcIcmsSTEntSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. ST de icms compra de Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcSTEntSN").focus();
                }

            }

        });
    }

});

//Salvar Aliquota red bas calc Compra de Simples Nacional
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCEntSN = document.getElementById("salvarAliqRedBasCalEntSN"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCEntSN) {
        botaoSalvarRedBCEntSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcIcmsEntSN = document.getElementById("aliqRedBasCalcEntSN").value; //pegar o valor do imput
            if (aliqRedBasCalcIcmsEntSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcIcmsEntSN: aliqRedBasCalcIcmsEntSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsCompSNMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsEntSN=' + aliqRedBasCalcIcmsEntSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcIcmsEntSN: aliqRedBasCalcIcmsEntSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsCompSNMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsEntSN=' + aliqRedBasCalcIcmsEntSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. de icms compra de Simples Nacional para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcEntSN").focus();
                }

            }

        });
    }

});

//Salvar Aliquota red bas calc ST compra de Atacado
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCStEntAta = document.getElementById("salvarAliqRedBasCalSTEntAta"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCStEntAta) {
        botaoSalvarRedBCStEntAta.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcIcmsSTEntAta = document.getElementById("aliqRedBasCalcSTEntAta").value; //pegar o valor do imput
            if (aliqRedBasCalcIcmsSTEntAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcIcmsSTEntAta: aliqRedBasCalcIcmsSTEntAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsSTCompAtaMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsSTEntAta=' + aliqRedBasCalcIcmsSTEntAta;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcIcmsSTEntAta: aliqRedBasCalcIcmsSTEntAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsSTCompAtaMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsSTEntAta=' + aliqRedBasCalcIcmsSTEntAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc.ST de icms de compra de Atacado para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcSTEntAta").focus();
                }

            }

        });
    }

});

//Salvar Aliquota red bas calc compra de atacado
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCEntAta = document.getElementById("salvarAliqRedBasCalEntAta"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCEntAta) {
        botaoSalvarRedBCEntAta.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcIcmsEntAta = document.getElementById("aliqRedBasCalcEntAta").value; //pegar o valor do imput
            if (aliqRedBasCalcIcmsEntAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcIcmsEntAta: aliqRedBasCalcIcmsEntAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsCompAtaMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsEntAta=' + aliqRedBasCalcIcmsEntAta;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcIcmsEntAta: aliqRedBasCalcIcmsEntAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsCompAtaMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsEntAta=' + aliqRedBasCalcIcmsEntAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. de icms de compra de Atacado para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcSTEntInd").focus();
                }

            }

        });
    }

});



//Salvar aliquota Red Base calc ST entrada Ind
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCSTInd = document.getElementById("salvarAliqRedBasCalSTEntInd"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCSTInd) {
        botaoSalvarRedBCSTInd.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcIcmsSTEntInd = document.getElementById("aliqRedBasCalcSTEntInd").value; //pegar o valor do imput
            if (aliqRedBasCalcIcmsSTEntInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcIcmsSTEntInd: aliqRedBasCalcIcmsSTEntInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsSTCompIndMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsSTEntInd=' + aliqRedBasCalcIcmsSTEntInd;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcIcmsSTEntInd: aliqRedBasCalcIcmsSTEntInd },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsSTCompIndMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsSTEntInd=' + aliqRedBasCalcIcmsSTEntInd;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. de icms de compra de Industria para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcSTEntInd").focus();
                }

            }

        });
    }

});


//Salvar aliquota Red Base Calc Entrada Industria
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarRedBCEntInd = document.getElementById("salvarAliqRedBasCalEntInd"); //variavel para receber o botao de salvar
    if (botaoSalvarRedBCEntInd) {
        botaoSalvarRedBCEntInd.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqRedBasCalcIcmsEntInd = document.getElementById("aliqRedBasCalcEntInd").value; //pegar o valor do imput
            if (aliqRedBasCalcIcmsEntInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqRedBasCalcIcmsEntInd: aliqRedBasCalcIcmsEntInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditRedBCIcmsCompIndMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsEntInd=' + aliqRedBasCalcIcmsEntInd;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqRedBasCalcIcmsEntInd: aliqRedBasCalcIcmsEntInd },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditRedBCIcmsCompIndMassaModalPost?strDados=' + strDados + '&aliqRedBasCalcIcmsEntInd=' + aliqRedBasCalcIcmsEntInd;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Redução da base de calc. de icms de entrada de Industria para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqRedBasCalcEntInd").focus();
                }

            }

        });
    }

});

//Salvar Aliquota Confins saida
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqEntPis = document.getElementById("salvarAliqCofinsSai"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqEntPis) {
        botaoSalvarAliqEntPis.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqSaiCofins = document.getElementById("aliqSaidaCofins").value; //pegar o valor do imput
            if (aliqSaiCofins) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqSaiCofins: aliqSaiCofins },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliSaiCofinsMassaModalPost?strDados=' + strDados + '&aliqSaiCofins=' + aliqSaiCofins;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqSaiCofins: aliqSaiCofins },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliSaiCofinsMassaModalPost?strDados=' + strDados + '&aliqSaiCofins=' + aliqSaiCofins;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Entrada Pis para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqSaidaCofins").focus();
                }

            }

        });
    }

});

//Salvar aliquota Cofins entrada
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqEntPis = document.getElementById("salvarAliqCofinsEnt"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqEntPis) {
        botaoSalvarAliqEntPis.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqEntCofins = document.getElementById("aliqEntradaCofins").value; //pegar o valor do imput
            if (aliqEntCofins) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqEntCofins: aliqEntCofins },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliEntCofinsMassaModalPost?strDados=' + strDados + '&aliqEntCofins=' + aliqEntCofins;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqEntCofins: aliqEntCofins },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliEntCofinsMassaModalPost?strDados=' + strDados + '&aliqEntCofins=' + aliqEntCofins;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Entrada Pis para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqEntradaCofins").focus();
                }

            }

        });
    }

});

//Salvar aliquota pis saida
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqEntPis = document.getElementById("salvarAliqPisSai"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqEntPis) {
        botaoSalvarAliqEntPis.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqSaidaPis = document.getElementById("aliqSaidaPis").value; //pegar o valor do imput
            if (aliqSaidaPis) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqSaidaPis: aliqSaidaPis },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliSaiPisMassaModalPost?strDados=' + strDados + '&aliqSaidaPis=' + aliqSaidaPis;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqSaidaPis: aliqSaidaPis },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliSaiPisMassaModalPost?strDados=' + strDados + '&aliqSaidaPis=' + aliqSaidaPis;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Entrada Pis para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqSaidaPis").focus();
                }

            }

        });
    }
});


//Salvar Aliquota Pis Entrada
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarAliqEntPis = document.getElementById("salvarAliqPisEnt"); //variavel para receber o botao de salvar
    if (botaoSalvarAliqEntPis) {
        botaoSalvarAliqEntPis.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqEntPis = document.getElementById("aliqEntradaPis").value; //pegar o valor do imput
            if (aliqEntPis) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqEntPis: aliqEntPis },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditAliEntPisMassaModalPost?strDados=' + strDados + '&aliqEntPis=' + aliqEntPis;

                    }

                });
                //fim if verificacao vazio
            } else {
                //toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                //return false;
                var resultado = confirm("A alíquota informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqEntPis: aliqEntPis },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/EditAliEntPisMassaModalPost?strDados=' + strDados + '&aliqEntPis=' + aliqEntPis;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Alíquota de Entrada Pis para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqEntradaPis").focus();
                }

            }

        });
    }
});


//Salvar CST Comra com NFE SN
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstNfeSN = document.getElementById("salvarCstNfeSN"); //variavel para receber o botao de salvar

    if (botaoSalvarCstNfeSN) {
        botaoSalvarCstNfeSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstCompraNfeSN = document.getElementById("cstNfeSN").value; //pegar o valor do imput

            if (cstCompraNfeSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstCompraNfeSN: cstCompraNfeSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstNfeSNMassaModalPost?strDados=' + strDados + '&cstCompraNfeSN=' + cstCompraNfeSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                return false;

            }
        });
    }
});


//Salvar CSt Compra com Nfe Ata
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstNfeAta = document.getElementById("salvarCstNfeAta"); //variavel para receber o botao de salvar

    if (botaoSalvarCstNfeAta) {
        botaoSalvarCstNfeAta.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstCompraNfeAta = document.getElementById("cstNfeAta").value; //pegar o valor do imput

            if (cstCompraNfeAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstCompraNfeAta: cstCompraNfeAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstNfeAtaMassaModalPost?strDados=' + strDados + '&cstCompraNfeAta=' + cstCompraNfeAta;

                    }

                });
                //fim if verificacao vazio
            } else {
                toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                return false;

            }
        });
    }
});


//Salvar Cst Compra com Nfe Ind
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstNfeInd = document.getElementById("salvarCstNfeInd"); //variavel para receber o botao de salvar

    if (botaoSalvarCstNfeInd) {
        botaoSalvarCstNfeInd.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstCompraNfeInd = document.getElementById("cstNfeInd").value; //pegar o valor do imput

            if (cstCompraNfeInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstCompraNfeInd: cstCompraNfeInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstNfeIndMassaModalPost?strDados=' + strDados + '&cstCompraNfeInd=' + cstCompraNfeInd;

                    }

                });
                //fim if verificacao vazio
            } else {
                toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                return false;

            }
        });
    }
});

//Salvar CSt Compra de Simples Nacional
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstCompSN = document.getElementById("salvarCstCompraSN"); //variavel para receber o botao de salvar

    if (botaoSalvarCstCompSN) {
        botaoSalvarCstCompSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstCompraSN = document.getElementById("cstCompSN").value; //pegar o valor do imput

            if (cstCompraSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstCompraSN: cstCompraSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstCompraSNMassaModalPost?strDados=' + strDados + '&cstCompraSN=' + cstCompraSN;

                    }

                });
                //fim if verificacao vazio
            } else {
                toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                return false;

            }
        });
    }

});

//Salvar CST Compra de Atacado
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstCompAta = document.getElementById("salvarCstCompraAta"); //variavel para receber o botao de salvar

    if (botaoSalvarCstCompAta) {
        botaoSalvarCstCompAta.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstCompraAta = document.getElementById("cstCompAta").value; //pegar o valor do imput
            if (cstCompraAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstCompraAta: cstCompraAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstCompraAtacadoMassaModalPost?strDados=' + strDados + '&cstCompraAta=' + cstCompraAta;

                    }

                });
                //fim if verificacao vazio
            } else {
                toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                return false;

            }
        })
    }
});

//Salvar CST Compra de Industria
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstCompInd = document.getElementById("salvarCstCompraInd"); //variavel para receber o botao de salvar

    if (botaoSalvarCstCompInd) {

        botaoSalvarCstCompInd.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstCompraInd = document.getElementById("cstCompInd").value; //pegar o valor do imput
            if (cstCompraInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstCompraInd: cstCompraInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstCompraIndustriaMassaModalPost?strDados=' + strDados + '&cstCompraInd=' + cstCompraInd;

                    }

                });
                //fim if verificacao vazio
            } else {
                toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                return false;

            }
        });
    }
});


//Salvar CST Pis/Confins Entrada
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstVenAtaSN = document.getElementById("salvarCstPCEntrada"); //variavel para receber o botao de salvar

    //verificar se o botão existe
    if (botaoSalvarCstVenAtaSN) {

        //funcao para enviar para action os dados para serem salvos
        botaoSalvarCstVenAtaSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstPisCofinsE = document.getElementById("cstPCEntrada").value; //pegar o valor do imput
            //verificar se o valor do imput está diferente de vazio
            if (cstPisCofinsE != "") {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstPisCofinsE: cstPisCofinsE },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstPisCofinsEntradaMassaModalPost?strDados=' + strDados + '&cstPisCofinsE=' + cstPisCofinsE;

                    }

                });
                //fim if verificacao vazio
            } else {
                toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                return false;

            }



        });//fim botao salvar

    }//fim if botão salvar

});

//Salvar CST Venda Atacado Simples Nacional - Selecionados
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstVenAtaSN = document.getElementById("salvarCstVenAtaSN"); //variavel para receber o botao de salvar

    //verificar se o botão existe
    if (botaoSalvarCstVenAtaSN) {

        //funcao para enviar para action os dados para serem salvos
        botaoSalvarCstVenAtaSN.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstVendAtaSN = document.getElementById("cstVenAtaSN").value; //pegar o valor do imput
            var origem = document.getElementById("ori").value; //pegar o valor do imput
            //verificar se o valor do imput está diferente de vazio
            if (cstVendAtaSN != "") {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstVendAtaSN: cstVendAtaSN, origem: origem },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstVendaAtaSNMassaModalPost?strDados=' + strDados + '&cstVendAtaSN=' + cstVendAtaSN + '&origem=' + origem;

                    }

                });
                //fim if verificacao vazio
            } else {
                toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                return false;

            }



        });//fim botao salvar

    }//fim if botão salvar
});

//Salvar CST Venda Atacado Contribuinte - Selecionados
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstVenAtaCont = document.getElementById("salvarCstVenAtaCont"); //variavel para receber o botao de salvar
    //verificar se o botão existe
    if (botaoSalvarCstVenAtaCont) {
        //funcao para enviar para action os dados para serem salvos
        botaoSalvarCstVenAtaCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstVendAtaCont = document.getElementById("cstVeAtaCont").value; //pegar o valor do imput
            //verificar se o valor do imput está diferente de vazio
            if (cstVendAtaCont != "") {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstVendAtaCont: cstVendAtaCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstVendaAtaContMassaModalPost?strDados=' + strDados + '&cstVendAtaCont=' + cstVendAtaCont;

                    }

                });
                //fim if verificacao vazio
            } else {
                toastr.error("Selecione um CST!"); //caso nao escolha um cst uma exceção é lançada
                return false;

            }

        });//fim botao salvar
    }

});

//Slvar CST Venda Varejo Contribuinte - Selecionados
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstVenVarCont = document.getElementById("salvarCstVenVarCont"); //variavel para receber o botao de salvar
    //verificar se o botão existe
    if (botaoSalvarCstVenVarCont) {
        //funcao para enviar para action os dados para serem salvos
        botaoSalvarCstVenVarCont.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstVendVarCont = document.getElementById("cstVeVarCont").value; //pegar o valor do imput

            if (cstVendVarCont != "") {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstVendVarCont: cstVendVarCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstVendaVarContMassaModalPost?strDados=' + strDados + '&cstVendVarCont=' + cstVendVarCont;

                    }

                });
            } else {
                toastr.error("Selecione um CST!");
                return false;
            }
        });//fim botao salvar

    }//fim if botao salvar
});

//Salvar CST Venda Varejo Consumidor Final
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstVenVarCF = document.getElementById("salvarCstVenVarCF"); //variavel para receber o botao de salvar

    //verificar se o botão existe
    if (botaoSalvarCstVenVarCF) {
        //funcao para enviar para action os dados para serem salvos
        botaoSalvarCstVenVarCF.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            /* var cstVenVarCFMudar = document.getElementById("cstVeVarCF"); //pegar o valor do input*/
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstVendVarCF = document.getElementById("cstVeVarCF").value; //pegar o valor

            if (cstVendVarCF != "") {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela
                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstVendVarCF: cstVendVarCF },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstVendaVarCFMassaModalPost?strDados=' + strDados + '&cstVendVarCF=' + cstVendVarCF;

                    }

                });
            } else {
                toastr.error("Selecione um CST!");
                return false;
            }
        });//fim botao salvar
    }

});


//Salvar CST Pis Cofins Saída
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCstPCSaida = document.getElementById("salvarCstPCSaida"); //variavel para receber o botao de salvar

    //verificar se o botão existe
    if (botaoSalvarCstPCSaida) {
        //funcao para enviar para action os dados para serem salvos
        botaoSalvarCstPCSaida.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var cstPCSaidaMudar = document.getElementById("cstPCSaida"); //pegar o valor do input
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cstPCSaida = cstPCSaidaMudar.value; // //variavel que recebe o valor do input

            if (cstPCSaida != "") {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, cstPCSaida: cstPCSaida },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstPisCofinsSaidaMassaModalPost?strDados=' + strDados + '&cstPCSaida=' + cstPCSaida;

                    }

                });



            } else {
                toastr.error("Selecione um CST!");
                return false;
            }

        });
    }


});

//Salvar Código de Barras
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botaoSalvarCodBarras = document.getElementById("salvarCodBarras"); //variavel para receber o botao de salvar

    //verificar se o botao existe
    if (botaoSalvarCodBarras) {
        //funcao para enviar para action os dados para serem salvos
        botaoSalvarCodBarras.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var codBarrasMudar = document.getElementById("codBarras"); //pegar o valor do input

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var codBarras = codBarrasMudar.value; // //variavel que recebe o valor do input

            if (codBarras != "") {
                if (codBarras.length > 13) {
                    document.getElementById("codBarras").focus(); //seta o foco no imput
                    toastr.error("Código de barras maior que o permitido"); //dispara mensagem de alerta

                } else {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }//fim do for
                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, codBarras: codBarras },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Produto/EditCodBarrasMassaModalPost?strDados=' + strDados + '&codBarras=' + codBarras;

                        }

                    });
                }
            } else { //se o imput for vazio verificar se a ação vai continuar
                var resultado = confirm("O Código informado foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, codBarras: codBarras },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Produto/EditCodBarrasMassaModalPost?strDados=' + strDados + '&codBarras=' + codBarras;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de CÓDIGO DE BARRAS para os produto selecionado abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("codBarras").focus();
                }
            }

        });

    }


});

//Salvar CEST alterados
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var botalSalvarCEST = document.getElementById("salvarCEST"); //variavel para receber o botao de salvar cest


    //verificar se o botão existe
    if (botalSalvarCEST) {

        //funcao para enviar para action os dados para serem salvos
        botalSalvarCEST.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var cestMudar = document.getElementById("cest"); //pegar o valor do input

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cest = cestMudar.value; // //variavel que recebe o valor do input

            if (cest != "") {
                if (cest.length != 9) {
                    /* alert("Tamanho do NCM incorreto! Digite novamente");*/
                    document.getElementById("cest").focus();
                    /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
                    toastr.error("Tamanho do CEST incorreto! Digite novamente");

                } else {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, cest: cest },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Produto/EditCestMassaModalPost?strDados=' + strDados + '&cest=' + cest;

                        }

                    });

                }//fim do else verificar se menor que 9

            } else {
                var resultado = confirm("O CEST informado foi NULO, deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, cest: cest },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Produto/EditCestMassaModalPost?strDados=' + strDados + '&cest=' + cest;

                        }

                    });


                } else {
                    toastr.warning("Atribuição de CEST para os produtos selecionados abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("cest").focus();
                }



            }//fim do else verficar se nulo
        });
    }


});


//salvar ncm tributacao

$(document).ready(function () {
    toastOpcoes();
    var botaoSalvarNCMTribNCM = document.getElementById("salvarNCMTribNCM"); //variavel que recebe o botao da alteracao do ncm
    //com NCM
    var botaoSalvarNCMtribComNCM = document.getElementById("salvarNCMTribComNCM");



    //verificar se o botao existe
    if (botaoSalvarNCMTribNCM) {
        botaoSalvarNCMTribNCM.addEventListener("click", function () {
            var dados = {}
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var ncmMudar = document.getElementById("ncm");
            var cestMudar = document.getElementById("cest"); //pegar o valor do input

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";
            var ncm = ncmMudar.value;
            var cest = cestMudar.value; // //variavel que recebe o valor do input

            if (cest != "")
            {
                if (cest == "NULL") {
                    var resultado = confirm("Todos os NCM selecionados TERÃO O CEST APAGADOS, confirma?");
                    if (resultado == false) {
                        toastr.warning("Atribuição de CEST com valor Nulo para os produtos selecionados abortada");
                        /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                        document.getElementById("ncm").focus();
                        return;
                    }

                } else
                {
                    if (cest.length != 9) {
                        /* alert("Tamanho do NCM incorreto! Digite novamente");*/
                        document.getElementById("cest").focus();
                        /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
                        toastr.error("Tamanho do CEST incorreto! Digite novamente");

                    }
                }

                

            }



            if (ncm != "") {
                if (ncm.length != 10) {
                    /* alert("Tamanho do NCM incorreto! Digite novamente");*/

                    document.getElementById("ncm").focus();
                    /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
                    toastr.error("Tamanho do NCM incorreto! Digite novamente");

                } else {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, ncm: ncm, cest: cest },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/TributacaoNcmEditMassaModalEditMassaModalPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;

                        }


                    });


                }


            } else {
                var resultado = confirm("O NCM informado foi NULO, deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, ncm: ncm, cest: cest },
                        types: "GET",
                        processData: true,
                        success: function () {
                            window.location.href = '/Tributacao/TributacaoNcmEditMassaModalEditMassaModalPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;
                            //window.location.href = '/Produto/EditMassaModalPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;

                        }

                    });


                } else {
                    toastr.warning("Atribuição de NCM para os produtos selecionados abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("ncm").focus();
                }

            }



        });
    }

    if (botaoSalvarNCMtribComNCM)
    {
        botaoSalvarNCMtribComNCM.addEventListener("click", function ()
        {
            var dados = {}
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var ncmMudar = document.getElementById("ncm");
            var cestMudar = document.getElementById("cest"); //pegar o valor do input

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";
            var ncm = ncmMudar.value;
            var cest = cestMudar.value; // //variavel que recebe o valor do input

            if (cest != "") {
                if (cest == "NULL") {
                    var resultado = confirm("Todos os NCM selecionados TERÃO O CEST APAGADOS, confirma?");
                    if (resultado == false) {
                        toastr.warning("Atribuição de CEST com valor Nulo para os produtos selecionados abortada");
                        /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                        document.getElementById("ncm").focus();
                        return;
                    }

                } else {
                    if (cest.length != 9) {
                        /* alert("Tamanho do NCM incorreto! Digite novamente");*/
                        document.getElementById("cest").focus();
                        /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
                        toastr.error("Tamanho do CEST incorreto! Digite novamente");

                    }
                }



            }



            if (ncm != "") {
                if (ncm.length != 10) {
                    /* alert("Tamanho do NCM incorreto! Digite novamente");*/

                    document.getElementById("ncm").focus();
                    /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
                    toastr.error("Tamanho do NCM incorreto! Digite novamente");

                } else {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, ncm: ncm, cest: cest },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Tributacao/TributacaoNcmEditMassaModalEditMassaModalComNCMPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;

                        }


                    });


                }


            } else {
                var resultado = confirm("O NCM informado foi NULO, deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, ncm: ncm, cest: cest },
                        types: "GET",
                        processData: true,
                        success: function () {
                            window.location.href = '/Tributacao/TributacaoNcmEditMassaModalEditMassaModalComNCMPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;
                            //window.location.href = '/Produto/EditMassaModalPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;

                        }

                    });


                } else {
                    toastr.warning("Atribuição de NCM para os produtos selecionados abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("ncm").focus();
                }

            }






        });
    }


});




//salvar ncm alterados
$(document).ready(function () {
    toastOpcoes();
    var botaoSalvarNCM = document.getElementById("salvarNCM"); //variavel que recebe o botao da alteracao do ncm

    //verificar se o botao existe
    if (botaoSalvarNCM) {
        botaoSalvarNCM.addEventListener("click", function () {
            var dados = {}
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var ncmMudar = document.getElementById("ncm");
            var cestMudar = document.getElementById("cest"); //pegar o valor do input

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";
            var ncm = ncmMudar.value;
            var cest = cestMudar.value; // //variavel que recebe o valor do input

            if (cest != "")
            {
                if (cest.length != 9) {
                    /* alert("Tamanho do NCM incorreto! Digite novamente");*/
                    document.getElementById("cest").focus();
                    /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
                    toastr.error("Tamanho do CEST incorreto! Digite novamente");

                }

            }



            if (ncm != "") {
                if (ncm.length != 10) {
                    /* alert("Tamanho do NCM incorreto! Digite novamente");*/

                    document.getElementById("ncm").focus();
                    /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
                    toastr.error("Tamanho do NCM incorreto! Digite novamente");

                } else {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, ncm: ncm, cest: cest },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Produto/EditMassaModalPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;

                        }


                    });


                }


            } else {
                var resultado = confirm("O NCM informado foi NULO, deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, ncm: ncm, cest: cest },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Produto/EditMassaModalPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;

                        }

                    });


                } else {
                    toastr.warning("Atribuição de NCM para os produtos selecionados abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("ncm").focus();
                }

            }



        });
    }



});


$(document).ready(function () {
    /** Script para selecionar a linha tabela */
    var tabela = document.getElementById("tablepr");
    var tabela = document.getElementById("tablepr-user");
    var tabela = document.getElementById("tablepr-2");
    var linhas = document.getElementsByTagName("tr");
    var btnEditar = document.getElementById("editarDados"); //variavel que representa o botão
    var btnVisualizar = document.getElementById("visualizarDados"); //variavel que representa o botão
    var btnApagar = document.getElementById("apagarDados"); //variavel que representa o botao de apagar

    var a = document.querySelector(".pr-titulo"); //pegar o nome do controler
    var controller = a.innerText;


    for (var i = 0; i < linhas.length; i++) {
        var linha = linhas[i];
        linha.addEventListener("click", function () {
            //Adicionar ao atual
            selLinha(this, false); //Selecione apenas um (parametro true seleciona mais de uma linha)

        });
    }

    function selLinha(linha) {
        if (linha) {
            var linhas = linha.parentElement.getElementsByTagName("tr");
            for (var i = 0; i < linhas.length; i++) {
                var linha_ = linhas[i];
                linha_.classList.remove("selecionado");
            }
        }
        linha.classList.toggle("selecionado");
    }

    if (btnVisualizar) {
        /* Função para pegar o clique do botão VISUALIZAR*/
        btnVisualizar.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                alert("Selecione pelo menos uma linha");
                return false;
            }

            var dados = ""; //variavel auxiliar para receber o ID

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
            }
            var id = parseInt(dados); //converte para inteiro
            bloqueioTela();//bloqueia tela
            $.ajax(
                {
                    url: controller + '/Detalhes',
                    data: { id: id },
                    types: "GET",
                    processData: true,
                    success: function () {
                        window.location.href = controller + '/detalhes?id=' + id
                    }

                });

        });

    }

    if (btnEditar) {
        /*Função para pegar o clique do botão editar*/
        btnEditar.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                alert("Selecione pelo menos uma linha");
                return false;
            }

            var dados = ""; //variavel auxiliar para receber o ID

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
            }

            var id = parseInt(dados); //converte para inteiro
            bloqueioTela();//bloqueia tela

            $.ajax(
                {

                    data: { id: id },
                    types: "GET",
                    processData: true,
                    success: function () {
                        window.location.href = controller + '/edit?id=' + id

                        $("#modalEdit").load(controller + '/edit?id=' + id, function () {
                            $("#modalEdit").modal();
                        })


                    }

                });

        });

    }


    if (btnApagar) {
        /*Função para pegar o clique do botão apagar*/
        btnApagar.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                alert("Selecione pelo menos uma linha");
                return false;
            }

            var dados = ""; //variavel auxiliar para receber o ID

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
            }

            var id = parseInt(dados); //converte para inteiro



            $.ajax(
                {
                    /* url: controller + '/Delete',*/
                    data: { id: id },
                    types: "GET",
                    processData: true,
                    success: function () {
                        window.location.href = controller + '/delete?id=' + id
                    }

                });

        });
    }



})


//DataTable -->
$(document).ready(function () {
    $('#tablepr').DataTable({
        "aoColumnDefs": [
            {
                'bSortable': false,
                'aTargets': [0]
            }],
        language: {
            lengthMenu: "Mostrando _MENU_ registros por página..",
            decimal: ",",
            search: "Pesquisar:",
            emptyTable: "Lista Vazia",
            info: "Mostrando de _START_ até _END_",
            infoEmpty: "Tabela vazia",
            infoFiltered: "(Filtro entre _MAX_ total)",
            infoPostFix: "",
            thousands: ".",
            loadingRecords: "Carregando...",
            processing: "Processando...",
            paginate: {
                first: "Primeiro",
                previous: "Anterior",
                next: "Próximo",
                last: "Último"
            },


        }
    });


});




/*Converter inputs em maiusculo*/


function maiuscula(z) {
    v = z.value.toUpperCase();
    z.value = v;
}
function minuscula(z) {
    v = z.value.toLowerCase();
    z.value = v;
}

function onlynumber(evt) {
    var theEvent = evt || window.event;
    var key = theEvent.keyCode || theEvent.which;
    key = String.fromCharCode(key);
    //var regex = /^[0-9.,]+$/;
    var regex = /^[0-9.]+$/;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}
function onlynumberDecimal(evt) {
    var theEvent = evt || window.event;
    var key = theEvent.keyCode || theEvent.which;
    key = String.fromCharCode(key);
    var regex = /^[0-9,]+$/;

    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

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
//FIM DA FUNÇÃO MASCARA MAIUSCULA


//FUNÇÃO VALIDAR CNPJ
function validarCNPJ(el) {
    if (!_cnpj(el.value)) {
        alert("CNPJ informado está inválido!: " + el.value);
        // apaga o valor
        el.value = "";
    }
}
function _cnpj(cnpj) {
    cnpj = cnpj.replace(/[^\d]+/g, '');
    if (cnpj == '') return false;
    if (cnpj.length != 14)
        return false;
    if (cnpj == "00000000000000" ||
        cnpj == "11111111111111" ||
        cnpj == "22222222222222" ||
        cnpj == "33333333333333" ||
        cnpj == "44444444444444" ||
        cnpj == "55555555555555" ||
        cnpj == "66666666666666" ||
        cnpj == "77777777777777" ||
        cnpj == "88888888888888" ||
        cnpj == "99999999999999")
        return false;
    tamanho = cnpj.length - 2
    numeros = cnpj.substring(0, tamanho);
    digitos = cnpj.substring(tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(0)) return false;
    tamanho = tamanho + 1;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(1))
        return false;
    return true;
}
//FIM FUNÇÃO VALIDAR CNPJ

function enviarProd() {
    var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
    //Verificar se está selecionado
    if (selecionados.length < 1) {
        alert("Selecione pelo menos uma linha");
        return false;
    }
    var dados = ""; //variavel auxiliar para receber o ID
    /*Laço para varrer os elementos com a tag TD*/
    for (var i = 0; i < selecionados.length; i++) {
        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
        dados = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
        dados1 = selecionado[1].innerHTML;
    }
    dados1 = dados1.trim();
    dados = dados.trim();
    var id = parseInt(dados); //converte para inteiro
    $("#idProd").val(dados);
    $("#descproduto").val(dados1);

}


//LIMPAR INPUT
function clearInput() {
    $(".formInput").val(null);
}

function toastOpcoes() {
    toastr.options = {
        "closeButton": true,
        "debug": true,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-center",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "100",
        "hideDuration": "1000",
        "timeOut": "4000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
}



//pegar quantidade linhas da tabela
$(document).ready(function () {

    var tabela = document.getElementById("table-editmassa");

    if (tabela) {
        var linhas = tabela.getElementsByTagName('tr');
        var lab = document.getElementById('lab');
        lab.innerHTML = (linhas.length - 1);

    }


});

$(document).ready(function () {
    setTimeout(function () {
        $(".alert").fadeOut("slow", function () {

            $(this).alert('close');
        });

    }, 5000);
});
/*Liberar a tela apos a execução do codigo dentro da chamada ajax*/
$(document).ajaxStop($.unblockUI);