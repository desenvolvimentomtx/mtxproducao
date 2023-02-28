



//DataTable -->
$(document).ready(function () {
    $('#table-graficos').DataTable({
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

//Alterar selecionados icms ST venda atacado para simples nacional
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsSTVenAtaSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsSTVenAtaSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsSTVenAtaSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//Alterar selecionados icms ST venda atacado para simples nacional
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsVenAtaSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsVenAtaSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsVenAtaSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});




//Alterar selecionados icms ST venda atacado para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsSTVenAtaContMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsSTVenAtaCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsSTVenAtaCont"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados icms venda atacado para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsVenAtaContMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsVenAtaCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsVenAtaCont"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});



//Alterar selecionados icms st venda varejo para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsSTVenVarContMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsSTVenVarCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsSTVenVarCont"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados icms venda varejo para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsVenVarContMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsVenVarCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsVenVarCont"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//Alterar selecionados icms st venda varejo para consumidor final
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsSTVenVarCFMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsSTVenVafCF"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsSTVenVarCF"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados icms venda varejo para consumidor final
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsVenVarCFMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsVenVafCF"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsVenVarCF"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Altear Selecionados icms nfe compra ata
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsNfeAtaMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsNfeCompAta"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsNfeCompAta"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados icms nfe compra SN
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsNfeSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsNfeCompSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsNfeCompSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados icms nfe compra ind
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsNfeIndMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsNfeCompInd"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsNfeCompInd"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar Selecionados icms st compra de simples nacional
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsSTCompSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsSTCompSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsSTCompSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//Alterar selecionados icms compra de simples nacional
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsCompSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsCompSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsCompSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados icms st compra de atacado EditAliqIcmsSTCompAtaMassa
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsSTCompAtaMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsSTCompAta"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsSTCompAta"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados icms compra de atacado
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsCompAtaMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsCompAta"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsCompAta"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Altear selecionado imcs st compra de industria
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsSTCompIndMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsSTCompInd"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsSTCompInd"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar Selcionado Icms compra de industria
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliqIcmsCompIndMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqIcmsCompInd"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAliqIcmsCompInd"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//Alterar Selecionado red bas calc st icms  venda ata para simples nacional
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsSTVendaAtaSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedAliqIcmsVenAtaSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("edtSelRedAliqIcmsSTVendaAtaSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});



//Alterar Selecionado red bas calc icms  venda ata para simples nacional
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsVendaAtaSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcVendaAtaSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcVendaAtaSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//alterar selecionado red bas calc icms st venda ata contribuinte
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsSTVendaAtaContMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcSTVendaAtaCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcSTVendaAtaCont"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//Alterar Selecionado Red Base Calc Icms Venda ATa Contribuinte
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsVendaAtaContMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcVendaAtaCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcVendaAtaCont"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//Alterar Selecionado red base calc st venda varejo para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsSTVendaVarContMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcSTVendaVarCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcSTVendaVarCont"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar Selecionado red base calc venda varejo para contribuinte
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsVendaVarContMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcVendaVarCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcVendaVarCont"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar Selecionados Red Base de Calc ST Venda varejo consumidor final
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsSTVendaVarCFMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcSTVendaVarCF"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcSTVendaVarCF"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados Red Base de calc venda varejo consumidor final
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsVendaVarCFMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcVendaVarCF"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcVendaVarCF"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//alterar Selecionados Red Base de calc ST compra de simples nacional
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsSTCompSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcSTCompSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcSTCompSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar Selecionados Red Base de Calc Compra de Simples nacional
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsCompSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcCompSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcCompSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar Selecionados Red Base de Calc ST compra de atacado
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsSTCompAtaMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcSTCompAta"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcSTCompAta"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});



//Alterar selecionados Red Base de Calc compra de Atacado
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsCompAtaMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcCompAta"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcCompAta"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});



//Alterar selecionados Red Base de Calc ST Compra de indrustria
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsSTCompIndMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcSTCompInd"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcSTCompInd"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados Red Base de Calc Compra de industria
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditRedBCIcmsCompIndMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosRedBasCalcCompInd"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosRedBCalcCompInd"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});



