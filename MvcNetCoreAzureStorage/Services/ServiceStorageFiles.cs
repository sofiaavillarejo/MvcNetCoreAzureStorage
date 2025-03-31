using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Files.Shares;

namespace MvcNetCoreAzureStorage.Services
{
    public class ServiceStorageFiles
    {
        //TODO SERVICIO STORAGE SIEMPRE UTILIZA CLIENTS PARA TRABAJAR
        //UN CLIENT PUEDE SER DIRECTAMENTE SOBRE UN SHARED O PODRIA SER SOBRE TODO EL SERVICIO DE FILES
        private ShareDirectoryClient root;

        public ServiceStorageFiles(IConfiguration configuration)
        {
            string keys = configuration.GetValue<string>("AzureKeys:StorageAccount");
            //NUESTRO CLIENTE TRABAJARA SOBRE UN SHARED DETERMINADO 
            ShareClient client = new ShareClient(keys, "ejemploiles"); //ES EJEMPLOFILES PERO ME HE COMIDO LA F AL CREARLO
            this.root = client.GetRootDirectoryClient();
        }

        //METODO PARA RECUPERAR TODOS LOS FICHEROS DE LA RAIZ DEL SHARED
        public async Task<List<string>> GetFilesAsync()
        {
            List<string> files = new List<string>();

            await foreach (ShareFileItem item in this.root.GetFilesAndDirectoriesAsync())
            {
                files.Add(item.Name);
            }
            return files;
        }

        public async Task<string> ReadFileAsync(string fileName)
        {
            ShareFileClient file = this.root.GetFileClient(fileName);
            ShareFileDownloadInfo data = await file.DownloadAsync();

            Stream stream = data.Content;
            string contenido = "";

            using (StreamReader reader = new StreamReader(stream))
            {
                contenido = await reader.ReadToEndAsync();
            }
            return contenido;
        }

        public async Task UploadFileAsync(string fileName, Stream stream)
        {
            ShareFileClient file = this.root.GetFileClient(fileName);
            await file.CreateAsync(stream.Length);
            await file.UploadAsync(stream);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            ShareFileClient file = this.root.GetFileClient(fileName);
            await file.DeleteAsync();
        }
    }
}
