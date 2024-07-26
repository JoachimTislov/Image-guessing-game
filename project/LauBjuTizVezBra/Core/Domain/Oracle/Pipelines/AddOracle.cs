using Infrastructure.Data;
using MediatR;

namespace Core.Domain.OracleContext.Pipelines
{
    public class AddOracle<T> where T : class
    {
        public record Request(T Oracle, string ImageIdentifier) : IRequest<Guid>;

        public class Handler : IRequestHandler<Request, Guid>
        {
            private readonly GameContext _db;

            public Handler(GameContext db)
                => _db = db ?? throw new ArgumentNullException(nameof(db));

            public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
            {
                // Create an instance of GenericOracle<T> where T is the type of Oracle
                var genericOracle = new GenericOracle<T>(request.Oracle);
                genericOracle.AssignImageId(request.ImageIdentifier); 

                _db.Oracles.Add(genericOracle);
                await _db.SaveChangesAsync(cancellationToken);

                return genericOracle.Id;
            }
        }
    }
}