// Uso a API a apontar para o próprio servidor,
// logo não meto o link (http://localhost:5000).
// Se fosse outro servidor, teria que colocar aqui o link.
let api = new ApiAgentes("");

function mostraDetalhesAgente(agente) {
    let detalhes = document.getElementById('detalhesAgente');

    document.getElementById('esconderDetalhes').onclick = (e) => {
        e.preventDefault();

        detalhes.classList.add('hidden');
    };

    document.getElementById('nomeAgente').textContent = agente.nome;
    document.getElementById('fotoAgente').src = api.getLinkFoto(agente.id);

    let containerMultas = document.querySelector('#multas > tbody');

    // Limpar as multas
    containerMultas.innerHTML = "";

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
        tdData.textContent = dateFormat.format(new Date(multa.dataDaMulta));
        row.appendChild(tdData);

        containerMultas.appendChild(row);
    }

    detalhes.classList.remove('hidden');
}

function mostraListaAgentes(agentes) {
    let container = document.querySelector('#agentes');

    container.innerHTML = "";

    for (let agente of agentes) {
        let divAgente = criarDivAgente(agente);

        container.appendChild(divAgente);
    }
}

function criarDivAgente(agente) {
    let divAgente = document.createElement("div");

    let imgAgente = document.createElement("img");
    imgAgente.src = api.getLinkFoto(agente.id);
    divAgente.appendChild(imgAgente);

    let nomeAgente = document.createElement("h4");
    nomeAgente.textContent = agente.nome;
    divAgente.appendChild(nomeAgente);

    // Botão detalhes.
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


////////////////////////////////////////////////////////////////////////////////////////////////////
// CRIAR AGENTES
////////////////////////////////////////////////////////////////////////////////////////////////////

document.getElementById('criarAgente').onsubmit = async (evt) => {
    // Impedir que o browser submeta
    evt.preventDefault();
    try {

        let nome = document.getElementById('nome').value;
        let esquadra = document.getElementById('esquadra').value;

        // Para obter ficheiros, usa-se o 'files' (é um array)
        let foto = document.getElementById('foto').files[0];

        let novoAgente = await api.createAgente(nome, esquadra, foto);

        // Mostrar o novo agente no ecrã.
        let novoDivAgente = criarDivAgente(novoAgente);

        document.getElementById('agentes').appendChild(novoDivAgente);
    } catch (e) {
        console.error("Erro ao criar o agente", e);
    }
};

////////////////////////////////////////////////////////////////////////////////////////////////////
// PESQUISA
////////////////////////////////////////////////////////////////////////////////////////////////////

document.getElementById('pesquisaAgentesForm').onsubmit = async (evt) => {
    evt.preventDefault();

    let pesquisa = document.getElementById('pesquisaAgentes').value;

    let agentes = await api.getAgentes(pesquisa);
    mostraListaAgentes(agentes);
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
    }
}


document.addEventListener('DOMContentLoaded', () => {
    main();
});
