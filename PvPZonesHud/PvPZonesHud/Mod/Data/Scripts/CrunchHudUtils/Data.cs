using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace CrunchHudUtils.Data.Scripts.PvPArea
{
    [ProtoContract]
    public class Data
    {
        [ProtoContract]
        public class PlayerDataPvP
        {
            [ProtoMember(1)]
            public bool WarEnabled;

            [ProtoMember(2)]
            public List<PvPArea> PvPAreas;
        }
        [ProtoContract]
        public class PvPArea
        {
            [ProtoMember(1)]
            public Vector3 Position;

            [ProtoMember(2)]
            public int Distance;

            [ProtoMember(3)]
            public string Name;
        }

        [ProtoContract]
        public class ChatStatus
        {
            [ProtoMember(1)]
            public bool ChatEnabled;
        }
    }
}
