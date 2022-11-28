namespace ApiMvc.Functions
{
    public class Gerador:IDisposable
    {
        private bool disposedValue;

        public string AleatoriosUser()
        {
            var rand = new Random();
            var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var numAlea = new string(Enumerable.Repeat(allChar, 40).Select(letra =>letra[rand.Next(allChar.Length)]).ToArray());


            return numAlea;
        }
        public string AleatoriosProd()
        {
            var rand = new Random();
            var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var numAlea = new string(Enumerable.Repeat(allChar, 60).Select(letra => letra[rand.Next(allChar.Length)]).ToArray());


            return numAlea;
        }
        public string AleatoriosComprarProd()
        {
            var rand = new Random();
            var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var numAlea = new string(Enumerable.Repeat(allChar, 8).Select(letra => letra[rand.Next(allChar.Length)]).ToArray());


            return numAlea;
        }
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

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