//Alterar Selecionados Aliq Confins Saida
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliSaiCofinsMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqCofinsSai"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAlqCofinsSai"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar selecionados Aliq Confins Entrada
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliEntCofinsMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqCofinsEnt"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAlqCofinsEnt"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//Alterar Selecionados Aliq Pis Saida
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliSaiPisMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqPisSai"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAlqPisSai"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//Alterar SElecionados Aliq PIs Entrada
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditAliEntPisMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosAliqPisEnt"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosAlqPisEnt"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);

});

//Alterar Selecionados CST Nfe SN
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditCstNfeSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosNfeSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosNfeSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);

});

//Alterar Selecionados CST Nfe Ata
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditCstNfeAtaMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosNfeAta"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosNfeAta"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);

});

//Altear Selecionados CST NFe Ind
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditCstNfeIndMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosNfeInd"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosNfeInd"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});

//Alterar Selecionados CST Compra de Simples Nacional
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditCstCompraSNMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosCstCompraSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCstCompraSN"); //botao para confirmar a edição dos selecionados

    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela); 

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);


});

//Alterar Selecionados CST Compra de Atacado
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditCstCompraAtacadoMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosCstCompraAta"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCstCompraAta"); //botao para confirmar a edição dos selecionados
    selecionaLinhas(tabela); //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});


//Alterar Selecionados CST Compra de Industria
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var controller = "EditCstCompraIndustriaMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosCstCompraInd"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCstCompraInd"); //botao para confirmar a edição dos selecionados
    selecionaLinhas(tabela); //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller);
});



//Alterar Selecionados CST Entrada Pis/Cofins
$(document).ready(function () {
    toastOpcoes();//configurar o toast
    var controller = "EditCstPisCofinsEntradaMassaModal"; //envia o nome da Action para a função
    var tabela = document.getElementById("table-graficosCstPCEntrada"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCstPisCofinsEntrada"); //botao para confirmar a edição dos selecionados

    selecionaLinhas(tabela); //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID

    //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
    alterarSelecionados(btnAlterarSelecionados, controller); 

});



//Alterar selecionas CST venda atacado simples nacional
$(document).ready(function () {
    toastOpcoes();//configurar o toast
    var tabela = document.getElementById("table-graficosCstVendaAtaSN"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCstVendaAtaSN"); //botao para confirmar a edição dos selecionados
    //se a tabela existir ele continuar
    if (tabela) {
        var linhas = tabela.getElementsByTagName("tr");//variavel para as linhas da tabela

        //laço for para as linhas
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)


            });

        }//fim for

    }//fim if tabela

    //verificar se o botão existe
    if (btnAlterarSelecionados) {
        //alterar selecionados
        btnAlterarSelecionados.addEventListener("click", function () {
            //pega os elementos da linha com a classe selecionado
            var selecionados = document.getElementsByClassName("selecionado");

            //Verificar se está selecionado
            if (selecionados.length < 1) {
                toastr.error("Selecione pelo menos uma linha para proceder a alteração do registro!");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }

            bloqueioTela();//bloqueia a tela ate a chamada do ajax

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {
                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstVendaAtaSNMassaModal?strDados=' + strDados;

                    }


                });


        });
    }//fim if botao atlerar selecionados

});

//Alterar selecionados CST venda Atacado Contribuinte
$(document).ready(function () {
    toastOpcoes();//configurar o toast
    var tabela = document.getElementById("table-graficosCstVendaAtaCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCstVendaAtaCont"); //botao para confirmar a edição dos selecionados
    //se a tabela existir ele continuar
    if (tabela) {
        var linhas = tabela.getElementsByTagName("tr");//variavel para as linhas da tabela

        //laço for para as linhas
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)


            });

        }


    }//fim if tabela
    //verificar se o botão existe
    if (btnAlterarSelecionados) {

        //alterar selecionados
        btnAlterarSelecionados.addEventListener("click", function () {
            //pega os elementos da linha com a classe selecionado
            var selecionados = document.getElementsByClassName("selecionado");

            //Verificar se está selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                toastr.error("Selecione pelo menos uma linha para proceder a alteração do registro!");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }

            bloqueioTela();//bloqueia a tela ate a chamada do ajax

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {
                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstVendaAtaContMassaModal?strDados=' + strDados;

                    }


                });


        });

    }//fim if btnAlterarSelecionados



})

