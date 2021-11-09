using Cosmos.Api.Services;
using Cosmos.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.UnitTests
{
    [TestFixture]
    public class CandidateServiceTest
    {
        private IQueryable<Candidate> candidates;
        private List<string> technologies;

        [SetUp]
        public void Setup()
        {
            candidates = new List<Candidate>()
            {
                new Candidate(){ Id = Guid.NewGuid().ToString("n"), FirstName = "FirstName_1", LastName = "LastName_1"  },
                new Candidate(){ Id = Guid.NewGuid().ToString("n"), FirstName = "FirstName_2", LastName = "LastName_2"  },
                new Candidate(){ Id = Guid.NewGuid().ToString("n"), FirstName = "FirstName_3", LastName = "LastName_3"  }
            }.AsQueryable();

            technologies = new List<string>() { "Azure", "SQL", "C#", "Angular", "HTML", "CSS" };
        }

        [Test]
        public async Task GetAll_Success()
        {
            // Arrange
            var mockService = new Mock<ICandidateService>();
            mockService.Setup(x => x.GetAsync()).Returns(async () =>
            {
                await Task.Yield();
                return candidates;
            });

            // Act
            var actual = await mockService.Object.GetAsync();

            // Assert
            Assert.AreEqual(candidates.Count(), actual.Count());
        }

        [Test]
        public async Task Get_Success()
        {
            // Arrange
            var candidate = new Candidate
            {
                Id = Guid.NewGuid().ToString("n"),
                FirstName = "Solly",
                LastName = "Fathi",
                Technologies = technologies.Take(2).ToArray()
            };

            var mockService = new Mock<ICandidateService>();
            mockService.Setup(x => x.GetAsync(candidate.Id)).Returns(async () =>
            {
                await Task.Yield();
                return candidate;
            });

            // Act
            await mockService.Object.CreateAsync(candidate);
            var actual = await mockService.Object.GetAsync(candidate.Id);

            // Assert
            Assert.AreEqual(candidate, actual);
        }

        [Test]
        public async Task Get_NotFound_Success()
        {
            // Arrange
            var candidateId = Guid.NewGuid().ToString();
            var mockService = new Mock<ICandidateService>();

            mockService.Setup(x => x.GetAsync(candidateId)).Returns(async () =>
            {
                await Task.Yield();
                return null;
            });

            // Act
            var actual = await mockService.Object.GetAsync(candidateId);

            // Assert
            mockService.Verify(m => m.GetAsync(candidateId), Times.AtLeastOnce());
            Assert.AreEqual(null, actual);
        }

        [Test]
        public async Task Add_Success()
        {
            // Arrange
            var candidate = new Candidate
            {
                Id = Guid.NewGuid().ToString("n"),
                FirstName = "Solly",
                LastName = "Fathi",
                Technologies = technologies.Take(2).ToArray()
            };

            var mockService = new Mock<ICandidateService>();
            mockService.Setup(x => x.CreateAsync(It.IsAny<Candidate>())).Returns(async () =>
            {
                await Task.Yield();
                return candidate;
            });

            // Act
            var actual = await mockService.Object.CreateAsync(candidate);

            // Assert
            Assert.AreEqual(candidate, actual);
        }

        [Test]
        public void Add_IsNull_Failure_Throws()
        {
            string errorMessage = "Candidate cannot be null";

            // Arrange
            var candidate = It.IsAny<Candidate>();

            // Act and Assert
            Assert.That(async () =>
                await Add_ThrowException(candidate, errorMessage),
                Throws.Exception.TypeOf<Exception>().And.Message.EqualTo(errorMessage));
        }

        [Test]
        public void Add_FirstNameIsEmpty_Failure_Throws()
        {
            string errorMessage = "First name cannot be empty";

            // Arrange
            var candidate = new Candidate
            {
                Id = Guid.NewGuid().ToString("n"),
                LastName = "Fathi",
                Technologies = technologies.Take(2).ToArray()
            };

            // Act and Assert
            Assert.That(async () =>
                await Add_ThrowException(candidate, errorMessage),
                Throws.Exception.TypeOf<Exception>().And.Message.EqualTo(errorMessage));
        }

        [Test]
        public async Task Update_Success()
        {
            // Arrange
            var candidate = new Candidate
            {
                Id = Guid.NewGuid().ToString("n"),
                FirstName = "Solly",
                LastName = "Fathi",
                Email = "mail@gmail.com",
                RegistrationDate = DateTime.Now,
                Technologies = technologies.Take(2).ToArray()
            };

            var mockService = new Mock<ICandidateService>();
            mockService.Setup(x => x.UpdateAsync(candidate.Id, candidate.LastName, It.IsAny<Candidate>())).Returns(async () =>
            {
                await Task.Yield();
                return candidate;
            });

            // Act
            await mockService.Object.UpdateAsync(candidate.Id, candidate.LastName, candidate);

            // Assert
            mockService.Verify(m => m.UpdateAsync(candidate.Id, candidate.LastName, It.IsAny<Candidate>()), Times.AtLeastOnce());
            Assert.That(candidate.FirstName, Is.EqualTo("Solly"));
        }

        [Test]
        public async Task Delete_Success()
        {
            // Arrange
            var candidate = new Candidate
            {
                Id = Guid.NewGuid().ToString("n"),
                FirstName = "Solly",
                LastName = "Fathi",
                Technologies = technologies.Take(2).ToArray()
            };

            var mockService = new Mock<ICandidateService>();
            mockService.Setup(x => x.DeleteAsync(candidate.Id)).Returns(async () =>
            {
                await Task.Yield();
                return candidate;
            });

            // Act
            await mockService.Object.DeleteAsync(candidate.Id);
            var actual = await mockService.Object.GetAsync(candidate.Id);

            // Assert
            mockService.Verify(m => m.DeleteAsync(candidate.Id));
            mockService.Verify(m => m.GetAsync(candidate.Id));
            Assert.AreEqual(null, actual);
        }

        static async Task Add_ThrowException(Candidate candidate, string errorMessage)
        {
            var mockService = new Mock<ICandidateService>();
            await mockService.Object.CreateAsync(candidate).ConfigureAwait(false);
            throw new Exception(errorMessage);
        }
    }
}