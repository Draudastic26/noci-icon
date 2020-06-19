using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace drstc.nociincon
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class NociRend : MonoBehaviour
    {
        SpriteRenderer rend;
        void Start()
        {
            rend = GetComponent<SpriteRenderer>();

            var factory = new NociFactory();

            rend.sprite = factory.GetSprite();
        }

    }
}
