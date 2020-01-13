using System.ComponentModel;

namespace ExemploAplicacaoWeb.Business.Enums
{
    public enum TipoAutenticacaoEnum
    {
        Nenhuma,
        oAuth,
        basic,
        bearer
    }
    public enum TipoConteudoEnum
    {
        [Description("application/x-www-form-urlencoded")]
        Formulario,
        [Description("application/json")]
        Json
    }
    /// <summary>
    /// Metodo retorna a descrição dos itens no enumerador em questão
    /// </summary>
    public static class TipoConteudoEnumExtensions
    {
        public static string ToDescriptionString(this TipoConteudoEnum? val)
        {
            if (val == null)
            {
                return null;
            }
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
