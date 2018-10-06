using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceEdit
{
    public sealed class CacheManager
    {
        public ApplicationContext Context { get; private set; }

        public CacheManager(ApplicationContext context)
        {
            Assert.ArgumentNotNull (context, nameof (context));
            Context = context;
        }
    }
}
