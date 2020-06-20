using System.Collections;
using System.Collections.Generic;
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
            var config = new NociConfig(10, 2);

            var dimLength = spriteCount.x * spriteCount.y;
            nociRends = new NociRenderer[dimLength];

            var offset = (Vector2)(spriteCount / 2);
            offset.x = offset.x + (gap * (spriteCount.x - 1) / 2f);
            offset.y = offset.y + (gap * (spriteCount.y - 1) / 2f);

            // 100.0 = pixel per unit value
            var spriteScale = 100.0f / (spriteSize * config.Dimension);

            for (var i = 0; i < nociRends.Length; i++)
            {
                var x = (i % spriteCount.x) - offset.x;
                var y = offset.y - (i / spriteCount.x);
                x += gap * (i % spriteCount.x);
                y -= gap * (i / spriteCount.x);

                var go = new GameObject("Noci-" + i);
                go.transform.parent = transform;
                go.transform.position = new Vector3(x, y, 0f);

                var nociGo = new GameObject("NociRenderer", typeof(SpriteRenderer));
                nociGo.transform.parent = go.transform;
                nociGo.transform.localPosition = Vector3.zero;
                nociGo.transform.localScale = Vector3.one * spriteScale;

                //config.Iterations = config.Iterations % spriteCount.y; 

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
