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
        }
    }
    linha.classList.toggle("selecionado");
}

var btnEditTrib = document.getElementById("editartrib"); //variavel que representa o botão

/*Botão editar tributação*/
btnEditTrib.addEventListener("click", function () {
    var selecionados = document.getElementsByClassName("selecionado"); //pega os elementos da linha com a classe selecionado
    //Verificar se está selecionado
    if (selecionados.length < 1) {
        alert("Selecione pelo menos uma linha");
        return false;
    }

    var selecionado;
    var testeID;
    var convertID = "";

    /*Laço para varrer os elementos com a tag TD*/
    for (var i = 0; i < selecionados.length; i++) {
        selecionado = selecionados[i]; //variavel para conter os itens selecionados
        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
        testeID = selecionado[0].innerHTML;
        convertID += "|" + parseInt(testeID);
    }

    var dados = convertID.toString();

    $.ajax({
        url: "/TributacaoEmpresa/Edit",
        type: "POST",
        data: { convertID: dados },
        success() {
        }
    });
});
