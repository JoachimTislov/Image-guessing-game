using Infrastructure.Data;
using MediatR;

namespace Core.Domain.ImageContext.Pipelines;


//This is only needed if you want add the Images with relevant info to the database 
//For example when you delete the database and want to add the images again
public class DeleteImageRecords
{
    public record Request() : IRequest<Unit>;

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly GameContext _db;
        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            _db.ImageRecords.RemoveRange(_db.ImageRecords);
            await _db.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}