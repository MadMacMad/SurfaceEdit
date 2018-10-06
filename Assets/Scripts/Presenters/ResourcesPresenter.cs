using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceEdit.Presenters
{
    public sealed class ResourcesPresenter
    {
        private ResourcesViewData viewData;

        public ResourcesPresenter(ResourcesViewData viewData)
        {
            Assert.ArgumentNotNull (viewData, nameof (viewData));

            this.viewData = viewData;
        }
    }
}
