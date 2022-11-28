using Seguranca;
using System.Security.Cryptography;
using System.Text;

namespace ApiProjeto.Functions
{
    public class Criptografia
    {
        static public string Decriptografar(byte[] txtEncrypt)
        {
            using Aes myAes = Aes.Create();

            myAes.Key = Encoding.ASCII.GetBytes("S_g4#1/=O09ds032*-4/Çvp9)4,!F6pX");
            myAes.IV = Encoding.ASCII.GetBytes("cha1212112veTest");

            string Decriptado = Cript.DecryptarStringDeBytes(txtEncrypt, myAes.Key, myAes.IV, "^*U2!}=D379BSJGdr*$w");
            return Decriptado;

        }
    }
}
