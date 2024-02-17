
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AODb.Data.TemplateData
{
    public class AnimationMesh 
    {
        public StatValue Animation { get; private set; }
        public StatValue Mesh { get; private set; }

        public AnimationMesh() { }

        public void PopulateFromStream(BinaryReader reader)
        {
            int confirm = reader.ReadInt32();
            if(!(confirm == 0x1b)) { throw new System.Exception("Confirmation Check Failed!"); }
            
            int itemCount = reader.ReadInt32();
            itemCount = (itemCount / 1009) -1;

            for(int i = 0; i < itemCount; i++) {
                StatValue sv = new StatValue();
                sv.PopulateFromStream(reader);
                if(sv.Stat == Stat.Anim) { this.Animation = sv; }
                else if(sv.Stat == Stat.Mesh) { this.Mesh = sv; }
            }
        }
    }
}
