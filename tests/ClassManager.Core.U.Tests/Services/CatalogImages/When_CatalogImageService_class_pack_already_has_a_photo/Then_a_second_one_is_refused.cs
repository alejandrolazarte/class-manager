using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.U.Tests.Services.CatalogImages.When_CatalogImageService_class_pack_already_has_a_photo;

public sealed class Then_a_second_one_is_refused
{
    [Fact]
    public async Task Then_a_second_one_is_refused_Run()
    {
        var builder = new CatalogImageServiceBuilder();
        var classPack = CatalogImageSamples.ClassPackWithImages(ClassPack.MaximumImages);

        var result = await builder.Build().AddAsync(classPack, DocumentOwner.ClassPack, CatalogImageSamples.Png, CancellationToken.None);

        result.Error!.Code.ShouldBe(ImageErrorCodes.LimitReached);
    }
}
