
/**
 * Uma classe que encapsula as operações de CRUD dos agentes.
 */
class ApiAgentes {
    /**
     * Cria uma nova instância da classe ApiAgentes.
     * @param {string} linkApi O link "base" da API.
     */
    constructor(linkApi) {
        this.linkApi = linkApi;
    }

    /**
     * Constrói o link para a foto de um agente.
     * @param {number} id ID do agente.
     * @returns {string} link para a foto.
     */
    getLinkFoto(id) {
        return this.linkApi + "/api/agentes/" + id + "/foto";
    }

    /**
     * Obtém a lista dos agentes.
     * @param {string} pesquisa Termo de pesquisa.
     * @param {bool} comFoto Só mostrar agentes com foto.
     * @returns {Promise<any[]>} Array de Agentes.
     */
    async getAgentes(pesquisa = "", comFoto = false) {

        let query = [];

        if (pesquisa) {
            query.push("pesquisa=" + encodeURIComponent(pesquisa));
        }

        if (comFoto === true) {
            query.push("comFoto=" + encodeURIComponent(comFoto));
        }

        let queryString = "?" + query.join("&");




        // A classe URLSearchParams é usada para construir query strings
        // (isto é ?pesquisa=ourém&comFoto=true)
        // de forma dinâmica.
        //let termosPesquisa = new URLSearchParams();

        //if (pesquisa) {
        //    termosPesquisa.set("pesquisa", pesquisa);
        //}

        //if (comFoto) {
        //    termosPesquisa.set("comFoto", "true");
        //}

        let resposta = await fetch(this.linkApi + "/api/agentes?" + termosPesquisa.toString(), {
            method: 'GET',
            headers: {
                Accept: 'application/json'
            }
        });
        
        if (!resposta.ok) {
            let textoErro = await resposta.text();
            throw new Error(textoErro);
        }

        let agentes = await resposta.json();

        return agentes;
    }

    /**
     * Obtém um agente através do seu ID.
     * @param {number} id ID do agente.
     * @returns {Promise<any>} Agente.
     */
    async getAgente(id) {
        let resposta = await fetch(this.linkApi + "/api/agentes/" + id, {
            method: 'GET',
            headers: {
                Accept: 'application/json'
            }
        });

        if (!resposta.ok) {
            let textoErro = await resposta.text();
            throw new Error(textoErro);
        }

        let agente = await resposta.json();

        return agente;
    }

    /**
     * Cria um agente na API.
     * @param {string} nome Nome do agente.
     * @param {string} esquadra Esquadra do agente.
     * @param {File} foto Ficheiro da foto do agente.
     * @returns {Promise<any>} Agente criado.
     */
    async createAgente(nome, esquadra, foto) {
        // Usar um objecto do tipo FormData permite-nos enviar ficheiros por AJAX.
        let form = new FormData();

        form.append("nome", nome);
        form.append("esquadra", esquadra);
        form.append("fotografia", foto);

        let resposta = await fetch(this.linkApi + "/api/agentes", {
            method: 'POST',
            headers: {
                // NOTA: não coloco o Content-Type, porque quando uso FormData
                // com o fetch, este é definido automaticamente.
                'Accept': 'application/json'
            },
            body: form
        });

        if (!resposta.ok) {
            let textoErro = await resposta.text();
            throw new Error(textoErro);
        }

        let agenteCriado = await resposta.json();

        return agenteCriado;
    }

    /**
     * Atualiza um agente na API.
     * @param {number} id ID do agente.
     * @param {string} nome Novo nome do agente
     */
    async updateAgente(id, nome) {
        let body = {
            nome: nome
        };

        let resposta = await fetch(this.linkApi + "/api/agentes/" + id, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(body)
        });

        if (!resposta.ok) {
            let textoErro = await resposta.text();
            throw new Error(textoErro);
        }

        // A API não devolve nada se o update for OK.
        // Se precisarmos dos dados atualizados, 
        // podemos usar a função para obter um agente aqui.
        // Isto é opcional.
        // return await this.getAgente(id);
    }

    /**
     * Apaga um agente da API.
     * @param {number} id ID do agente a apagar.
     */
    async deleteAgente(id) {
        let resposta = await fetch(this.linkApi + "/api/agentes/" + id, {
            method: 'DELETE',
            headers: {}
        });

        if (!resposta.ok) {
            let textoErro = await resposta.text();
            throw new Error(textoErro);
        }

        // Não tenho retorno, não há nada a fazer se o agente for apagado com sucesso.
    }
}
