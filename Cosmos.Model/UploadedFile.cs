using System;

namespace Cosmos.Model
{
    public class UploadedFile
    {
        public string Id { get { return Name?.Split(".")?[0]; } }
        public string Uri { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? LastModified { get; set; }
    }
}
