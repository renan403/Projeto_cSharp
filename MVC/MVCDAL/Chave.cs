﻿using System.Text;

namespace MVCDAL.Produto
{
    public class Chave:IDisposable
    {
        protected const string chave = "S_g4#1/=O09ds032*-4/Çvp9)4,!F6pX";
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }

        public static Tuple<byte[], byte[]> GetKey()
        {
           
            var tuple = new Tuple<byte[], byte[]>
                (
                Encoding.ASCII.GetBytes(chave),
                Encoding.ASCII.GetBytes("cha1212112veTest")
                );
            return tuple;
        }

        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