//Alterar selecionados CST Venda Varejo Contribuinte
$(document).ready(function () {
    toastOpcoes();//configurar o toast
    var tabela = document.getElementById("table-graficosCstVendaVarCont"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCstVendaVarCont"); //botao para confirmar a edição dos selecionados

    //se a tabela existir ele continuar
    if (tabela) {
        var linhas = tabela.getElementsByTagName("tr"); //variavel para as linhas da tabela
        //laço for para as linhas
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)


            });

        }
    }//fim if tabela
    //verificar se o botão existe
    if (btnAlterarSelecionados) {

        //alterar selecionados
        btnAlterarSelecionados.addEventListener("click", function () {
            //pega os elementos da linha com a classe selecionado
            var selecionados = document.getElementsByClassName("selecionado");

            //Verificar se está selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                toastr.error("Selecione pelo menos uma linha para proceder a alteração do registro!");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }

            bloqueioTela();//bloqueia a tela ate a chamada do ajax

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {
                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstVendaVarContMassaModal?strDados=' + strDados;

                    }


                });


        });

    }//fim if btnAlterarSelecionados

});

//Alterar Selecionados CST Venda Varejo Consumidor Final
$(document).ready(function () {
    toastOpcoes();//configurar o toast
    var tabela = document.getElementById("table-graficosCstVendaVarCF"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCstVendaVarCF"); //botao para confirmar a edição dos selecionados

    //se a tabela existir ele continuar
    if (tabela) {
        var linhas = tabela.getElementsByTagName("tr"); //variavel para as linhas da tabela

        //laço for para as linhas
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)


            });

        }
    }//fim if tabela

    //verificar se o botão existe
    if (btnAlterarSelecionados) {

        //alterar selecionados
        btnAlterarSelecionados.addEventListener("click", function () {
            //pega os elementos da linha com a classe selecionado
            var selecionados = document.getElementsByClassName("selecionado");

            //Verificar se está selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                toastr.error("Selecione pelo menos uma linha para proceder a alteração do registro!");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }

            bloqueioTela();//bloqueia a tela ate a chamada do ajax

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {
                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstVendaVarCFMassaModal?strDados=' + strDados;

                    }


                });


        });
    }
});


//Alterar Selecionados CStPisCofinsSaida
$(document).ready(function () {
    toastOpcoes();//configurar o toast
    var tabela = document.getElementById("table-graficosCstPCSaida"); //variavel para a tabela
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCstPisCofinsSaida"); //botao para confirmar a edição dos selecionados
    //se a tabela existir ele continuar
    if (tabela) {
        var linhas = tabela.getElementsByTagName("tr"); //variavel para as linhas da tabela
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];


            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)


            });


        }
    }
    
    if (btnAlterarSelecionados) {
        //alterar selecionados
        btnAlterarSelecionados.addEventListener("click", function () {
            //pega os elementos da linha com a classe selecionado
            var selecionados = document.getElementsByClassName("selecionado");

            //Verificar se está selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                toastr.error("Selecione pelo menos uma linha para proceder a alteração do registro!");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }

            bloqueioTela();//bloqueia a tela ate a chamada do ajax

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {
                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Tributacao/EditCstPisCofinsSaidaMassaModal?strDados=' + strDados;

                    }


                });


        });
    }
    


});

//Alterar Selecionados Codigo de Barras
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var tabela = document.getElementById("table-graficosCodBarras"); //variavel para a tabela
    
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCodBarras");

    if (tabela) {
        var linhas = tabela.getElementsByTagName("tr"); //variavel para as linhas da tabela
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)


            });
        }
    }
    
    if (btnAlterarSelecionados) {
        //alterar selecionados
        btnAlterarSelecionados.addEventListener("click", function () {
            //pega os elementos da linha com a classe selecionado
            var selecionados = document.getElementsByClassName("selecionado");

            //Verificar se está selecionado
            if (selecionados.length == 0) {
                toastr.error("Selecione um registro para ser alterado");
                /* alert("Selecione pelo menos uma linha");*/
                return false;
            } else {
                if (selecionados.length > 1) {
                    toastr.error("Edição em massa de COD. DE BARRAS do registro não permitida, selecione apenas uma linha.");
                    /* alert("Selecione pelo menos uma linha");*/
                    return false;
                }
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }

            bloqueioTela();//bloqueia a tela ate a chamada do ajax

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {
                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Produto/EditCodBarrasMassaModal?strDados=' + strDados;

                    }


                });


        });
    }
   
});


