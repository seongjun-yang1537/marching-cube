using System.Collections;
using Corelib.Utils;

namespace Ingame
{
    public interface IBSP3DGenerationStep
    {
        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model);
    }
}