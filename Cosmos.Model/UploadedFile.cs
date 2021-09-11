using System;

namespace Cosmos.Model
{
    public class UploadedFile
    {
        public string Id { get { return Uri?.Split("_")?[1]?.Split(".")?[0]; } }
        public string Uri { get; set; }
        public string FileName { get; set; }
        public DateTimeOffset? LastModified { get; set; }
    }
}
