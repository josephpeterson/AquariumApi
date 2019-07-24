using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class SnapshotRequest
    {
        public IFormFile PhotoFile { get; set; }
        public AquariumSnapshot Snapshot { get; set; }
    }
}
