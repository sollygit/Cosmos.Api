using Cosmos.Interface;
using Cosmos.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
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
            mockService.Setup(x => x.GetAll()).Returns(async () =>
            {
                await Task.Yield();
                return candidates;
            });

            // Act
            var actual = await mockService.Object.GetAll();

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
            mockService.Setup(x => x.Get(candidate.Id)).Returns(async () =>
            {
                await Task.Yield();
                return candidate;
            });

            // Act
            await mockService.Object.Add(candidate);
            var actual = await mockService.Object.Get(candidate.Id);

            // Assert
            Assert.AreEqual(candidate, actual);
        }

        [Test]
        public async Task Get_NotFound_Success()
        {
            // Arrange
            var candidateId = Guid.NewGuid().ToString();
            var mockService = new Mock<ICandidateService>();

            mockService.Setup(x => x.Get(candidateId)).Returns(async () =>
            {
                await Task.Yield();
                return null;
            });

            // Act
            var actual = await mockService.Object.Get(candidateId);

            // Assert
            mockService.Verify(m => m.Get(candidateId), Times.AtLeastOnce());
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
            mockService.Setup(x => x.Add(It.IsAny<Candidate>())).Returns(async () =>
            {
                await Task.Yield();
                return candidate;
            });

            // Act
            var actual = await mockService.Object.Add(candidate);

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
            mockService.Setup(x => x.Update(candidate.Id, candidate.LastName, It.IsAny<Candidate>())).Returns(async () =>
            {
                await Task.Yield();
                return candidate;
            });

            // Act
            await mockService.Object.Update(candidate.Id, candidate.LastName, candidate);

            // Assert
            mockService.Verify(m => m.Update(candidate.Id, candidate.LastName, It.IsAny<Candidate>()), Times.AtLeastOnce());
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
            mockService.Setup(x => x.Delete(candidate.Id, candidate.LastName)).Returns(async () =>
            {
                await Task.Yield();
                return candidate;
            });

            // Act
            await mockService.Object.Delete(candidate.Id, candidate.LastName);
            var actual = await mockService.Object.Get(candidate.Id);

            // Assert
            mockService.Verify(m => m.Delete(candidate.Id, candidate.LastName));
            mockService.Verify(m => m.Get(candidate.Id));
            Assert.AreEqual(null, actual);
        }

        private async Task Add_ThrowException(Candidate candidate, string errorMessage)
        {
            var mockService = new Mock<ICandidateService>();
            await mockService.Object.Add(candidate).ConfigureAwait(false);
            throw new Exception(errorMessage);
        }
    }
}