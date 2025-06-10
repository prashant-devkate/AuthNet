using AuthNet.Models.Domain;

namespace AuthNet.Services.Interfaces
{
    public interface IImageService
    {
        Task<Image> Upload(Image image);
    }
}