//Alterar selecionados CEST
$(document).ready(function () {

    var tabela = document.getElementById("table-graficosCest"); //variavel para a tabela
   
    var btnAlterarSelecionados = document.getElementById("editarSelecionadosCest");
    if (tabela) {
        var linhas = tabela.getElementsByTagName("tr"); //variavel para as linhas da tabela
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)

            });
        }
    }

    
    function selLinha(linha, multiplos) {
        if (!multiplos) {

            var linhas = linha.parentElement.getElementsByTagName("tr");
            for (var i = 0; i < linhas.length; i++) {
                var linha_ = linhas[i];
                linha_.classList.remove("selecionado");
            }
        }
        linha.classList.toggle("selecionado");
    }//fim funcao

    if (btnAlterarSelecionados) {
        //alterar selecionados
        btnAlterarSelecionados.addEventListener("click", function () {

            //pega os elementos da linha com a classe selecionado
            var selecionados = document.getElementsByClassName("selecionado");

            //Verificar se está selecionado
            if (selecionados.length < 1) {
                alert("Selecione pelo menos uma linha");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }

            bloqueioTela();


            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {
                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Produto/EditCestMassaModal?array=' + strDados;

                    }


                });


        });
    }
   



}); //fim alterar selecionados CEST




//tributacaoncm
$(document).ready(function () {

    /** Script para selecionar a linha tabela */
    var tabela = document.getElementById("tablepr-2");

    var btnAlterarSelecionadosTribNcm = document.getElementById("editarSelecionadosTribNCM");
    var btnAlterarSelecionadosTribComNCM = document.getElementById("editarSelecionadosTribComNCM"); //pega os ids para alterar com ncm (id da tabela tributacaoncm)
    var btnAlterarPorNCMTribNcm = document.getElementById("editarPorNcmTribNcm");

   

    if (tabela) {
        var linhas = tabela.getElementsByTagName("tr");
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)

            });
        }
    }



    function selLinha(linha, multiplos) {
        if (!multiplos) {

            var linhas = linha.parentElement.getElementsByTagName("tr");
            for (var i = 0; i < linhas.length; i++) {
                var linha_ = linhas[i];
                linha_.classList.remove("selecionado");
            }
        }
        linha.classList.toggle("selecionado");
    }

    if (btnAlterarSelecionadosTribComNCM) {
        //alterar selecionados
        btnAlterarSelecionadosTribComNCM.addEventListener("click", function () {


            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length > 1) {
                alert("Apenas um registro de NCM pode ser selecionado!! os dados nessa tabela NÃO SE REPETEM");
                return false;
            }
            if (selecionados.length == 0) {
                alert("Selecione uma linha para alterar");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";
            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }
            bloqueioTela();

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {

                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        //mandar para alterar so o cest e o ncm
                        window.location.href = '/Tributacao/TributacaoNcmEditMassaModalComNCM?array=' + strDados;

                    }


                });


        });

    }


    if (btnAlterarSelecionadosTribNcm) {
        //alterar selecionados
        btnAlterarSelecionadosTribNcm.addEventListener("click", function () {


            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                alert("Selecione pelo menos uma linha");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";
            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }
            bloqueioTela();

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {

                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        //mandar para alterar so o cest e o ncm
                        window.location.href = '/Tributacao/TributacaoNcmEditMassaModal?array=' + strDados;

                    }


                });


        });

    }

  
    //selecionado por ncm
    if (btnAlterarPorNCMTribNcm) {
        //alterar selecionados
        btnAlterarPorNCMTribNcm.addEventListener("click", function () {

            var titulo = document.getElementById("titulo");

           // alert(titulo.innerHTML);

            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length > 1) {
                alert("Para alterar usando o NCM selecione somente uma linha");
                return false;
            }
            if (selecionados.length == 0) {
                alert("Selecione uma linha para alterar");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var dadosNCM = {};
            var strDadosNCM = "";
            var strDados = "";
            var tituloPagina = titulo.innerHTML;
            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                if (titulo.innerHTML != "TributacaoNcm") {
                    dadosNCM[i] = selecionado[3].innerHTML;
                } else {
                    dadosNCM[i] = selecionado[5].innerHTML;
                }
              
                dados[i] = dados[i].trim();
                dadosNCM[i] = dadosNCM[i].trim();
                strDados += dados[i];
                strDadosNCM = dadosNCM[i];


            }
            bloqueioTela();

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {

                    data: { id: strDados, ncm: strDadosNCM, titulo: tituloPagina  },
                    types: "GET",
                    processData: true,
                    success: function () {
                        //alterear pelo ncm - mesmo ncm mesma tributacao - altera tudo, inclusive a tributacao
                        window.location.href = '/Tributacao/TributacaoNcmEditMassaNCMModal?id=' + strDados + '&ncm=' + strDadosNCM + '&titulo=' + tituloPagina;

                    }


                });


        });

    }


});





