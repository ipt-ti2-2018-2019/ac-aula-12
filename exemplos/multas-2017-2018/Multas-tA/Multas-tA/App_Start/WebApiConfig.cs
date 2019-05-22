using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Multas
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            #region Formatação JSON e XML

            // Desligar o formatador do XML.
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Impedir referências circulares no JSON.
            // Nota: isto é uma má ideia, porque esconde erros no nosso código.
            // É preferível usar a anotação [JsonIgnore] no campo que se quer ignorar.
            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = 
            //    Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            // Pretty-print do JSON
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = 
                Newtonsoft.Json.Formatting.Indented;

            // Opcional: Converter os nomes de propriedades PascalCase (a la .net)
            // para camelCase (a la Java/JavaScript).
            // Descomentar as duas seguintes linhas mudará o JSON devolvido.

            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
            //    new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            #endregion
            
            #region Routing

            // Configuração do Attribute Routing para Web API.
            // isto não fará nada se não tivermos [Route] nos nossos métodos.
            // Isto permite que usemos [Route], combinado de 
            // [HttpGet], [HttpPost], [HttpPut], [HttpDelete], [HttpPatch]
            // para definir um route para uma função num controller.

            // Uso:
            // - [RoutePrefix] é usado num controller para definir a "raíz"
            // dos seus métodos (ex: [RoutePrefix("api/agentes")]).
            // - [Route] é usado para definir o caminho para uma função.
            // isto é combinado com o [RoutePrefix], e é "concatenado":
            // [Route("{id}/multas")] -> no controller com o [RoutePrefix] acima
            // responde a "api/agentes/5/multas".
            // - [HttpGet] e etc. podem ser combinados (podemos ter múltiplos, mas não recomendo)
            // para indicar a que métodos HTTP a função responde (GET, PUT, POST, DELETE).

            // Attribute routing toma prioriade sobre conventions-based routing (abaixo).
            config.MapHttpAttributeRoutes();

            // O equivalente do "MapRoute" do MVC (ver RouteConfig.cs nesta pasta).
            // Atenção que aqui é diferente, não se mete o "{action}",
            // o nome do método ajuda a determinar a operação.
            // Ou seja, no AgentesController:
            // - GetAgentes() -> GET /api/agentes -> Listar agentes.
            // - GetAgentes(int id) -> GET /api/agentes/{id} -> Obter um agente, pelo seu ID.
            // - PostAgentes(Agentes agente) -> POST /api/agentes -> Criar um agente. Os dados do agente vão no BODY.
            // - PutAgentes(int id, Agentes agente) -> PUT /api/agentes/{id} -> Update de um agente, pelo seu ID. Os dados do agente vão no BODY.
            // - DeleteAgentes(int id) -> DELETE /api/agentes/{id} -> Apaga um agente.
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Configuração de uma rota para as multas de um Agente.
            // Nota: Isto é um workaround para as limitações do MapHttpRoute.
            // Attribute Routing é muito melhor...
            //config.Routes.MapHttpRoute(
            //    name: "MultasDeUmAgente",
            //    routeTemplate: "api/agentes/{id}/multas",
            //    defaults: new { controller = "Agentes", action= "GetMultasByAgente" }
            //);

            #endregion


        }
    }
}
