using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);//IFormFile represent file sent with the HttpRequest
        Task<DeletionResult> DeletePhotoAsync(string publicId);//publicId sent by cloudinary,and it is use to delete the image
    }
}