//alterar selecionados NCM
$(document).ready(function () {

    /** Script para selecionar a linha tabela */
    var tabela = document.getElementById("table-graficos2");
    
    var btnAlterarSelecionados = document.getElementById("editarSelecionados");
    var btnAlterarPorNCM = document.getElementById("editarPorNcm");

    var btnTributacaoPorNCM = document.getElementById("tributacaoPorNcm");

    if (tabela) {
        var linhas = tabela.getElementsByTagName("tr");
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)

            });
        }
    }
   
   

    function selLinha(linha, multiplos) {
        if (!multiplos) {
          
            var linhas = linha.parentElement.getElementsByTagName("tr");
            for (var i = 0; i < linhas.length; i++) {
                var linha_ = linhas[i];
                linha_.classList.remove("selecionado");
            }
        }
        linha.classList.toggle("selecionado");
    }
   
    if (btnAlterarSelecionados) {
        //alterar selecionados
        btnAlterarSelecionados.addEventListener("click", function () {


            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                alert("Selecione pelo menos uma linha");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";
            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }
             bloqueioTela();

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {

                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Produto/EditMassaModal?array=' + strDados;

                    }


                });


        });

    }

    //tributacaoPorNCM
    if (btnTributacaoPorNCM) {
        btnTributacaoPorNCM.addEventListener("click", function () {

            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            if (selecionados.length > 1) {
                alert("Para alterar TRIBUTAÇÃO usando o NCM selecione somente uma linha");
                return false;
            }
            if (selecionados.length == 0) {
                alert("Selecione PELO MENOS uma linha para alterar");
                return false;
            }
            var dados = {}; //variavel auxiliar para receber o ID
            var dadosNCM = {};
            var strDadosNCM = "";
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dadosNCM[i] = selecionado[5].innerHTML;
                dados[i] = dados[i].trim();
                dadosNCM[i] = dadosNCM[i].trim();
                strDados += dados[i];
                strDadosNCM = dadosNCM[i];


            }
            bloqueioTela();


            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {

                    data: { id: strDados, ncm: strDadosNCM },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Produto/EditTributacaoMassaNCMModal?id=' + strDados + '&ncm=' + strDadosNCM;

                    }


                });


        });
    }

    //selecionado por ncm
    if (btnAlterarPorNCM) {
        //alterar selecionados
        btnAlterarPorNCM.addEventListener("click", function () {


            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length > 1) {
                alert("Para alterar usando o NCM selecione somente uma linha");
                return false;
            }
            if (selecionados.length == 0) {
                alert("Selecione uma linha para alterar");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var dadosNCM = {};
            var strDadosNCM = "";
            var strDados = "";
            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dadosNCM[i] = selecionado[5].innerHTML;
                dados[i] = dados[i].trim();
                dadosNCM[i] = dadosNCM[i].trim();
                strDados += dados[i];
                strDadosNCM = dadosNCM[i];
                

            }
            bloqueioTela();

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {

                    data: { id: strDados, ncm : strDadosNCM },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Produto/EditMassaNCMModal?id=' + strDados + '&ncm=' + strDadosNCM;

                    }


                });


        });

    }
   

});







