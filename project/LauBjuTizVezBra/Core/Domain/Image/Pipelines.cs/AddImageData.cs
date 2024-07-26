using Infrastructure.Data;
using MediatR;

namespace Core.Domain.ImageContext.Pipelines;

public class AddImageData
{
    public record Request(ImageData ImageData) : IRequest<ImageData>;

    public class Handler : IRequestHandler<Request, ImageData>
    {
        private readonly GameContext _db;
        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<ImageData> Handle(Request request, CancellationToken cancellationToken)
        {
            _db.ImageRecords.Add(request.ImageData);

            await _db.SaveChangesAsync(cancellationToken);

            return request.ImageData;
        }
    }
}