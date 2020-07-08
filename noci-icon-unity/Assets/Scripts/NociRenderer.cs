using UnityEngine;

namespace drstc.nociincon
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class NociRenderer : MonoBehaviour
    {
        private SpriteRenderer rend;
        private NociConfig config;

        public void SetConfig(NociConfig newConfig)
        {   
            config = newConfig;
        }

        public void Refresh()
        {
            if (config == null) config = new NociConfig(8, 2);
            if (rend == null) rend = GetComponent<SpriteRenderer>();

            rend.sprite = NociFactory.GetSprite(config);
        }
    }
}
