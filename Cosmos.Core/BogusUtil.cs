using Bogus;
using Cosmos.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cosmos.Common
{
    public static class BogusUtil
    {
        public static List<Candidate> Candidates(int count)
        {
            var candidates = GetCandidates(count-1);
            var inactive = GetCandidate(false);

            candidates.Add(inactive);

            return candidates;
        }

        private static Candidate GetCandidate(bool isActive = true)
        {
            // Mockup a candidate
            return new Faker<Candidate>()
                .RuleFor(c => c.Id, f => Guid.NewGuid().ToString("N"))
                .RuleFor(c => c.FirstName, f => f.Name.FirstName())
                .RuleFor(c => c.LastName, f => f.Name.LastName())
                .RuleFor(c => c.Email, f => f.Internet.Email().ToLower())
                .RuleFor(c => c.RegistrationDate, f => f.Date.Recent(30))
                .RuleFor(c => c.Technologies, GetTechnologies(3))
                .RuleFor(c => c.IsActive, isActive)
                .Generate(1)
                .FirstOrDefault();
        }

        private static List<Candidate> GetCandidates(int count)
        {
            var candidates = new List<Candidate>();

            for (int i = 0; i < count; i++)
            {
                candidates.Add(GetCandidate());
            }

            return candidates;
        }

        private static string[] GetTechnologies(int count)
        {
            var technologies = new Faker<Technology>()
                .RuleFor(o => o.Name, f => f.Rant.Random.CollectionItem(Constants.Technologies))
                .Generate(count)
                .Select(o => o.Name)
                .Distinct()
                .ToArray();
            return technologies;
        }

        class Technology
        {
            public string Name { get; set; }
        }
    }
}
