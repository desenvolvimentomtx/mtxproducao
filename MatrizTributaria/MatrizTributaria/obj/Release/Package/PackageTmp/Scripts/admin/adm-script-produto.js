$(document).ready(function () {
    /** Script para selecionar a linha tabela */
    var tabela = document.getElementById("tablepr");
    var tabela = document.getElementById("tablepr-user");
    var tabela = document.getElementById("tablepr-2");
    var linhas = document.getElementsByTagName("tr");
    var btnEditar = document.getElementById("editarDados"); //variavel que representa o botão
    var btnEditarCategoria = document.getElementById("editarDadosCategoria"); //variavel que representa o botão
    var botaoSavarlAlteracaoCat = document.getElementById("savarlAlteracaoCat"); //variavel que recebe o botao da alteracao do ncm
    var btnVisualizar = document.getElementById("visualizarDados"); //variavel que representa o botão
    var btnApagar = document.getElementById("apagarDados"); //variavel que representa o botao de apagar

    var a = document.querySelector(".pr-titulo"); //pegar o nome do controler
    var controller = a.innerText;


    for (var i = 0; i < linhas.length; i++) {
        var linha = linhas[i];
        linha.addEventListener("click", function () {
            //Adicionar ao atual
            selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)

        });
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
            if (selecionados.length > 1) {
                alert("Selecione apenas uma linha para essas alterações");
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

    //editar de multiplos
    if (btnEditarCategoria) {
        /*Função para pegar o clique do botão editar*/
        btnEditarCategoria.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
            //Verificar se está selecionado
            if (selecionados.length < 1) {
                alert("Selecione pelo menos uma linha");
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
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";
            }

            /* var id = parseInt(dados); //converte para inteiro*/
            bloqueioTela();//bloqueia tela

            $.ajax(
                {

                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {
                        //window.location.href = controller + '/editVarios?id=' + id

                        //$("#modalEdit").load(controller + '/editVarios?id=' + id, function () {
                        //    $("#modalEdit").modal();
                        //})

                        //mandar para alterar so o cest e o ncm
                        window.location.href = controller + '/EditVarios?array=' + strDados;

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

   //salvar categoria em massa
    if (botaoSavarlAlteracaoCat) {
        botaoSavarlAlteracaoCat.addEventListener("click", function () {
            var dados = {}
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado

            //os ids para alterar
            var ncmMudar = document.getElementById("ncm");
            var cestMudar = document.getElementById("cest"); //pegar o valor do input
            var categoriaMudar = document.getElementById("categor");

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";
            var ncm = ncmMudar.value;
            var cest = cestMudar.value; // //variavel que recebe o valor do input
            var caTeg = categoriaMudar.value;

            if (ncm == "" && cest == "" && caTeg == "")
            {
                document.getElementById("ncm").focus();
                toastr.error("Nenhum valor informado para alterações");
            }


            if (cest != "")
            {
                if (cest.length != 9) {
                    /* alert("Tamanho do NCM incorreto! Digite novamente");*/
                    document.getElementById("cest").focus();
                    /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
                    toastr.error("Tamanho do CEST incorreto! Digite novamente");

                }

            }
            if (ncm != "")
            {
                if (ncm.length != 10) {
                    /* alert("Tamanho do NCM incorreto! Digite novamente");*/

                    document.getElementById("ncm").focus();
                    /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
                    toastr.error("Tamanho do NCM incorreto! Digite novamente");
                }
            }


            if (ncm != "" || cest != "" || caTeg != "" ) {


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

                        data: { strDados: strDados, ncm: ncm, cest: cest, caTeg: caTeg },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Produto/EdtVariosPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest + '&categ=' + caTeg;

                        }


                    });


                


            }
           

        });
    }

});



////salvar categoria em massa
//$(document).ready(function () {
//    toastOpcoes();
//    var botaoSavarlAlteracaoCat = document.getElementById("savarlAlteracaoCat"); //variavel que recebe o botao da alteracao do ncm
   

//    //verificar se o botao existe
//    if (botaoSavarlAlteracaoCat) {
//        botaoSavarlAlteracaoCat.addEventListener("click", function () {
//            var dados = {}
//            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado

//            //os ids para alterar
//            var ncmMudar = document.getElementById("ncm");
//            var cestMudar = document.getElementById("cest"); //pegar o valor do input
//            var categoriaMudar = document.getElementById("categor");

//            var dados = {}; //variavel auxiliar para receber o ID
//            var strDados = "";
//            var ncm = ncmMudar.value;
//            var cest = cestMudar.value; // //variavel que recebe o valor do input
//            var caTeg = categoriaMudar.value;

//            if (cest != "") {
//                if (cest.length != 9) {
//                    /* alert("Tamanho do NCM incorreto! Digite novamente");*/
//                    document.getElementById("cest").focus();
//                    /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
//                    toastr.error("Tamanho do CEST incorreto! Digite novamente");

//                }



//            }


//            if (ncm != "") {
//                if (ncm.length != 10) {
//                    /* alert("Tamanho do NCM incorreto! Digite novamente");*/

//                    document.getElementById("ncm").focus();
//                    /* swal('Tamanho do NCM incorreto! Digite novamente"');*/
//                    toastr.error("Tamanho do NCM incorreto! Digite novamente");

//                } else {
//                    /*Laço para varrer os elementos com a tag TD*/
//                    for (var i = 0; i < selecionados.length; i++) {
//                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
//                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
//                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
//                        dados[i] = dados[i].trim();
//                        strDados += dados[i] + ",";
//                    }

//                    bloqueioTela();//bloqueia tela

//                    //agora o ajax
//                    $.ajax({

//                        data: { strDados: strDados, ncm: ncm, cest: cest, categ: caTeg },
//                        types: "GET",
//                        processData: true,
//                        success: function () {

//                            window.location.href = '/Produto/EdtVariosPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest + '&categ='+categ;

//                        }


//                    });


//                }


//            }
//            //else {
//            //    var resultado = confirm("O NCM informado foi NULO, deseja continuar ?");
//            //    if (resultado == true) {
//            //        /*Laço para varrer os elementos com a tag TD*/
//            //        for (var i = 0; i < selecionados.length; i++) {
//            //            var selecionado = selecionados[i]; //variavel para conter os itens selecionados
//            //            selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
//            //            dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
//            //            dados[i] = dados[i].trim();
//            //            strDados += dados[i] + ",";
//            //        }

//            //        bloqueioTela();//bloqueia tela

//            //        //agora o ajax
//            //        $.ajax({

//            //            data: { strDados: strDados, ncm: ncm, cest: cest },
//            //            types: "GET",
//            //            processData: true,
//            //            success: function () {
//            //                window.location.href = '/Tributacao/TributacaoNcmEditMassaModalEditMassaModalPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;
//            //                //window.location.href = '/Produto/EditMassaModalPost?strDados=' + strDados + '&ncm=' + ncm + '&cest=' + cest;

//            //            }

//            //        });


//            //    } else {
//            //        toastr.warning("Atribuição de NCM para os produtos selecionados abortada");
//            //        /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
//            //        document.getElementById("ncm").focus();
//            //    }

//            //}



//        });
//    }

    


//});



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