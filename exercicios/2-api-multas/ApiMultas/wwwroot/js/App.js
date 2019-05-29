// Uso a API a apontar para o próprio servidor,
// logo não meto o link (http://localhost:5000).
// Se fosse outro servidor, teria que colocar aqui o link.
let api = new ApiAgentes("");

function mostraAgentes(agentes) {
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

    return divAgente;
}

async function main() {
    try {
        let agentes = await api.getAgentes();
        mostraAgentes(agentes);
    } catch (e) {
        console.error("Erro ao obter agentes", e);
    }
}

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
}


document.addEventListener('DOMContentLoaded', () => {
    main();
});
