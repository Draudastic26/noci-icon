using UnityEngine;

namespace drstc.noci
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class NociRenderer : MonoBehaviour
    {
        public NociConfig config;
        private SpriteRenderer rend;

        private Noci noci;

        private void Start()
        {
            if (config == null) SetConfig(null);
        }

        public void SetConfig(NociConfig newConfig)
        {
            if (rend == null) rend = GetComponent<SpriteRenderer>();

            // Start with default config when newConfig is null
            if (newConfig == null) newConfig = new NociConfig(new Vector2Int(10, 10), 2, true);

            config = newConfig;
            
            // with random seed
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
