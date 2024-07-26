using MediatR;

namespace Core.Domain.ImageContext.Pipelines;

public class GetImageNameBy_Label_Mapping
{
    public record Request(string Image_nameId, string DataSetPath) : IRequest<string>;

    public class Handler : IRequestHandler<Request, string>
    {
        public Handler() { }
        public async Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            var label_mappingPath = Path.Combine($"{request.DataSetPath}/label_mapping.csv");
            var label_mapping_lines = await File.ReadAllLinesAsync(label_mappingPath, cancellationToken);

            foreach (var line in label_mapping_lines)
            {
                var split = line.Split(' ');
                var nameId = split[0];
                var image_name = split[1];

                if(nameId == request.Image_nameId)
                {
                    return image_name;
                }
            }
            return string.Empty;
        }
    }
}
