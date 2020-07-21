using UnityEngine;

namespace drstc.nociincon
{
    public class NociArray : MonoBehaviour
    {
        public Vector2Int spriteCount = new Vector2Int(8, 4);
        public float spriteSize = 1f;
        public float gap = 0.1f;

        private NociRenderer[] nociRends;

        private void Start()
        {
            var config = new NociConfig(new Vector2Int(10, 10), 2, true);

            var dimLength = spriteCount.x * spriteCount.y;
            nociRends = new NociRenderer[dimLength];

            // 100.0 = pixel per unit value
            var spriteScale = 100.0f / (config.Dimension.x / spriteSize);

            var offset = (Vector2)(spriteCount) / 2f * spriteSize;
            offset.x = offset.x + ((gap * (spriteCount.x - 1)) / 2f) - (spriteSize * 0.5f);
            offset.y = offset.y + ((gap * (spriteCount.y - 1)) / 2f) - (spriteSize * 0.5f);

            for (var i = 0; i < nociRends.Length; i++)
            {
                var x = (i % spriteCount.x) * spriteSize - offset.x;
                var y = offset.y - (i / spriteCount.x) * spriteSize;
                x += gap * (i % spriteCount.x);
                y -= gap * (i / spriteCount.x);

                var go = new GameObject("Noci-" + i);
                go.transform.parent = transform;
                go.transform.position = new Vector3(x, y, 0f);

                var nociGo = new GameObject("NociRenderer", typeof(SpriteRenderer));
                nociGo.transform.parent = go.transform;
                nociGo.transform.localPosition = Vector3.zero;
                nociGo.transform.localScale = Vector3.one * spriteScale;

                nociRends[i] = nociGo.AddComponent<NociRenderer>();
                nociRends[i].SetConfig(config);
                nociRends[i].Refresh();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var noci in nociRends)
                {
                    noci.Refresh();
                }
            }
        }
    }
}
