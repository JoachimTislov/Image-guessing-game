using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.OracleContext.Pipelines;

public class GetOracleById<T> where T : class 
{
    public record Request(Guid OracleId) : IRequest<GenericOracle<T>>;

    public class Handler : IRequestHandler<Request, GenericOracle<T>>
    {
        private readonly GameContext _db;

        public Handler(GameContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        
        public async Task<GenericOracle<T>> Handle(Request request, CancellationToken cancellationToken)
        {
          var Oracle = await _db.Oracles
            .Where(o => o.Id == request.OracleId)
            .OfType<GenericOracle<T>>()
            .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception($"Oracle, with type: {typeof(T)} and Id: {request.OracleId} not found");
        
            return Oracle;
        }
    }
}
