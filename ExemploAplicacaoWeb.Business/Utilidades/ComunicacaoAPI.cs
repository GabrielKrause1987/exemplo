using ExemploAplicacaoWeb.Business.Enums;
using ExemploAplicacaoWeb.Business.Modelos;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.IO;
using System.Net;

namespace ExemploAplicacaoWeb.Business
{
    public class ComunicacaoAPI
    {
        public Method Metodo { get; set; }
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string AccessToken { get; set; }

        /// <summary>
        /// Construtor para iniciar o processo de comunicação com APIS
        /// </summary>
        /// <param name="metodo"></param>
        /// <param name="url"></param>
        /// <param name="clientid"></param>
        /// <param name="accesstoken"></param>
        public ComunicacaoAPI(Method metodo, string url, string clientid, string accesstoken)
        {
            Metodo = metodo;
            Url = url;
            ClientId = clientid;
            AccessToken = accesstoken;
        }

        private void SetSecurityHeaders_oAuth(RestRequest request)
        {
            request.AddHeader("client_id", ClientId);
            request.AddHeader("access_token", AccessToken);
        }
        private void SetSecurityHeaders_basic(RestClient client)
        {
            client.Authenticator = new HttpBasicAuthenticator(ClientId, AccessToken);
        }
        private void SetSecurityHeaders_bearer(RestClient client)
        {
            client.AddDefaultHeader("Authorization", $"Bearer {AccessToken}");
        }

        /// <summary>
        /// Metodo para chamar a API informando os parametros necessarios
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tipoAutenticacao"></param>
        /// <param name="tipoConteudo"></param>
        /// <param name="cdJob"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public RetornoChamadaApiModelo<T> Executar<T>(TipoAutenticacaoEnum tipoAutenticacao,
            TipoConteudoEnum? tipoConteudo, decimal cdJob = 0, object body = null)
        {
            var client = new RestClient(Url);
            client.Timeout = (1000 * 450);

            var request = new RestRequest();
            request.Method = Metodo;
            request.AddHeader("Content-Type", tipoConteudo.ToDescriptionString());

            //Tipo Autenticação:
            switch (tipoAutenticacao)
            {
                case TipoAutenticacaoEnum.Nenhuma:
                    break;
                case TipoAutenticacaoEnum.oAuth:
                    SetSecurityHeaders_oAuth(request);
                    break;
                case TipoAutenticacaoEnum.basic:
                    SetSecurityHeaders_basic(client);
                    break;
                case TipoAutenticacaoEnum.bearer:
                    SetSecurityHeaders_bearer(client);
                    break;
                default:
                    break;
            }

            //Tipo conteúdo:
            if (tipoConteudo == TipoConteudoEnum.Formulario)
            {
                request.AddParameter("username", ClientId);
                request.AddParameter("password", AccessToken);
                request.AddParameter("grant_type", "password");
            }

            //Quando for POST, verificar body:
            if (request.Method == Method.POST)
            {
                if (body != null)
                {
                    request.AddJsonBody(body);
                }
            }

            //Efetuar chamada End Point:
            try
            {
                var response = client.Execute(request);
                var resultado = response.Content;
                var code = response.StatusCode;
                var retorno = JsonConvert.DeserializeObject<T>(resultado);
                return new RetornoChamadaApiModelo<T>()
                {
                    CodigoResponse = code,
                    ModeloRetorno = retorno
                };
            }
            catch (WebException ex)
            {
                var resp = ex.Response;
                var code = HttpStatusCode.InternalServerError;
                string json;
                using (Stream respStream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    json = reader.ReadToEnd();
                }
                return new RetornoChamadaApiModelo<T>()
                {
                    CodigoResponse = code,
                    ModeloRetorno = JsonConvert.DeserializeObject<T>(json)
                };
            }
        }
    }
}
