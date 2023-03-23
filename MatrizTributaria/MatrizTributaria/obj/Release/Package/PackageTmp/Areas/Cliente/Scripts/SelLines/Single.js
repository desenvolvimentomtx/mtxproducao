
/*Laço para pegar linhas Selected*/
var linhas = document.getElementsByTagName("tr");

for (var i = 0; i < linhas.length; i++) {
    var linha = linhas[i];
    linha.addEventListener("click", function () {
        //Adicionar ao atual
        selLinha(this); //Selecione apenas um (parametro true seleciona mais de uma linha)

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

var btnEditar = document.getElementById("editarDados"); //variavel que representa o botão
var btnVisualizar = document.getElementById("visualizarDados"); //variavel que representa o botão
var btnApagar = document.getElementById("apagarDados"); //variavel que representa o botao de apagar
var a = document.querySelector(".pr-titulo"); //pegar o nome do controler
var controller = a.innerText;

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

    /*Chama o modal passando o id como parametro
    $("#modal").load("Usuario/Details?id=" + id, function () {
        $("#modal").modal();
    })*/

    $.ajax(
        {
            url: controller + '/Edit',
            data: { id: id },
            types: "GET",
            processData: true,
            success: function () {
                window.location.href = controller + '/edit?id=' + id
            }

        });

});

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

    /*Chama o modal passando o id como parametro
    $("#modal").load("Usuario/Details?id=" + id, function () {
        $("#modal").modal();
    })*/

    $.ajax(
        {
            url: controller + '/Delete',
            data: { id: id },
            types: "GET",
            processData: true,
            success: function () {
                window.location.href = controller + '/delete?id=' + id
            }

        });

});

/*Função para pegar o clique do botão VISUALIZAR*/
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