function maiuscula(z) {
    v = z.value.toUpperCase();
    z.value = v;
}
function minuscula(z) {
    v = z.value.toLowerCase();
    z.value = v;
}

    //mascaras
    $(document).ready(function () {
        $('.date').mask('99/99/9999');
        $('.time').mask('00:00:00');
        $('#cep').mask('99.999-999');
        $('.phone').mask('9999-9999');
        $('#cnpj').mask('99.999.999/9999-99');
        $('#telefone').mask('(99)99999-9999');
        $(".senha").mask("xxxxxxxxx");
        $("#cest").mask("99.999.99");
        $("#ncm").mask("9999.99.99");
        $(".decimal").mask("9999,999");
        $(".pr-aliq").mask("9999.999");
    });

//função selecionar a linha
function selLinha(linha, multiplos) {
    if (!multiplos) {

        var linhas = linha.parentElement.getElementsByTagName("tr");
        for (var i = 0; i < linhas.length; i++) {
            var linha_ = linhas[i];
            linha_.classList.remove("selecionado");
        }
    }
    linha.classList.toggle("selecionado");
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


//opcoes toast
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


////alterar selecionados com post
//function alterarSelecionadosPost(par1, par2)
//{
//    //verificar se o botão existe
//    if (par1) {
//        //alterar selecionados
//        par1.addEventListener("click", function () {
//            //pega os elementos da linha com a classe selecionado
//            var selecionados = document.getElementsByClassName("selecionado");

//            //Verificar se está selecionado
//            if (selecionados.length < 1) {
//                toastr.error("Selecione pelo menos uma linha para proceder a alteração do registro!");
//                return false;
//            }

//            var dados = {}; //variavel auxiliar para receber o ID
//            var strDados = "";

//            /*Laço para varrer os elementos com a tag TD*/
//            for (var i = 0; i < selecionados.length; i++) {
//                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
//                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
//                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
//                dados[i] = dados[i].trim();
//                strDados += dados[i] + ",";

//            }

//            bloqueioTela();//bloqueia a tela ate a chamada do ajax

            

//            ////agora mandar esse vetor para o modal e passar o valor correto do ncm
//            $.ajax(
//                {
//                    data: { array: strDados },
//                    types: "GET",
//                    processData: true,
//                    success: function () {

//                        window.location.href = '/Tributacao/' + par2 + '?strDados=' + strDados;

//                    }

//                });
           

//        });
//    }//fim if botao atlerar selecionados

//}


function alterarSelecionados(par1, par2) {
    //verificar se o botão existe
    if (par1) {
        //alterar selecionados
        par1.addEventListener("click", function () {
            //pega os elementos da linha com a classe selecionado
            var selecionados = document.getElementsByClassName("selecionado");

            //Verificar se está selecionado
            if (selecionados.length < 1) {
                toastr.error("Selecione pelo menos uma linha para proceder a alteração do registro!");
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }

            bloqueioTela();//bloqueia a tela ate a chamada do ajax

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {
                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {
                        
                        window.location.href = '/Tributacao/' + par2 + '?strDados=' + strDados;

                    }

                });

        });
    }//fim if botao atlerar selecionados
}

function selecionaLinhas(paramTabela) {
    //se a tabela existir ele continuar
    if (paramTabela) {
        var linhas = paramTabela.getElementsByTagName("tr");//variavel para as linhas da tabela
        //laço for para as linhas
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)


            });

        }//fim for

    }//fim if tabela
}

/*Liberar a tela apos a execução do codigo dentro da chamada ajax*/
$(document).ajaxStop($.unblockUI);

//esconder mensagem de alerta quando for mostrada
$(document).ready(function () {
    setTimeout(function () {
        $(".alert").fadeOut("slow", function () {

            $(this).alert('close');
        });

    }, 3000);
});
