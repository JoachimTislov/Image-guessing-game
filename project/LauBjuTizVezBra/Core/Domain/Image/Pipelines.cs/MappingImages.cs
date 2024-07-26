


// This code is not used anymore, unless
// we lose the mapping file, which we also dont need
// Unless we loose all ImageRecords in the database

using MediatR;

namespace Core.Domain.ImageContext.Pipelines;

public class MappingImages
{
    public record Request() : IRequest<Unit>;

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IMediator _mediator;
        public Handler(IMediator mediator) 
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        } 

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var DataSetPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","DataSet");

            var image_mapping_lines = await File.ReadAllLinesAsync(Path.Combine(DataSetPath, "image_mapping.csv"), cancellationToken);

            var imagePath = Path.Combine(DataSetPath, "ScatteredImages");

            string[] subdirectories = Directory.GetDirectories(imagePath);

            // Print the names of all subdirectories
            for (var i = 0; i < subdirectories.Length; i++)
            {
                var split2 = subdirectories[i].Split('\\');
                var name1 = split2[^1];
                for(var j = 0; j < image_mapping_lines.Length; j++)
                {
                    var Line = image_mapping_lines[j];
                    var split = Line.Split(' ');
                    var imageIdentifier = split[0]; 
                    var image_nameId1 = split.Length > 1 ? split[1] : null;

                    var name = $"{imageIdentifier}_scattered";
                    if (name1 == name)
                    {
                        var imageName1 = await _mediator.Send(new GetImageNameBy_Label_Mapping.Request(image_nameId1, DataSetPath), cancellationToken);
                        Console.WriteLine(name1 + " " + imageName1);
                    }
                }
            }

            return Unit.Value;
        }
    }
}