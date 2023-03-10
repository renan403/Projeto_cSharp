using System.Security.Cryptography;
using System.Text;
using Seguranca;

namespace MVC.Funcoes
{
    public class Salvar : Chave
    {
        public byte[] Criptografar(string txt)
        {
            using (Aes myAes = Aes.Create())
            {
                myAes.Key = Encoding.ASCII.GetBytes("S_g4#1/=O09ds032*-4/Çvp9)4,!F6pX");
                myAes.IV = Encoding.ASCII.GetBytes("cha1212112veTest");

                byte[] encriptado = Cript.EncryptarStringParaByte(txt, myAes.Key, myAes.IV, "#P>EET|MkkPa{oE0[Zcm");
                return encriptado;
            }

        }

    }
}