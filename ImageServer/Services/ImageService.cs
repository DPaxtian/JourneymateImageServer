using Grpc.Core;
using ImageServer;

namespace ImageServer.Services
{
    public class ImageService : ImageServices.ImageServicesBase
    {
        string solutionFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        string relativeFolderPath = @"..\ProfileImages";


    public override async Task<Google.Protobuf.WellKnownTypes.Empty> SaveImage(Image request, ServerCallContext context)
        {
            string imageName = request.Name;
            byte[] imageData = request.Data.ToByteArray();
            string absoluteFolderPath = Path.GetFullPath(Path.Combine(solutionFolderPath, relativeFolderPath));

            // Verificar si la carpeta existe y crearla si no
            if (!Directory.Exists(absoluteFolderPath))
            {
                Directory.CreateDirectory(absoluteFolderPath);
            }

            string imagePath = Path.Combine(absoluteFolderPath, imageName);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            using (var fileStream = File.Create(imagePath))
            {
                await fileStream.WriteAsync(imageData, 0, imageData.Length);
            }

            return new Google.Protobuf.WellKnownTypes.Empty();
        }


        public override async Task<DownloadImageResponse> DownloadImage(DownloadImageRequest request, ServerCallContext context)
        {
            string imageName = request.ImageName;
            string absoluteFolderPath = Path.GetFullPath(Path.Combine(solutionFolderPath, relativeFolderPath));
            string imagePath = Path.Combine(absoluteFolderPath, imageName);

            if (!Directory.Exists(absoluteFolderPath))
            {
                Directory.CreateDirectory(absoluteFolderPath);
            }


            if (!File.Exists(imagePath))
            {
                return null;
            }

            byte[] imageData;

            using (var fileStream = File.OpenRead(imagePath))
            {
                imageData = new byte[fileStream.Length];
                await fileStream.ReadAsync(imageData, 0, imageData.Length);
            }

            var response = new DownloadImageResponse
            {
                ImageData = Google.Protobuf.ByteString.CopyFrom(imageData)
            };

            return response;
        }

    }
}
