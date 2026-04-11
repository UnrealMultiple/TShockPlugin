using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaForce.Protocol.Interfaces;

public interface IRequestPacket : INetPacket
{
    Guid RequestId { get; set; }
}
