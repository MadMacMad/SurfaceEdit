using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SurfaceEdit.Presenters
{
    public sealed class ResourcesViewData : ViewData
    {
        public Button[] importResourcesButtons;
        public GameObject resourcePrefab;
        public GameObject resourcesParent;
    }
}
