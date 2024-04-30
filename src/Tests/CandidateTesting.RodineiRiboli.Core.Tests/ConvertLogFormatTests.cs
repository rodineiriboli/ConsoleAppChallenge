using CandidateTesting.RodineiRiboli.Core.Interfaces;
using CandidateTesting.RodineiRiboli.Core.Services;
using Moq;

namespace CandidateTesting.RodineiRiboli.Core.Tests
{
    public class ConvertLogFormatTests
    {
        [Theory (DisplayName = "Returns Strings Converteds")]
        [InlineData("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2", "\n\"MINHA CDN\" GET 200 /robots.txt 100 312 HIT")]
        [InlineData("101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4", "\n\"MINHA CDN\" POST 200 /myImages 319 101 MISS")]
        [InlineData("199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9", "\n\"MINHA CDN\" GET 404 /not-found 143 199 MISS")]
        [InlineData("312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1", "\n\"MINHA CDN\" GET 200 /robots.txt 245 312 REFRESH_HIT")]
        [Trait("ConvertLogFormat", "ConvertToAgoraFormat")]
        public void ConvertLogFormat_ConvertToAgoraFormat_ReturnsStringConverted(string v1, string result)
        {
            //Arrange
            var consumeAwsS3 = new Mock<IConsumeAwsS3>();
            var convertLogFormat = new ConvertLogFormat(consumeAwsS3.Object);

            //Act
            var str = convertLogFormat.BodyFileMount(v1);

            //Assert
            Assert.Equal(result, str);
        }


        [Fact (DisplayName = "Returns a tuple with two strings")]
        [Trait("ConvertLogFormat", "Input")]
        public void ConvertLogFormat_Input_ReturnsTupleTwoStrings()
        {
            // Arrange
            string input = "convert http://aws_s3.com/file.txt ./output/file.txt";
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);
            var consumeAwsS3 = new Mock<IConsumeAwsS3>();
            var convertLogFormat = new ConvertLogFormat(consumeAwsS3.Object);

            // Act
            var (uri, path) = convertLogFormat.Input();

            // Assert
            Assert.Equal("http://aws_s3.com/file.txt", uri);
            Assert.Equal("./output/file.txt", path);
        }

        [Fact(DisplayName = "Check console data entry")]
        [Trait("ConvertLogFormat", "CheckEntry")]
        public void ConvertLogFormat_CheckEntry_ExecutesTheMethodAndReturnsTheInputItself()
        {
            // Arrange
            string input = "convert http://aws_s3.com/file.txt ./output/file.txt";
            var consumeAwsS3 = new Mock<IConsumeAwsS3>();
            var convertLogFormat = new ConvertLogFormat(consumeAwsS3.Object);

            // Act
            var inpuReturned = convertLogFormat.CheckEntry(input);

            // Assert
            Assert.Equal("convert http://aws_s3.com/file.txt ./output/file.txt", inpuReturned);
        }
    }
}
