using SharedKernel;

namespace Core.Domain.ImageContext;

public class ImageData : BaseEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Identifier { get; set; } = null!;

    public string Link { get; set; } = null!;

    public string FolderWithImagePiecesLink { get; set; } = null!;

    public int PieceCount { get; set; }

    public ImageData(){}

    public ImageData(string name, string identifier, string link, string folderWithImagePiecesLink, int pieceCount)
    {
        Name = name;
        Identifier = identifier;
        Link = link;
        FolderWithImagePiecesLink = folderWithImagePiecesLink;
        PieceCount = pieceCount;
    }
}