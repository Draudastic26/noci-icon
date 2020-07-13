using UnityEngine;

namespace drstc.nociincon
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class NociRenderer : MonoBehaviour
    {
        private SpriteRenderer rend;
        public NociConfig config;

        private Noci noci;

        private void Start()
        {
            if (config == null) SetConfig(null);
        }

        public void SetConfig(NociConfig newConfig)
        {
            // Start with default config
            if (newConfig == null) newConfig = new NociConfig(new Vector2Int(10, 10), 2, true);
            if (rend == null) rend = GetComponent<SpriteRenderer>();
            config = newConfig;
            noci = new Noci(config);
            rend.sprite = noci.GetSprite();
        }

        public void Refresh()
        {
            noci.Reroll();
            rend.sprite = noci.GetSprite();
        }
    }
}
