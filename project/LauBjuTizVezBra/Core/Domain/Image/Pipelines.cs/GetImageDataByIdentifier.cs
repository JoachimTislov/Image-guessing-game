using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.ImageContext.Pipelines;

public class GetImageDataByIdentifier
{
    public record Request(string Identifier) : IRequest<ImageData>;

    public class Handler : IRequestHandler<Request, ImageData>
    {
        private readonly GameContext _db;
        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<ImageData> Handle(Request request, CancellationToken cancellationToken)
        {
            var Image = await _db.ImageRecords
            .Where(i => i.Identifier == request.Identifier)
            .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception($"Image with Identifier: {request.Identifier} not found");

            Image.Link = Image.Link.Replace("\\", Path.DirectorySeparatorChar.ToString());
            Image.FolderWithImagePiecesLink = Image.FolderWithImagePiecesLink.Replace("\\", Path.DirectorySeparatorChar.ToString());

            return Image;
        }
    }
}