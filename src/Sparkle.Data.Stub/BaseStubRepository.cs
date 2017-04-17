using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparkle.Data.Stub
{
    public class BaseStubRepository
    {
        protected StaticData Data
        {
            get { return StaticData.Default; }
        }
    }
}
