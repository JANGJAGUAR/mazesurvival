using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColorChange : MonoBehaviour
{
    private Renderer r;

    private MaterialPropertyBlock mpb;

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
        Color customColor = new Color(1f, 0.825f, 0.978f, 1.0f);
        mpb.SetColor("_Color", customColor);
        r.SetPropertyBlock(mpb);
    }

    private void OnMouseExit()
    {
        //Color customColor = new Color(0.828f, 0.708f, 0.327f, 1.0f);
        Color customColor = new Color(1f, 1f, 1f, 1.0f);
        mpb.SetColor("_Color", customColor);
        r.SetPropertyBlock(mpb);
    }


}
