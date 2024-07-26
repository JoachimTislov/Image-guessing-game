using MediatR;
using NuGet.Packaging;

namespace Core.Domain.ImageContext.Pipelines;

public class GetRandomImage
{
    public record Request() : IRequest<ImageData>;

    public class Handler : IRequestHandler<Request, ImageData>
    {
        private readonly IMediator _mediator;
        public Handler(IMediator mediator) 
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        } 

        public async Task<ImageData> Handle(Request request, CancellationToken cancellationToken)
        {
            var MappedImagesFile = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","DataSet","Mapped_Images.csv"); 

            if(!File.Exists(MappedImagesFile))
            {
                throw new Exception($"{MappedImagesFile} does not exist");
            }
            var Mapped_Images_Lines = await File.ReadAllLinesAsync(MappedImagesFile, cancellationToken);

            var random = new Random();

            var randomNumber = random.Next(0, Mapped_Images_Lines.Length);
            var Line = Mapped_Images_Lines[randomNumber];

            var split = Line.Split(' ');
            var image_Identifier = split[0];

            var ImageData = await _mediator.Send(new GetImageDataByIdentifier.Request(image_Identifier), cancellationToken);

            return ImageData;
        }
    }
}