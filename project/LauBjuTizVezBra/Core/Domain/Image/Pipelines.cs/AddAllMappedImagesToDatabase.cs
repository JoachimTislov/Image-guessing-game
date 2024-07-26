using System.Drawing;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core.Domain.ImageContext.Pipelines;


//This is only needed if you want add the Images with relevant info to the database 
//For example when you delete the database and want to add the images again

//Does not work atm, will delete this later
public class AddAllMappedImagesToDatabase
{
    public record Request() : IRequest<Unit>;

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IMediator _mediator;
        public Handler(IMediator mediator) => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var DataSetFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet");
            var Mapped_ImagesFile = Path.Combine(DataSetFolder, "Mapped_Images.csv");
    
            if(File.Exists(Mapped_ImagesFile))
            {
                var Mapped_Images_Lines = await File.ReadAllLinesAsync(Mapped_ImagesFile, cancellationToken);

                foreach(var line in Mapped_Images_Lines)
                {
                    var split = line.Split(' ');
                    var image_Identifier = split[0];
                    var image_Name = split[1];

                    
                    var ImageLink = Path.Combine("DataSet","MergedImages", image_Identifier +".png");
                
                    var pieceCount = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "ScatteredImages", image_Identifier)).Length;

                    var ImagePieceFolderLink = Path.Combine("wwwroot", "DataSet","ScatteredImages", image_Identifier);

                    var ImageData = new ImageData(image_Name, image_Identifier, ImageLink, ImagePieceFolderLink, pieceCount);
                    
                    await _mediator.Send(new AddImageData.Request(ImageData), cancellationToken);

                    //MERGE IMAGES
                    //await _mediator.Send(new ImageMerger.Request(ImageData.Identifier), cancellationToken);
                }
            }
            return Unit.Value;
        }
    }
}