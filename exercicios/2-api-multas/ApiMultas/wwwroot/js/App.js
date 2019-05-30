// Uso a API a apontar para o próprio servidor,
// logo não meto o link (http://localhost:5000).
// Se fosse outro servidor, teria que colocar aqui o link.
let api = new ApiAgentes("");

////////////////////////////////////////////////////////////////////////////////////////////////////
// CRIAR AGENTES
////////////////////////////////////////////////////////////////////////////////////////////////////

document.getElementById('criarAgente').onsubmit = async (evt) => {
    // Impedir que o browser submeta
    evt.preventDefault();
    try {

        let nome = document.getElementById('nome').value;
        let esquadra = document.getElementById('esquadra').value;

        // Para obter ficheiros, usa-se o 'files' (é um array) num <input type="file" />
        let foto = document.getElementById('foto').files[0];

        let novoAgente = await api.createAgente(nome, esquadra, foto);

        // Mostrar o novo agente no ecrã.
        let novoDivAgente = criarDivAgente(novoAgente);
        document.getElementById('agentes').appendChild(novoDivAgente);

    } catch (e) {
        console.error("Erro ao criar o agente", e);
        alert("Ocorreu um erro ao criar o agente. Tente novamente.");
    }
};

////////////////////////////////////////////////////////////////////////////////////////////////////
// PESQUISA
////////////////////////////////////////////////////////////////////////////////////////////////////

document.getElementById('pesquisaAgentesForm').onsubmit = async (evt) => {
    evt.preventDefault();

    let pesquisa = document.getElementById('pesquisaAgentes').value;

    try {
        let agentes = await api.getAgentes(pesquisa);
        mostraListaAgentes(agentes);
    } catch (e) {
        console.error("Erro ao pesquisar", e);
        alert("Ocorreu um erro ao efetuar a pesquisa. Tente novamente.");
    }
};

////////////////////////////////////////////////////////////////////////////////////////////////////
// MAIN
////////////////////////////////////////////////////////////////////////////////////////////////////

async function main() {
    try {
        let agentes = await api.getAgentes();
        mostraListaAgentes(agentes);
    } catch (e) {
        console.error("Erro ao obter agentes", e);
        alert("Ocorreu um erro ao obter os agentes. Tente novamente.");
    }
}


document.addEventListener('DOMContentLoaded', () => {
    main();
});

////////////////////////////////////////////////////////////////////////////////////////////////////
// FUNÇÕES AUXILIARES
////////////////////////////////////////////////////////////////////////////////////////////////////

/**
 * Mostra os detalhes do agente no div com ID #detalhesAgente.
 * @param {any} agente Agente para qual mostrar os dados.
 */
function mostraDetalhesAgente(agente) {
    let detalhes = document.getElementById('detalhesAgente');

    // Esconder o pop-up dos detalhes quando se clica no botão
    document.getElementById('esconderDetalhes').onclick = (e) => {
        e.preventDefault();

        detalhes.classList.add('hidden');
    };

    document.getElementById('nomeAgente').textContent = agente.nome;
    document.getElementById('fotoAgente').src = api.getLinkFoto(agente.id);

    let containerMultas = document.querySelector('#multas > tbody');

    // Limpar as multas
    containerMultas.innerHTML = "";

    // FORMATAR DATAS
    // Posso usar um objeto Intl.DateTimeFormat para formatar datas com diversos aspectos.
    // https://developer.mozilla.org/pt-BR/docs/Web/JavaScript/Reference/Global_Objects/DateTimeFormat
    let dateFormat = new Intl.DateTimeFormat("pt", { year: 'numeric', month: 'long', day: 'numeric' });

    for (let multa of agente.listaDeMultas) {
        let row = document.createElement('tr');

        let tdLocal = document.createElement('td');
        tdLocal.textContent = multa.localDaMulta;
        row.appendChild(tdLocal);

        let tdInfra = document.createElement('td');
        tdInfra.textContent = multa.infracao;
        row.appendChild(tdInfra);

        let tdValor = document.createElement('td');
        tdValor.textContent = multa.valorMulta;
        row.appendChild(tdValor);

        let tdData = document.createElement('td');
        // Usar o .format() do DateTimeFormat para formatar a data.
        // Nota: A data vem como TEXTO no JSON (JSON não suporta datas, logo estas vêm como texto, ou números).
        tdData.textContent = dateFormat.format(new Date(multa.dataDaMulta));
        row.appendChild(tdData);

        containerMultas.appendChild(row);
    }

    detalhes.classList.remove('hidden');
}

/**
 * Coloca no div com o ID #agentes uma lista de agentes.
 * @param {any[]} agentes Lista de agentes.
 */
function mostraListaAgentes(agentes) {
    let container = document.querySelector('#agentes');

    container.innerHTML = "";

    for (let agente of agentes) {
        let divAgente = criarDivAgente(agente);

        container.appendChild(divAgente);
    }
}

/**
 * Método auxiliar para criar uma div para a lista dos agentes.
 * @param {any} agente Agente para qual fazer o div.
 * @returns {HTMLDivElement} Div com o elemento da lista.
 */
function criarDivAgente(agente) {
    let divAgente = document.createElement("div");

    let imgAgente = document.createElement("img");
    imgAgente.src = api.getLinkFoto(agente.id);
    divAgente.appendChild(imgAgente);

    let nomeAgente = document.createElement("h4");
    nomeAgente.textContent = agente.nome;
    divAgente.appendChild(nomeAgente);

    // Botão detalhes para mostrar o pop-up com os detalhes.
    let btnDetalhes = document.createElement('button');
    btnDetalhes.type = "text";
    btnDetalhes.textContent = "Ver mais";
    btnDetalhes.onclick = async (e) => {
        e.preventDefault();

        try {
            let detalhesAgente = await api.getAgente(agente.id);

            mostraDetalhesAgente(detalhesAgente);
        } catch (e) {
            console.error("Erro ao obter o agente.", e);
            alert("Erro ao obter o agente.");
        }
    };

    divAgente.appendChild(btnDetalhes);

    return divAgente;
}
