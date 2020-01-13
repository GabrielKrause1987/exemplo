using System.Net;

namespace ExemploAplicacaoWeb.Business.Modelos
{
    public class RetornoChamadaApiModelo<T>
    {
        public HttpStatusCode CodigoResponse { get; set; }
        public T ModeloRetorno { get; set; }
    }
}
