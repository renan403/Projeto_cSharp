using Firebase.Database;

namespace ApiMvc.FireData
{
    public class Base : IDisposable
    {
        protected readonly FirebaseClient _client ;
        private bool disposedValue;
        public Base()
        {         
           _client = new FirebaseClient("https://projetoport-50b66-default-rtdb.firebaseio.com/");
        }
        protected async Task<string> GetUserKey(string userId)
        {
            return (await _client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(m => m.Object.IdUser == userId).FirstOrDefault().Key;
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
        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }          
}
    

