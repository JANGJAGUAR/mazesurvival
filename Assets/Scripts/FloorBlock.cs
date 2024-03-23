using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBlock : MonoBehaviour
{
    public GameObject overBlock;
    private Renderer r;
    private MaterialPropertyBlock mpb;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        if(gameObject.tag == "Ground")
        {
            Color customColor = new Color(0.95f, 0.75f, 0.75f, 1.0f);
            mpb.SetColor("_Color", customColor);
            r.SetPropertyBlock(mpb);
        }
    }

    private void OnMouseExit()
    {
        if(gameObject.tag == "Ground")
        {
            //Color customColor = new Color(0.841f, 0.751f, 0.567f, 1.0f);
            Color customColor = new Color(1f, 1f, 1f, 1.0f);
            mpb.SetColor("_Color", customColor);
            r.SetPropertyBlock(mpb);
        }
    }
}
