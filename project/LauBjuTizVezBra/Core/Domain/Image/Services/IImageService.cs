namespace Core.Domain.ImageContext.Services;

public interface IImageService
{
    List<string> GetFileNameOfImagePieces(string imageIdentifier);

    List<(string ImageId, List<(int X, int Y)> Coordinates)> GetCoordinatesForImagePieces(string imagePiecesFolderPath, List<string> ImagePieceList);
}